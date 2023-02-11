using CsvHelper.TypeConversion;
using CsvHelper;
using CsvImportSiteJS.Configuration;
using CsvImportSiteJS.Data;
using CsvImportSiteJS.Interfaces;
using CsvImportSiteJS.Models;
using CsvImportSiteJS.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CsvImportSiteJS.Services
{
	public class CsvParsingService : ICsvParsingService
	{
		/// <summary>
		/// returns ICollection of Employees parsed from .csv file (using CsvHelper)
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		/// <exception cref="CsvHelperException"></exception>
		public async Task<ICollection<Employee>> ParseCsvFile(IFormFile file)
		{
			string filePath = Path.GetFullPath(file.FileName.ToString());
			using (var stream = File.Create(filePath))
			{
				await file.CopyToAsync(stream);
			}
			using (var reader = new StreamReader(filePath))
			{
				using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				{
					var options = new TypeConverterOptions { Formats = new[] { "dd/MM/yyyy", "dd/M/yyyy" } };
					csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
					csv.Context.RegisterClassMap<CsvEmployeeMap>();
					ICollection<Employee> employees;
					try
					{
						employees = csv.GetRecords<Employee>().ToList();
					}
					catch (Exception ex)
					{
						throw new CsvHelperException(csv.Context, $"Error during the parsing process({ex.Message})");
					}
					return employees;
				}
			}
		}
	}
}
