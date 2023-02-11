using System.ComponentModel.DataAnnotations;

namespace CsvImportSiteJS.ViewModels
{
	public class ChangePayrollNumberViewModel
	{
		public string Payroll_Number { get; set; }
		[MaxLength(10, ErrorMessage = "Payroll number can not be longer then 10 charcters")]
		public string NewPayroll_Number { get; set; }
		public string Forenames { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
	}
}
