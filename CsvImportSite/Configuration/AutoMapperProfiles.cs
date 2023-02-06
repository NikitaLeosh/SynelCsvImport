using AutoMapper;
using CsvImportSite.Models;
using CsvImportSite.ViewModels;
using Microsoft.AspNetCore.Routing.Constraints;

namespace CsvImportSite.Configuration
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
