using CsvImportSite.Models;

namespace CsvImportSite.Interfaces
{
	public interface IEmployeeRepository
	{
		Task<Employee> GetEmployeeByPayrollNumberAsync(string id);
		Task<Employee> GetEmployeeByPayrollNumberAsyncNoTracking(string id);
		Task<ICollection<Employee>> GetAllEmployeesAsync();
		Task<bool> AddAsync(Employee employee);
		Task<bool> AddRangeAsync(ICollection<Employee> employees);
		Task<bool> ExistsByPayrollNumberAsync(string id);
		bool Update(Employee employee);
		bool Delete(Employee employee);
		bool Save();
	}
}
