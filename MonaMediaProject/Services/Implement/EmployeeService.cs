using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using MonaMediaProject.DataTier;
using MonaMediaProject.Model;
using MonaMediaProject.Services.Interface;
using OfficeOpenXml;
using System.Globalization;
using static Azure.Core.HttpHeader;

namespace MonaMediaProject.Services.Implement
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        public EmployeeService(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public async Task<bool> ImportEmployeesFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0) return false;
            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];

                if (worksheet == null)
                    return false;

                var employees = new List<Employee>();

                var data = Enumerable.Range(2, worksheet.Dimension.End.Row - 1)
               .Select(row => new
               {
                   FullName = worksheet.Cells[row, 1].Text.Trim(),
                   DateOfBirthTemp = worksheet.Cells[row, 2].Text.Trim()
               })
               .Where(entry => !string.IsNullOrEmpty(entry.FullName) || !string.IsNullOrEmpty(entry.DateOfBirthTemp)).ToList();


                // Xử lý song song dữ liệu đã trích xuất
                Parallel.ForEach(data, entry =>
                {
                    if (!DateTime.TryParseExact(entry.DateOfBirthTemp, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                    {
                        dateOfBirth = DateTime.Now;
                    }

                    var employee = new Employee
                    {
                        FullName = entry.FullName,
                        DateOfBirth = dateOfBirth
                    };

                    lock (employees)
                    {
                        employees.Add(employee);
                    }
                });

                if (employees.Count == 0)
                    return false;

                foreach (var chunk in employees.Chunk(5000))
                {
                    await _context.Employees.AddRangeAsync(chunk);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                _logService.WriteLogError("Error importing employees from Excel", ex);
                return false;
            }
        }

        public async Task<EmployeeAPIModel> GetEmployees(int page, int pageSize)
        {
            var resultsObject = new EmployeeAPIModel()
            {
                Status = "200",
                Message = "Success",
                Data = null
            };
            try
            {
                var result = await _context.Employees.OrderBy(e => e.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                if (result != null && !result.Any())
                {
                    resultsObject.Status = "204";
                    resultsObject.Message = "No Data";
                    return resultsObject;
                }
                else
                {
                    //Format lại định dạng
                    var formattedResult = result.Select(e => new EmployeeViewModel
                    {
                        Id = e.Id,
                        CodeEmp =  e.CodeEmp,
                        FullName = e.FullName,
                        DateOfBirth = e.DateOfBirth.ToString("dd/MM/yyyy"),
                        Age = DateTime.Now.Year - e.DateOfBirth.Year -
                        (DateTime.Now < e.DateOfBirth.AddYears(DateTime.Now.Year - e.DateOfBirth.Year) ? 1 : 0)
                    }).ToList();

                    resultsObject.Data = formattedResult;
                }
            }
            catch (Exception ex)
            {
                //Ghi log lỗi
                _logService.WriteLogError("Error in progressing", ex);
                resultsObject.Status = "500";
                resultsObject.Message = "Error";
                return resultsObject;              
            }
            return resultsObject;
        }    
    }
}
