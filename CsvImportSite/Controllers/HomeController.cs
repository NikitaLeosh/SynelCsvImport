using CsvImportSiteJS.Interfaces;
using CsvImportSiteJS.Models;
using CsvImportSiteJS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using AutoMapper;
using System.Text.RegularExpressions;

namespace CsvImportSiteJS.Controllers
{
	public class HomeController : Controller
	{
		private readonly IEmployeeRepository _employeeRepository;
		private readonly ICsvParsingService _csvParser;
		private readonly IMapper _mapper;

		public HomeController(IEmployeeRepository repository, ICsvParsingService csvParser, IMapper mapper)
		{
			_csvParser = csvParser;
			_employeeRepository = repository;
			_mapper = mapper;
		}
		//GET: /Home/Index
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var indexViewModel = new IndexViewModel()
			{
				Employees = await _employeeRepository.GetAllEmployeesAsync()
			};
			return View(indexViewModel);
		}
		//POST: Home/Index
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(IndexViewModel indexViewModel)
		{
			if (indexViewModel == null) { return NotFound(); }
			if (indexViewModel.CsvFile != null)
			{
				try
				{
					var employees = await _csvParser.ParseCsvFile(indexViewModel.CsvFile);
					if (employees != null && employees.Count > 0)
					{
						await _employeeRepository.AddRangeAsync(employees);
						TempData["success"] = $"{employees.Count} row(s) have been added to the table";
					}
					else
					{
						TempData["error"] = "No records have been added";
					}
				}
				catch (Exception ex)
				{
					if (ex is DbUpdateException && ex.ToString().Contains("80131904"))
					{
						TempData["error"] = "Your file contains records with payroll numbers that have already been added";
					}
					else
					{
						TempData["error"] = ex.Message;
					}
				}
			}
			return RedirectToAction("Index");
		}
		//GET: /Home/Edit/{Payroll number}
		public async Task<IActionResult> Edit(string id)
		{
			var employeeToEdit = await _employeeRepository.GetEmployeeByPayrollNumberAsyncNoTracking(id);
			if (employeeToEdit == null) return NotFound();
			return View(employeeToEdit);
		}
		//POST: /Home/Edit/{Payroll number}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(string id, Employee employeeToEdit)
		{
			try
			{
				_employeeRepository.Update(employeeToEdit);
				TempData["success"] = $"Employee's {employeeToEdit.Payroll_Number} record has been edited successfully";
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("UserError", ex.Message);
				TempData["error"] = ex.Message;
				return View(id);
			}
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> ChangePayrollNumber(string id)
		{
			var employee = await _employeeRepository.GetEmployeeByPayrollNumberAsync(id);
			var changePayrollVM = _mapper.Map<ChangePayrollNumberViewModel>(employee);
			return View(changePayrollVM);
		}
		//POST: Home/ChangePayrollNumber/{PayrollNumber}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePayrollNumber(ChangePayrollNumberViewModel changeNumberVM)
		{
			//Changing the key value requires deleting the record to replace it with a new one.
			//To avoid inadvertant loss of data the changed record is added to the db prior to delete the old one. 
			if (changeNumberVM.Payroll_Number == changeNumberVM.NewPayroll_Number)
			{
				ModelState.AddModelError("NewPayroll_Number", "New payroll number must differ from the old one.");
				return View(changeNumberVM);
			}
			if (await _employeeRepository.ExistsByPayrollNumberAsync(changeNumberVM.NewPayroll_Number))
			{
				ModelState.AddModelError("DBConflictError", "Record with such a number already exists");
				TempData["error"] = "This payroll number is already taken";
				return View(changeNumberVM);
			}
			var employee = await _employeeRepository.GetEmployeeByPayrollNumberAsync(changeNumberVM.Payroll_Number);
			if (employee == null)
			{
				TempData["error"] = "Couldn't find the record to change number";
				return View(changeNumberVM);
			}
			//Adding new variable with changed payroll number to avoid extra query to a database
			Employee changedEmployee = new()
			{
				Payroll_Number = changeNumberVM.NewPayroll_Number,
				Forenames = employee.Forenames,
				Surname = employee.Surname,
				Date_of_Birth = employee.Date_of_Birth,
				Telephone = employee.Telephone,
				Mobile = employee.Mobile,
				Address = employee.Address,
				Address_2 = employee.Address_2,
				Postcode = employee.Postcode,
				EMail_Home = employee.EMail_Home,
				Start_Date = employee.Start_Date
			};
			try
			{
				await _employeeRepository.AddAsync(changedEmployee);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("AddError", "Error while adding the new numbered record");
				TempData["error"] = ex.Message;
				return View(changeNumberVM);
			}
			try
			{
				_employeeRepository.Delete(employee);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("DeletionError", "Error while deleting the old numbered record");
				TempData["error"] = ex.Message;
				return View(changeNumberVM);
			}
			TempData["success"] = $"Payroll number of employee {changeNumberVM.Forenames} {changeNumberVM.Surname} successfully changed";
			return RedirectToAction(nameof(Index));
		}

		//POST: Home/Delete/{PayrollNumber}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string id)
		{
			var employee = await _employeeRepository.GetEmployeeByPayrollNumberAsync(id);
			if (employee == null)
			{
				TempData["error"] = "Employee to delete not found";
				ModelState.AddModelError("DeleteError", "Employee to delete not found");
				return RedirectToAction(nameof(Index));
			}
			try
			{
				_employeeRepository.Delete(employee);
				TempData["success"] = $"Record of employee {employee.Forenames} {employee.Surname} has been deleted";
			}
			catch (Exception ex)
			{
				TempData["error"] = $"Could not delete the employee({ex.Message})";
			}
			return RedirectToAction(nameof(Index));
		}
	}
}