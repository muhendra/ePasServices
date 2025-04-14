using ePasServices.Models;
using ePasServices.ViewModels;

public interface IAppUserService
{
    Task<AppUserLoginDto?> GetUserByUsernameAsync(string username);
    Task UpdateSuffixRefreshToken(string username, string suffix);
    Task ClearRefreshTokenAsync(string username);
    Task<SuffixRefreshTokenInfo?> GetSuffixRefreshTokenInfoAsync(string username);
    Task<ProfileViewModel?> GetUserProfileAsync(string username);
    Task<ProfileWithSpbuViewModel?> GetUserProfileWithSpbuAsync(string username);
    Task<bool> ChangePasswordAsync(string username, ChangePasswordRequest request);
    Task<bool> UpdateNotificationTokenAsync(string username, string token);

}
