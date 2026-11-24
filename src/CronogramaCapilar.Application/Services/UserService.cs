using CronogramaCapilar.Application.Common;
using CronogramaCapilar.Application.DTOs.Users;
using CronogramaCapilar.Application.Errors;
using CronogramaCapilar.Application.Mappings;
using CronogramaCapilar.Application.Services.Interfaces;
using CronogramaCapilar.Domain.Repositories.Interfaces;
using CronogramaCapilar.Domain.ValueObjects;

namespace CronogramaCapilar.Application.Services;

public sealed class UserService(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IUserService
{
    public async Task<Result<UserResponse>> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return UserErrors.NotFound;

        return user.ToResponse(Today());
    }

    public async Task<Result<UserResponse>> UpdatePersonalInfoAsync(
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return UserErrors.NotFound;

        user.UpdatePersonalInfo(
            request.Name,
            request.PreferredName,
            request.BirthDate.GetValueOrDefault(),
            request.Gender.GetValueOrDefault(),
            Today());

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.ToResponse(Today());
    }

    public async Task<Result<HairProfileResponse>> SetHairProfileAsync(
        Guid userId,
        HairProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return UserErrors.NotFound;

        var hairProfile = HairProfile.Create(
            request.HairType.GetValueOrDefault(),
            request.Length.GetValueOrDefault(),
            request.WashesPerWeek.GetValueOrDefault(),
            request.Treatments);

        user.SetHairProfile(hairProfile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return hairProfile.ToResponse();
    }

    /// <summary>Apaga a conta e tudo ligado a ela (exigência da Apple e direito previsto na LGPD).</summary>
    public async Task<Result> DeleteAccountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return UserErrors.NotFound;

        users.Remove(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private DateOnly Today() => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
}
