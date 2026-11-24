using CronogramaCapilar.Domain.Enums;
using CronogramaCapilar.Domain.Exceptions;
using CronogramaCapilar.Domain.ValueObjects;

namespace CronogramaCapilar.Domain.Tests.ValueObjects;

public sealed class HairProfileTests
{
    [Fact]
    public void Create_RemovesDuplicateTreatmentsAndSorts()
    {
        var hairProfile = HairProfile.Create(
            HairType.Coily,
            HairLength.Medium,
            2,
            [TreatmentType.Nutrition, TreatmentType.Hydration, TreatmentType.Nutrition]);

        Assert.Equal(new[] { TreatmentType.Hydration, TreatmentType.Nutrition }, hairProfile.Treatments);
    }

    [Fact]
    public void Create_WithoutTreatments_HasEmptyList()
    {
        var hairProfile = HairProfile.Create(HairType.Straight, HairLength.VeryShort, 7, null);

        Assert.Empty(hairProfile.Treatments);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(8)]
    public void Create_WithWashesOutOfRange_ThrowsDomainException(int washesPerWeek) =>
        Assert.Throws<DomainException>(() =>
            HairProfile.Create(HairType.Wavy, HairLength.Short, washesPerWeek, null));

    [Fact]
    public void Create_WithMissingHairType_ThrowsDomainException() =>
        Assert.Throws<DomainException>(() =>
            HairProfile.Create(default, HairLength.Short, 3, null));

    [Fact]
    public void Create_WithInvalidTreatment_ThrowsDomainException() =>
        Assert.Throws<DomainException>(() =>
            HairProfile.Create(HairType.Curly, HairLength.Long, 3, [(TreatmentType)99]));

    [Fact]
    public void TwoProfilesWithSameData_AreEqual()
    {
        var first = HairProfile.Create(HairType.Curly, HairLength.Long, 3, [TreatmentType.Nutrition, TreatmentType.Hydration]);
        var second = HairProfile.Create(HairType.Curly, HairLength.Long, 3, [TreatmentType.Hydration, TreatmentType.Nutrition]);

        Assert.Equal(first, second);
    }
}
