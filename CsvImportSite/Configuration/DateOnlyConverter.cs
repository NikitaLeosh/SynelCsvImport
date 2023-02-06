using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CsvImportSite.Configuration
{
	public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
	{
		public DateOnlyConverter() : base(
				  d => d.ToDateTime(TimeOnly.MinValue),
				  d => DateOnly.FromDateTime(d))
		{
		}
	}
}
