using CsvImportSiteJS.Configuration;
using CsvImportSiteJS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace CsvImportSiteJS.Data
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
		public DbSet<Employee> Employees { get; set; }
	}
}
