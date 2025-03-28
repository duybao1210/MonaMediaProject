using MonaMediaProject.Model;

namespace MonaMediaProject.Services.Interface
{
    public interface IEmployeeService
    {
        Task<bool> ImportEmployeesFromExcel(IFormFile file);
        Task<EmployeeAPIModel> GetEmployees(int page, int pageSize);
    }
}
