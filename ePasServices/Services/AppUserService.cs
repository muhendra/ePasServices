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

}
