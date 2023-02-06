using CsvImportSite.Configuration;
using CsvImportSite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace CsvImportSite.Data
{
	public class ProjectDbContext : DbContext
	{
		public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
		{
			//for docker-compose usage ensure existance of container db
			try
			{
				var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
				if (databaseCreator != null)
				{
					if (!databaseCreator.CanConnect()) databaseCreator.Create();
					if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
		//EF core can not add DateOnly fields to the SQL server database. That is why
		//Converter is used to transfer valid data to the database
		protected override void ConfigureConventions(ModelConfigurationBuilder builder)
		{
			builder.Properties<DateOnly>()
				.HaveConversion<DateOnlyConverter>()
				.HaveColumnType("date");
		}
		public DbSet<Employee> Employees { get; set; }
	}
}
