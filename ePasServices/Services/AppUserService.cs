using ePasServices.Models;
using Dapper;
using Npgsql;
using ePasServices.ViewModels;

public class AppUserService : IAppUserService
{
    private readonly NpgsqlConnection _conn;

    public AppUserService(IConfiguration config)
    {
        _conn = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
    }

    public async Task<AppUserLoginDto?> GetUserByUsernameAsync(string username)
    {
        var sql = @"
            SELECT au.username, au.password_hash, ar.app 
            FROM app_user au 
            INNER JOIN app_user_role aur ON aur.app_user_id = au.id 
            INNER JOIN app_role ar ON ar.id = aur.app_role_id 
            WHERE au.username = @username AND au.status = 'ACTIVE' 
            AND ar.app IN ('Auditor','SPBU')";

        return await _conn.QueryFirstOrDefaultAsync<AppUserLoginDto>(sql, new { username });
    }

    public async Task UpdateSuffixRefreshToken(string username, string suffix)
    {
        var sql = @"
            UPDATE app_user 
            SET suffix_refresh_token = @suffix, 
                updated_by = @username, 
                updated_date = CURRENT_TIMESTAMP 
            WHERE username = @username";

        await _conn.ExecuteAsync(sql, new { suffix, username });
    }

    public async Task ClearRefreshTokenAsync(string username)
    {
        var sql = @"
        UPDATE app_user 
        SET suffix_refresh_token = NULL,
            updated_by = @username,
            updated_date = CURRENT_TIMESTAMP
        WHERE username = @username";

        await _conn.ExecuteAsync(sql, new { username });
    }

    public async Task<SuffixRefreshTokenInfo?> GetSuffixRefreshTokenInfoAsync(string username)
    {
        var sql = @"
        SELECT 
            COALESCE(au.suffix_refresh_token, '') AS SuffixRefreshToken,
            ar.app AS App
        FROM app_user au
        INNER JOIN app_user_role aur ON aur.app_user_id = au.id
        INNER JOIN app_role ar ON ar.id = aur.app_role_id
        WHERE au.username = @username AND au.status = 'ACTIVE' AND ar.app IN ('Auditor','SPBU')";

        return await _conn.QueryFirstOrDefaultAsync<SuffixRefreshTokenInfo>(sql, new { username });
    }

    public async Task<ProfileViewModel?> GetUserProfileAsync(string username)
    {
        var sql = @"
        SELECT 
            au.username,
            au.name,
            au.email,
            au.phone_number AS PhoneNumber,
            ar.app AS App
        FROM app_user au
        INNER JOIN app_user_role aur ON aur.app_user_id = au.id
        INNER JOIN app_role ar ON ar.id = aur.app_role_id
        WHERE au.username = @username AND au.status = 'ACTIVE'
        LIMIT 1";

        return await _conn.QueryFirstOrDefaultAsync<ProfileViewModel>(sql, new { username });
    }

    public async Task<ProfileWithSpbuViewModel?> GetUserProfileWithSpbuAsync(string username)
    {
        var sql = @"
        SELECT 
            au.Name,
            ar.app AS App,
            s.spbu_no as SpbuNo,
            s.province_name as ProvinceName,
            s.city_name as CityName,
            s.""type"",
            s.""level"" 
        FROM app_user au
        INNER JOIN app_user_role aur ON aur.app_user_id = au.id
        INNER JOIN app_role ar ON ar.id = aur.app_role_id
        LEFT JOIN spbu s ON s.id = aur.spbu_id
        WHERE au.username = @username AND au.status = 'ACTIVE'
        LIMIT 1";

        var result = await _conn.QueryFirstOrDefaultAsync<ProfileWithSpbuTempDto>(sql, new { username });

        if (result == null) return null;

        return new ProfileWithSpbuViewModel
        {
            Name = result.Name,
            Spbu = result.App == "SPBU"
                ? new SpbuInfo
                {
                    SpbuNo = result.SpbuNo,
                    ProvinceName = result.ProvinceName,
                    CityName = result.CityName,
                    Type = result.Type,
                    Level = result.Level
                }
                : null
        };
    }

    public async Task<bool> ChangePasswordAsync(string username, ChangePasswordRequest request)
    {
        var sqlGet = "SELECT password_hash FROM app_user WHERE username = @username AND status = 'ACTIVE'";
        var currentHash = await _conn.QueryFirstOrDefaultAsync<string>(sqlGet, new { username });

        if (string.IsNullOrEmpty(currentHash) || !BCrypt.Net.BCrypt.Verify(request.OldPassword, currentHash))
            return false;

        var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        var sqlUpdate = @"
        UPDATE app_user
        SET password_hash = @newHash,
            last_change_passwd_dt = CURRENT_TIMESTAMP,
            updated_by = @username,
            updated_date = CURRENT_TIMESTAMP
        WHERE username = @username";

        var affected = await _conn.ExecuteAsync(sqlUpdate, new { newHash, username });

        return affected > 0;
    }

    public async Task<bool> UpdateNotificationTokenAsync(string username, string token)
    {
        var sql = @"
        UPDATE app_user
        SET notification_token = @token,
            updated_by = @username,
            updated_date = CURRENT_TIMESTAMP
        WHERE username = @username";

        var affected = await _conn.ExecuteAsync(sql, new { token, username });
        return affected > 0;
    }

    public async Task<string?> GetAppUserIdByUsernameAsync(string username)
    {
        var sql = "SELECT id FROM app_user WHERE username = @username AND status = 'ACTIVE'";
        return await _conn.QueryFirstOrDefaultAsync<string?>(sql, new { username });
    }

    public async Task<AppUser?> GetUserByEmailAsync(string email)
    {
        var sql = "SELECT * FROM app_user WHERE email = @email AND status = 'ACTIVE'";
        return await _conn.QueryFirstOrDefaultAsync<AppUser>(sql, new { email });
    }

    public async Task UpdateSuffixRefreshTokenAsync(string username, string suffix)
    {
        var sql = @"
        UPDATE app_user 
        SET suffix_refresh_token = @suffix,
            updated_date = CURRENT_TIMESTAMP
        WHERE username = @username AND status = 'ACTIVE';
    ";

        await _conn.ExecuteAsync(sql, new { username, suffix });
    }

}
