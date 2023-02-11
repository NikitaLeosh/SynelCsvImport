using CsvImportSiteJS.Data;
using CsvImportSiteJS.Models;
using CsvImportSiteJS.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CsvImportSiteJS.Tests.RepositoriesTests
{
	public class EmployeeRepositoryTests
	{
		private ProjectDbContext GetDatabaseContext()
		{
			var options = new DbContextOptionsBuilder<ProjectDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var databaseContext = new ProjectDbContext(options);
			databaseContext.Database.EnsureCreated();
			if (databaseContext.Employees.Count() <= 0)
			{
				for (int i = 0; i < 5; i++)
				{
					databaseContext.Employees.Add(
					new Models.Employee()
					{
						Payroll_Number = $"EMP{i + 1}",
						Forenames = "IVAN",
						Surname = "IVANOVIC",
						Date_of_Birth = new DateTime(1990, 5, (i + 1)),
						Telephone = "+78139329422",
						Mobile = "239879873",
						Address = $"City street {i}",
						Address_2 = "Another city",
						Postcode = $"DF13{i}",
						EMail_Home = $"email_{i}@mail.com",
						Start_Date = new DateTime(2000, 5, (i + 3))
					});
				}
				databaseContext.SaveChanges();
			}
			return databaseContext;
		}
		private readonly ProjectDbContext _dbContext;
		private readonly EmployeeRepository _employeeRepository;
		public EmployeeRepositoryTests()
		{
			_dbContext = GetDatabaseContext();
			_employeeRepository = new EmployeeRepository(_dbContext);
		}


		[Fact]
		public async void EmployeeRepository_GetAllEmployeesAsync_ReturnsICollection()
		{
			//Arrange
			//Act
			var result = await _employeeRepository.GetAllEmployeesAsync();
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(List<Employee>));
		}
		[Fact]
		public async void EmployeeRepository_GetEmployeeByPayrollNumberAsync_ReturnsEmployee()
		{
			//Arrange
			var payrollNumberPresent = "EMP3";
			var payrollNumberAbsent = "asd";
			//Act
			var resultPresent = await _employeeRepository.GetEmployeeByPayrollNumberAsync(payrollNumberPresent);
			var resultAbsent = await _employeeRepository.GetEmployeeByPayrollNumberAsync(payrollNumberAbsent);
			//Assert
			resultPresent.Should().NotBeNull();
			resultPresent.Should().BeOfType(typeof(Employee));
			resultAbsent.Should().BeNull();
		}
		[Fact]
		public async void EmployeeRepository_GetEmployeeByPayrollNumberAsyncNoTracking_ReturnsEmployee()
		{
			//Arrange
			var payrollNumberPresent = "EMP3";
			var payrollNumberAbsent = "asd";
			//Act
			var resultPresent = await _employeeRepository.GetEmployeeByPayrollNumberAsyncNoTracking(payrollNumberPresent);
			var resultAbsent = await _employeeRepository.GetEmployeeByPayrollNumberAsyncNoTracking(payrollNumberAbsent);
			//Assert
			resultPresent.Should().NotBeNull();
			resultPresent.Should().BeOfType(typeof(Employee));
			resultAbsent.Should().BeNull();
		}
		[Fact]
		public async void EmployeeRepository_ExistsByPayrollNumber_ReturnsBool()
		{
			//Arrange
			var numberTrue = "EMP3";
			var numberFalse = "bla";
			//Act
			var resultTrue = await _employeeRepository.ExistsByPayrollNumberAsync(numberTrue);
			var resultFalse = await _employeeRepository.ExistsByPayrollNumberAsync(numberFalse);
			//Assert
			resultTrue.Should().BeTrue();
			resultFalse.Should().BeFalse();

		}
	}
}
