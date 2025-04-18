using Dapper;
using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Npgsql;

public class TrxAuditService : ITrxAuditService
{
    private readonly NpgsqlConnection _conn;

    public TrxAuditService(IConfiguration config)
    {
        _conn = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
    }

    public async Task<(List<TrxAuditListItemViewModel> Data, int Total)> GetTrxAuditListAsync(int page, int limit)
    {
        var offset = (page - 1) * limit;

        var sql = @"
            SELECT
                a.id,
                a.audit_level AS AuditLevel,
                a.audit_type AS AuditType,
                a.audit_schedule_date AS AuditScheduleDate,
                a.status AS Status,
                s.spbu_no AS SpbuNo,
                s.latitude,
                s.longitude,
                ARRAY(
                    SELECT filepath FROM spbu_image si
                    WHERE si.spbu_id = s.id
                ) AS Images
            FROM trx_audit a
            INNER JOIN spbu s ON s.id = a.spbu_id
            ORDER BY a.created_date DESC
            LIMIT @limit OFFSET @offset;";

        var countSql = "SELECT COUNT(*) FROM trx_audit";

        var items = (await _conn.QueryAsync<TrxAuditListItemViewModel>(sql, new { limit, offset })).ToList();
        var total = await _conn.ExecuteScalarAsync<int>(countSql);

        return (items, total);
    }

    public async Task<(bool Success, string Message)> StartAuditAsync(string username, string auditId)
    {
        // Check trx_audit exists dan cocok dengan username
        var trxAuditSql = @"
        SELECT a.id, a.status, u.id AS AppUserId
        FROM trx_audit a 
        INNER JOIN app_user u ON u.id = a.app_user_id
        WHERE a.id = @auditId AND u.username = @username";

        var trxAudit = await _conn.QueryFirstOrDefaultAsync<dynamic>(trxAuditSql, new { auditId, username });
        if (trxAudit == null)
            return (false, "Audit tidak ditemukan atau bukan milik user ini");

        // Ambil master_questioner aktif hari ini
        var masterSql = @"
        SELECT id FROM master_questioner 
        WHERE current_date BETWEEN effective_start_date AND effective_end_date
        ORDER BY effective_end_date DESC
        LIMIT 1";

        var questionerId = await _conn.ExecuteScalarAsync<string?>(masterSql);

        // Jika status != NOT_STARTED, update status
        string newStatus = trxAudit.status == "NOT_STARTED" ? trxAudit.status : "IN_PROGRESS_INPUT";

        // Update trx_audit
        var updateSql = @"
        UPDATE trx_audit 
        SET status = @newStatus, 
            master_questioner_id = @questionerId, 
            updated_by = @username, 
            updated_date = CURRENT_TIMESTAMP
        WHERE id = @auditId";

        var affected = await _conn.ExecuteAsync(updateSql, new { newStatus, questionerId, username, auditId });
        return affected > 0
            ? (true, "Audit berhasil dimulai")
            : (false, "Gagal memperbarui audit");
    }
}
