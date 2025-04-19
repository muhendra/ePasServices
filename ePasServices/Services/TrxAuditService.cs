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

        // mapping dengan splitOn berdasarkan kolom pertama dari object kedua
        var items = (await _conn.QueryAsync<TrxAuditListItemViewModel, SpbuViewModel, TrxAuditListItemViewModel>(
            sql,
            (audit, spbu) =>
            {
                audit.Spbu = spbu;
                return audit;
            },
            new { limit, offset },
            splitOn: "SpbuNo"
        )).ToList();

        var total = await _conn.ExecuteScalarAsync<int>(countSql);

        return (items, total);
    }

    public async Task<(bool Success, string Message)> StartAuditAsync(string username, string auditId)
    {
        var trxAuditSql = @"
        SELECT a.id, a.status, a.app_user_id 
        FROM trx_audit a 
        INNER JOIN app_user u ON u.id = a.app_user_id
        WHERE a.id = @auditId AND u.username = @username";

        var trxAudit = await _conn.QueryFirstOrDefaultAsync<dynamic>(trxAuditSql, new { auditId, username });
        if (trxAudit == null)
            return (false, "Audit tidak ditemukan atau bukan milik user ini");

        string newStatus = "IN_PROGRESS_INPUT";

        var masterSql = @"
        SELECT id, category FROM master_questioner 
        WHERE current_date BETWEEN effective_start_date AND effective_end_date
        ORDER BY effective_end_date DESC";

        var questioners = (await _conn.QueryAsync<dynamic>(masterSql)).ToList();

        var introSql = @"
        SELECT mq.id 
        FROM trx_audit ta  
        LEFT JOIN master_questioner mq ON ta.master_questioner_intro_id = mq.id 
        WHERE mq.category = 'INTRO' 
        ORDER BY mq.version DESC 
        LIMIT 1";
        
        string? introId = await _conn.ExecuteScalarAsync<string?>(introSql);
        
        var checklistSql = @"
        SELECT mq.id 
        FROM trx_audit ta  
        LEFT JOIN master_questioner mq ON ta.master_questioner_checklist_id = mq.id 
        WHERE mq.category = 'CHECKLIST' 
        ORDER BY mq.version DESC 
        LIMIT 1";

        string? checklistId = await _conn.ExecuteScalarAsync<string?>(checklistSql);


        var countSql = "SELECT COUNT(1) FROM trx_audit where report_prefix = 'CB/AI/'";
        var totalAuditCount = await _conn.ExecuteScalarAsync<int>(countSql);
        var reportPrefix = "CB/AI/";
        var reportNo = reportPrefix + (totalAuditCount + 1).ToString("D4");
        var audit_execution_time = DateTime.Now;

        var updateSql = @"
        UPDATE trx_audit 
        SET status = @newStatus,
            master_questioner_intro_id = @introId,
            master_questioner_checklist_id = @checklistId,
            updated_by = @username,
            updated_date = CURRENT_TIMESTAMP,
            audit_execution_time = CURRENT_TIMESTAMP,
            report_prefix = @reportPrefix,
            report_no = @reportNo
        WHERE id = @auditId";

        var affected = await _conn.ExecuteAsync(updateSql, new
        {
            newStatus,
            introId,
            checklistId,
            username,
            auditId,
            reportPrefix,
            reportNo,
            audit_execution_time
        });

        return affected > 0
            ? (true, "Audit berhasil dimulai")
            : (false, "Gagal memperbarui audit");
    }
}
