using System.Globalization;
using AutoMapper;
using StudentPetitions.Api.Entities;
using StudentPetitions.Api.Models.Petitions;
using StudentPetitions.Api.Models.Students;

namespace StudentPetitions.Api.Infrastructure.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateStudentRequest, Student>()
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(destination => destination.CreatedAt, options => options.Ignore())
            .ForMember(destination => destination.Petitions, options => options.Ignore());

        CreateMap<Student, StudentResponse>()
            .ForMember(
                destination => destination.CreatedAt,
                options => options.MapFrom(source => source.CreatedAt.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));

        CreateMap<CreatePetitionRequest, Petition>()
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(destination => destination.Student, options => options.Ignore())
            .ForMember(destination => destination.Status, options => options.Ignore())
            .ForMember(destination => destination.CreatedAt, options => options.Ignore())
            .ForMember(destination => destination.UpdatedAt, options => options.Ignore())
            .ForMember(destination => destination.ReviewedBy, options => options.Ignore())
            .ForMember(destination => destination.ReviewedAt, options => options.Ignore())
            .ForMember(destination => destination.ReviewComment, options => options.Ignore());

        CreateMap<Petition, PetitionResponse>()
            .ForMember(
                destination => destination.CreatedAt,
                options => options.MapFrom(source => source.CreatedAt.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)))
            .ForMember(
                destination => destination.UpdatedAt,
                options => options.MapFrom(source => source.UpdatedAt.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)))
            .ForMember(
                destination => destination.ReviewedAt,
                options => options.MapFrom(source => source.ReviewedAt.HasValue
                    ? source.ReviewedAt.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
                    : null));
    }
}
