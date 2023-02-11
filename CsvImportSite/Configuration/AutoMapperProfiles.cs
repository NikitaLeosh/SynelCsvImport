using AutoMapper;
using CsvImportSiteJS.Models;
using CsvImportSiteJS.ViewModels;
using Microsoft.AspNetCore.Routing.Constraints;

namespace CsvImportSiteJS.Configuration
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<Employee, ChangePayrollNumberViewModel>().
				ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EMail_Home));
		}
	}
}
