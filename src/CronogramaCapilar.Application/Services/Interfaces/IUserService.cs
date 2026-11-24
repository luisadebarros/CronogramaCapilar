using CronogramaCapilar.Application.Common;
using CronogramaCapilar.Application.DTOs.Users;

namespace CronogramaCapilar.Application.Services.Interfaces;

public interface IUserService
{
    Task<Result<UserResponse>> GetAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> UpdatePersonalInfoAsync(
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<HairProfileResponse>> SetHairProfileAsync(
        Guid userId,
        HairProfileRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAccountAsync(Guid userId, CancellationToken cancellationToken = default);
}
