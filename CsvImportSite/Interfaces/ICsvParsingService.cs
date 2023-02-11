using CsvImportSiteJS.Models;

namespace CsvImportSiteJS.Interfaces
{
	public interface ICsvParsingService
	{
		Task<ICollection<Employee>> ParseCsvFile(IFormFile file);
	}
}
