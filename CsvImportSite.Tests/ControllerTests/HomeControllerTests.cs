using AutoMapper;
using Castle.Core.Logging;
using CsvImportSiteJS.Controllers;
using CsvImportSiteJS.Interfaces;
using CsvImportSiteJS.Models;
using CsvImportSiteJS.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CsvImportSiteJS.Tests.ControllerTests
{
	public class HomeControllerTests
	{
		private readonly IEmployeeRepository _employeeRepository;
		private readonly ICsvParsingService _csvParser;
		private readonly ITempDataDictionary _tempData;
		private readonly IMapper _mapper;

		public HomeControllerTests()
		{
			_employeeRepository = A.Fake<IEmployeeRepository>();
			_csvParser = A.Fake<ICsvParsingService>();
			_tempData = A.Fake<ITempDataDictionary>();
			_mapper = A.Fake<IMapper>();
		}

		[Fact]
		public async void HomeController_IndexGet_ReturnsView()
		{
			//Arrange
			var indexViewModel = A.Fake<IndexViewModel>();
			var employees = A.CollectionOfFake<Employee>(2);
			A.CallTo(() => _employeeRepository.GetAllEmployeesAsync()).Returns(employees);
			var controller = new HomeController(_employeeRepository, _csvParser, _mapper);
			//Act
			var result = await controller.Index();
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ViewResult));
		}
		[Fact]
		public async void HomeController_IndexPost_ReturnsRedirectTOActionResult()
		{
			//Arrange
			IndexViewModel indexVM = new()
			{
				CsvFile = A.Fake<IFormFile>(),
				Employees = A.CollectionOfFake<Employee>(2)
			};
			IndexViewModel indexVMEmptyResponse = new()
			{
				CsvFile = A.Fake<IFormFile>()
			};
			IndexViewModel indexVMTakenRows = new()
			{
				CsvFile = A.Fake<IFormFile>()
			};
			var employees = A.CollectionOfFake<Employee>(2);
			var employeesEmpty = A.CollectionOfFake<Employee>(0);
			var employeesTaken = A.CollectionOfFake<Employee>(1);
			A.CallTo(() => _csvParser.ParseCsvFile(indexVM.CsvFile)).Returns(employees);
			A.CallTo(() => _csvParser.ParseCsvFile(indexVMEmptyResponse.CsvFile)).Returns(employeesEmpty);
			A.CallTo(() => _csvParser.ParseCsvFile(indexVMTakenRows.CsvFile)).Returns(employeesTaken);
			A.CallTo(() => _employeeRepository.AddRangeAsync(employees)).Returns(true);
			A.CallTo(() => _employeeRepository.AddRangeAsync(employeesTaken)).Throws(new DbUpdateException());
			var controller = new HomeController(_employeeRepository, _csvParser, _mapper);
			controller.TempData = _tempData;
			//Act
			var resultNormal = await controller.Index(indexVM);
			var resultEmptyResponse = await controller.Index(indexVMEmptyResponse);
			var resultTakenRows = await controller.Index(indexVMTakenRows);
			//Assert
			resultNormal.Should().NotBeNull();
			resultNormal.Should().BeOfType(typeof(RedirectToActionResult));
			resultEmptyResponse.Should().NotBeNull();
			resultEmptyResponse.Should().BeOfType(typeof(RedirectToActionResult));
			resultTakenRows.Should().NotBeNull();
			resultTakenRows.Should().BeOfType(typeof(RedirectToActionResult));
		}
		[Fact]
		public async void HomeController_EditGet_ReturnsView()
		{
			//Arrange
			var idNormal = "a";
			var idEmpty = "b";
			var employeeToEdit = A.Fake<Employee>();
			Employee empNull = null;
			A.CallTo(() => _employeeRepository.GetEmployeeByPayrollNumberAsyncNoTracking(idNormal)).Returns(employeeToEdit);
			A.CallTo(() => _employeeRepository.GetEmployeeByPayrollNumberAsyncNoTracking(idEmpty)).Returns(empNull);
			var controller = new HomeController(_employeeRepository, _csvParser, _mapper);
			controller.TempData = _tempData;
			//Act
			var resultNormal = await controller.Edit(idNormal);
			var resultNotFound = await controller.Edit(idEmpty);
			//Assert
			resultNormal.Should().NotBeNull();
			resultNormal.Should().BeOfType(typeof(ViewResult));
			resultNotFound.Should().NotBeNull();
			resultNotFound.Should().BeOfType(typeof(NotFoundResult));
		}
		[Fact]
		public async void HomeController_EditPost_ReturnsRedirectToAction()
		{
			//Arrange
			var employeeToEditOk = A.Fake<Employee>();
			var employeeToEditError = A.Fake<Employee>();
			A.CallTo(() => _employeeRepository.Update(employeeToEditOk)).Returns(true);
			A.CallTo(() => _employeeRepository.Update(employeeToEditError)).Throws(new Exception("This is exception"));
			var controller = new HomeController(_employeeRepository, _csvParser, _mapper);
			controller.TempData = _tempData;
			//Act
			var resultNormal = controller.Edit(A.Dummy<string>(), employeeToEditOk);
			var resultError = (ViewResult)(controller.Edit(A.Dummy<string>(), employeeToEditError));
			//Assert
			resultNormal.Should().NotBeNull();
			resultNormal.Should().BeOfType(typeof(RedirectToActionResult));
			resultError.Should().NotBeNull();
			resultError.Should().BeOfType(typeof(ViewResult));
			resultError.ViewData.ModelState.Should().ContainKey("UserError");
		}
		[Fact]
		public async void HomeController_ChangePayrollNumber_ReturnsRedirectToAction()
		{
			//Arrange
			var VMNormal = A.Fake<ChangePayrollNumberViewModel>();
			VMNormal.Payroll_Number = "a";
			var VMSameNumber = A.Fake<ChangePayrollNumberViewModel>();
			VMSameNumber.Payroll_Number = "ABCACB";
			VMSameNumber.NewPayroll_Number = VMSameNumber.Payroll_Number;
			var VMTaken = A.Fake<ChangePayrollNumberViewModel>();
			VMTaken.Payroll_Number = "b";
			VMTaken.NewPayroll_Number = "c";
			A.CallTo(() => _employeeRepository.ExistsByPayrollNumberAsync(VMNormal.NewPayroll_Number)).Returns(false);
			A.CallTo(() => _employeeRepository.ExistsByPayrollNumberAsync(VMTaken.NewPayroll_Number)).Returns(true);
			var controller = new HomeController(_employeeRepository, _csvParser, _mapper);
			controller.TempData = _tempData;
			//Act
			var resultNormal = await controller.ChangePayrollNumber(VMNormal);
			var resultSameNumber = (ViewResult)(await controller.ChangePayrollNumber(VMSameNumber));
			var resultTakenNumber = (ViewResult)(await controller.ChangePayrollNumber(VMTaken));
			//Assert
			resultNormal.Should().NotBeNull();
			resultNormal.Should().BeOfType(typeof(RedirectToActionResult));
			resultSameNumber.Should().NotBeNull();
			resultSameNumber.ViewData.ModelState.Should().ContainKey("NewPayroll_Number");
			resultTakenNumber.Should().NotBeNull();
			resultTakenNumber.ViewData.ModelState.Should().ContainKey("DBConflictError");
		}
		[Fact]
		public async void EmployeeController_Delete_ReturnsReirectToAction()
		{
			//Arrange
			var payrollNumberOk = "1";
			var employeeOk = A.Fake<Employee>();
			A.CallTo(() => _employeeRepository.GetEmployeeByPayrollNumberAsync(payrollNumberOk)).Returns(employeeOk);
			A.CallTo(() => _employeeRepository.Delete(employeeOk)).Returns(true);
			var controller = new HomeController(_employeeRepository, _csvParser, _mapper);
			controller.TempData = _tempData;
			//Act
			var resultOk = (RedirectToActionResult)await controller.Delete(payrollNumberOk);
			//Arrange
			resultOk.Should().NotBeNull();
			resultOk.Should().BeOfType(typeof(RedirectToActionResult));
		}
	}
}