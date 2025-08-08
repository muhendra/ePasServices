using Dapper;
using ePasServices.Data;
using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class NotificationService : INotificationService
{
    private readonly NpgsqlConnection _conn;
    private readonly PostgreDbContext _context;

    public NotificationService(IConfiguration config, PostgreDbContext context)
    {
        _conn = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        _context = context;
    }

    public async Task<(List<NotificationListItemViewModel> Data, int Total)> GetNotificationListAsync(
        int page,
        int limit,
        string userId
    )
    {
        var offset = (page - 1) * limit;

        string sql = @"
            SELECT
                n.id,
                n.title,
                n.message,
                n.status,
                n.created_date AS CreatedDate
            FROM notification n
            WHERE n.app_user_id = @userId
            ORDER BY n.created_date ASC
            LIMIT @limit OFFSET @offset;";

        string countSql = @"
            SELECT COUNT(*)
            FROM notification n
            WHERE n.app_user_id = @userId;";

        var items = (await _conn.QueryAsync<NotificationListItemViewModel>(
            sql,
            new { userId, limit, offset }
        )).ToList();

        var total = await _conn.ExecuteScalarAsync<int>(countSql, new { userId });

        return (items, total);
    }

    public async Task<bool> UpdateNotificationStatusAsync(string id, string status, string userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.AppUserId == userId);

        if (notification == null) return false;

        notification.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }
}
