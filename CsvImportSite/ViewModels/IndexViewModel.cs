using CsvImportSite.Models;
using System.ComponentModel.DataAnnotations;

namespace CsvImportSite.ViewModels
{
	public class IndexViewModel
	{
		[FileExtensions(Extensions = ".csv", ErrorMessage = "Incorrect file format")]
		public IFormFile CsvFile { get; set; }
		public ICollection<Employee> Employees { get; set; }

	}
}
