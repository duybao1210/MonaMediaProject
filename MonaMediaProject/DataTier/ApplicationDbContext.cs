using Microsoft.EntityFrameworkCore;
using MonaMediaProject.Model;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MonaMediaProject.DataTier
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; } // Bảng Temp_Employees
    }
}
