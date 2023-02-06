using CsvImportSite.Models;

namespace CsvImportSite.Interfaces
{
	public interface ICsvParsingService
	{
		Task<ICollection<Employee>> ParseCsvFile(IFormFile file);
	}
}
