using CsvImportSite.Data;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CsvImportSite.Models
{
	public class Employee
	{
		[Key]
		[MaxLength(10, ErrorMessage = "Payroll number can not be longer then 10 characters")]
		public string Payroll_Number { get; set; }
		[MaxLength(30, ErrorMessage = "First name must not exceed 30 characters")]
		public string Forenames { get; set; }
		[MaxLength(30, ErrorMessage = "Last name must not exceed 30 characters")]
		public string Surname { get; set; }

		public DateTime Date_of_Birth { get; set; }
		[RegularExpression(Const.regularExpressionDigitsOnly, ErrorMessage = "Invalid number (digits only, can start with '+')")]
		public string Telephone { get; set; }
		[RegularExpression(Const.regularExpressionDigitsOnly, ErrorMessage = "Invalid number (digits only, can start with '+')")]
		public string? Mobile { get; set; }
		public string Address { get; set; }
		public string? Address_2 { get; set; }
		[MaxLength(10, ErrorMessage = "Post code can not be longer then 10 characters")]
		public string Postcode { get; set; }
		[MaxLength(30, ErrorMessage = "Last name must not exceed 30 characters")]
		public string EMail_Home { get; set; }
		public DateTime Start_Date { get; set; }
	}
}
