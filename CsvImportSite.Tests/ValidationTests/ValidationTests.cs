using CsvImportSiteJS.Data;
using CsvImportSiteJS.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CsvImportSiteJS.Tests.ModelTests
{
	public class ValidationTests
	{
		[Fact]
		public void Employee_TelephoneRegularExpressionAttribute_ReturnsBool()
		{
			//Arrange
			RegularExpressionAttribute attribute = new(Const.regularExpressionDigitsOnly);
			var stringValid = "+12344566";
			var stringInvalid = "ofo123fp";
			//Act
			var result = attribute.IsValid(stringValid);
			var resultInvalid = attribute.IsValid(stringInvalid);
			//Assert
			result.Should().BeTrue();
			resultInvalid.Should().BeFalse();
		}
	}
}
