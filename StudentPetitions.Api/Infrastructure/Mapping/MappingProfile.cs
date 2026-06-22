using System.Globalization;
using AutoMapper;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Models.Students;

namespace StudentPetitions.Api.Infrastructure.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateStudentRequest, Student>();

        CreateMap<Student, StudentResponse>()
            .ForMember(
                destination => destination.CreatedAt,
                options => options.MapFrom(source => source.CreatedAt.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
    }
}
