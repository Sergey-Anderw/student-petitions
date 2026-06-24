using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using StudentPetitions.Api.Infrastructure.Mapping;

namespace StudentPetitions.Api.Tests;

public class MappingProfileTests
{
    [Fact]
    public void MappingProfile_ShouldBeValid()
    {
        var configuration = new MapperConfiguration(
            configurationExpression => configurationExpression.AddProfile<MappingProfile>(),
            NullLoggerFactory.Instance);

        configuration.AssertConfigurationIsValid();
    }
}
