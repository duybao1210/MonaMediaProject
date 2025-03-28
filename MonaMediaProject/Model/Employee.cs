using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MonaMediaProject.Model
{
    [Table("Temp_Employees")] // Đảm bảo Entity Framework map đúng bảng
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string CodeEmp { get; set; } // Cột tính toán

        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }

        [NotMapped] // Vì Age được tính từ DateOfBirth
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
    }
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string CodeEmp { get; set; }
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }      
        public int Age { get; set; }
    }
    public class EmployeeAPIModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<EmployeeViewModel> Data { get; set; }
    }
}
