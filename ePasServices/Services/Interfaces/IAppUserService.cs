using ePasServices.Models;
using ePasServices.ViewModels;

public interface IAppUserService
{
    Task<AppUserLoginDto?> GetUserByUsernameAsync(string username);
    Task UpdateSuffixRefreshToken(string username, string suffix);
    Task ClearRefreshTokenAsync(string username);
    Task<SuffixRefreshTokenInfo?> GetSuffixRefreshTokenInfoAsync(string username);
}
