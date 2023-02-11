using CsvImportSiteJS.Data;
using CsvImportSiteJS.Interfaces;
using CsvImportSiteJS.Models;
using Microsoft.EntityFrameworkCore;

namespace CsvImportSiteJS.Repositories
{
	public class EmployeeRepository : IEmployeeRepository
	{
		private readonly ProjectDbContext _context;

		public EmployeeRepository(ProjectDbContext context)
		{
			_context = context;
		}
		public async Task<bool> AddRangeAsync(ICollection<Employee> employees)
		{
			_context.AddRangeAsync(employees);
			return Save();
		}

		public async Task<bool> AddAsync(Employee employee)
		{
			_context.AddAsync(employee);
			return Save();
		}

		public bool Delete(Employee employee)
		{
			_context.Remove(employee);
			return Save();
		}

		public async Task<ICollection<Employee>> GetAllEmployeesAsync()
		{
			return await _context.Employees.ToListAsync();
		}

		public async Task<Employee> GetEmployeeByPayrollNumberAsync(string id)
		{
			return await _context.Employees.FirstOrDefaultAsync(e => e.Payroll_Number == id);
		}

		public async Task<Employee> GetEmployeeByPayrollNumberAsyncNoTracking(string id)
		{
			return await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Payroll_Number == id);
		}
		public bool Save()
		{
			var result = _context.SaveChanges();
			return result > 0;
		}

		public bool Update(Employee employee)
		{
			_context.Update(employee);
			return Save();
		}

		public async Task<bool> ExistsByPayrollNumberAsync(string id)
		{
			return await _context.Employees.AnyAsync(e => e.Payroll_Number == id);
		}
	}
}
