using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Domain.Enums;
using CronogramaCapilar.Domain.Exceptions;
using CronogramaCapilar.Domain.ValueObjects;

namespace CronogramaCapilar.Domain.Tests.Entities;

public sealed class UserTests
{
    private static readonly DateOnly Today = new(2026, 11, 24);

    private static User CreateUser(
        string name = "Maria Silva",
        string? preferredName = "Mari",
        DateOnly? birthDate = null,
        Gender gender = Gender.Female) =>
        User.Create(
            name,
            preferredName,
            Email.Create("maria@email.com"),
            "password-hash",
            birthDate ?? new DateOnly(1998, 5, 10),
            gender,
            Today);

    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var user = CreateUser();

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Maria Silva", user.Name);
        Assert.Equal("Mari", user.PreferredName);
        Assert.Equal("maria@email.com", user.Email.Value);
        Assert.Equal(Gender.Female, user.Gender);
        Assert.Null(user.HairProfile);
    }

    [Fact]
    public void Create_WithoutPreferredName_UsesFirstName()
    {
        var user = CreateUser(name: "  João Pedro Souza ", preferredName: " ", gender: Gender.Male);

        Assert.Equal("João Pedro Souza", user.Name);
        Assert.Equal("João", user.PreferredName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ThrowsDomainException(string name) =>
        Assert.Throws<DomainException>(() => CreateUser(name: name));

    [Fact]
    public void Create_WithNameTooLong_ThrowsDomainException() =>
        Assert.Throws<DomainException>(() => CreateUser(name: new string('a', User.NameMaxLength + 1)));

    [Fact]
    public void Create_WithFutureBirthDate_ThrowsDomainException() =>
        Assert.Throws<DomainException>(() => CreateUser(birthDate: Today.AddDays(1)));

    [Fact]
    public void Create_WithMissingBirthDate_ThrowsDomainException() =>
        Assert.Throws<DomainException>(() => CreateUser(birthDate: default(DateOnly)));

    [Fact]
    public void Create_WithMissingGender_ThrowsDomainException() =>
        Assert.Throws<DomainException>(() => CreateUser(gender: default));

    [Theory]
    [InlineData(2000, 11, 24, 26)] // aniversário hoje
    [InlineData(2000, 11, 25, 25)] // aniversário amanhã
    [InlineData(2000, 11, 23, 26)] // aniversário ontem
    public void CalculateAge_TakesBirthdayIntoAccount(int year, int month, int day, int expectedAge)
    {
        var user = CreateUser(birthDate: new DateOnly(year, month, day));

        Assert.Equal(expectedAge, user.CalculateAge(Today));
    }

    [Fact]
    public void UpdatePersonalInfo_WithInvalidData_ChangesNothing()
    {
        var user = CreateUser();

        Assert.Throws<DomainException>(() =>
            user.UpdatePersonalInfo("Other Name", "Other", Today.AddDays(1), Gender.Male, Today));

        Assert.Equal("Maria Silva", user.Name);
        Assert.Equal("Mari", user.PreferredName);
        Assert.Equal(Gender.Female, user.Gender);
    }

    [Fact]
    public void SetHairProfile_StoresTheProfile()
    {
        var user = CreateUser();
        var hairProfile = HairProfile.Create(HairType.Curly, HairLength.Long, 3, [TreatmentType.Hydration]);

        user.SetHairProfile(hairProfile);

        Assert.Equal(hairProfile, user.HairProfile);
    }
}
