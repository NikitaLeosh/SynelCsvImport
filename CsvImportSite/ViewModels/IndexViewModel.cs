using CsvImportSiteJS.Models;
using System.ComponentModel.DataAnnotations;

namespace CsvImportSiteJS.ViewModels
{
	public class IndexViewModel
	{
		[FileExtensions(Extensions = ".csv", ErrorMessage = "Incorrect file format")]
		public IFormFile CsvFile { get; set; }
		public ICollection<Employee> Employees { get; set; }

	}
}
