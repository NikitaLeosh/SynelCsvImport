using CsvImportSite.Interfaces;
using CsvImportSite.Models;
using CsvImportSite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using AutoMapper;
using System.Text.RegularExpressions;

namespace CsvImportSite.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly ICsvParsingService _csvParser;
		private readonly IMapper _mapper;

		public HomeController(ILogger<HomeController> logger, IEmployeeRepository repository, ICsvParsingService csvParser, IMapper mapper)
		{
			_csvParser = csvParser;
			_logger = logger;
			_employeeRepository = repository;
			_mapper = mapper;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var indexViewModel = new IndexViewModel()
			{
				Employees = await _employeeRepository.GetAllEmployeesAsync()
			};
			return View(indexViewModel);
		}
		[HttpPost]
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

		public async Task<IActionResult> Edit(string id)
		{
			var employeeToEdit = await _employeeRepository.GetEmployeeByPayrollNumberAsyncNoTracking(id);
			if (employeeToEdit == null) return NotFound();
			return View(employeeToEdit);
		}

		[HttpPost]
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
		[HttpPost]
		public async Task<IActionResult> ChangePayrollNumber(ChangePayrollNumberViewModel changeNumberVM)
		{
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
			employee.Payroll_Number = changeNumberVM.NewPayroll_Number;
			try
			{
				await _employeeRepository.AddAsync(employee);
				TempData["success"] = $"Payroll number of employee {changeNumberVM.Forenames} {changeNumberVM.Surname} successfully changed";
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("AddError", "Error while adding the new numbered record");
				TempData["error"] = ex.Message;
				return View(changeNumberVM);
			}
			return RedirectToAction(nameof(Index));
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			_logger.BeginScope(typeof(HomeController));
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}