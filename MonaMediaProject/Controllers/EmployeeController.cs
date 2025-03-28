using Microsoft.AspNetCore.Mvc;
using MonaMediaProject.Services.Interface;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MonaMediaProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        // Inject EmployeeService thông qua constructor
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Authorize] // Yêu cầu JWT Token hợp lệ
        [HttpGet("GetListEmployee")]
        public async Task<IActionResult> GetListEmployee(int page = 1, int pageSize = 10)
        {
            try
            {
                var employees = await _employeeService.GetEmployees(page, pageSize);
                if (employees.Status == "204")
                {
                    return StatusCode(204, employees);
                }
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize] // Yêu cầu JWT Token hợp lệ
        [HttpGet("DownloadTemplate")]
        public IActionResult DownloadTemplate()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employee_Template");

                // Tạo tiêu đề cột
                worksheet.Cells[1, 1].Value = "FullName";
                worksheet.Cells[1, 2].Value = "DateOfBirth (dd/MM/yyyy)";

                // Căn chỉnh tiêu đề
                using (var range = worksheet.Cells[1, 1, 1, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                }
                worksheet.Cells.AutoFitColumns();
                worksheet.Cells[2, 2, 1000, 2].Style.Numberformat.Format = "dd/MM/yyyy";
                // Xuất file Excel
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = "Employee_Template.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }

        [Authorize] // Yêu cầu JWT Token hợp lệ
        [HttpPost("ImportEmployeesFromExcel")]
        public async Task<IActionResult> ImportEmployeesFromExcel([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }
            try
            {
                var result = await _employeeService.ImportEmployeesFromExcel(file);
                if (result)
                {
                    return Ok(new { Message = "Import successful." });
                }
                return BadRequest("Import failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
