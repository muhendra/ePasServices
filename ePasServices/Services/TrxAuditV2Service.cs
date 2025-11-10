using Dapper;
using ePasServices.Data;
using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class TrxAuditV2Service : ITrxAuditV2Service
{
    private readonly NpgsqlConnection _conn;
    private readonly PostgreDbContext _context;

    public TrxAuditV2Service(IConfiguration config, PostgreDbContext context)
    {
        _conn = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        _context = context;
    }

    public async Task<(List<TrxAuditListItemViewV2Model> Data, int Total)> GetTrxAuditListAsync(int page, int limit, bool history, string username)
    {
        var offset = (page - 1) * limit;

        string sql;
        string countSql;

        if (history)
        {
            sql = @"
            SELECT
                ta.id,
                ta.audit_level AS AuditLevel,
                ta.audit_type AS AuditType,
                ta.audit_schedule_date AS AuditScheduleDate,
                CASE
                    WHEN au.username = @username THEN COALESCE(ta.form_status_auditor1, ta.status)
                    WHEN au2.username = @username THEN ta.form_status_auditor2
                    ELSE ta.status
                END AS Status,
                CASE
                    WHEN au.username = @username THEN ta.form_type_auditor1
                    WHEN au2.username = @username THEN ta.form_type_auditor2
                    ELSE NULL
                END AS FormType,
                s.spbu_no AS SpbuNo,
                s.address,
                s.latitude,
                s.longitude,
                ARRAY(
                    SELECT filepath FROM spbu_image si
                    WHERE si.spbu_id = s.id
                ) AS Images,
                CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM trx_feedback tf
                        inner join trx_audit txa on txa.id = tf.trx_audit_id
                        WHERE txa.spbu_id = ta.spbu_id and tf.feedback_type = 'BANDING' and tf.status not in ('APPROVE','REJECT','CLOSED')
                    ) THEN TRUE
                    ELSE FALSE
                END AS HasProgressBanding
            FROM trx_audit ta
            INNER JOIN spbu s ON s.id = ta.spbu_id
            INNER JOIN app_user au ON ta.app_user_id = au.id
            LEFT JOIN app_user au2 ON ta.app_user_id_auditor2 = au2.id
            WHERE 
            CASE
                WHEN au.username = @username THEN ta.form_status_auditor1
                WHEN au2.username = @username THEN ta.form_status_auditor2
                ELSE ta.status
            END IN ('UNDER_REVIEW', 'VERIFIED')
            AND (@username IN (au.username, au2.username))
            ORDER BY ta.audit_schedule_date ASC
            LIMIT @limit OFFSET @offset;";

            countSql = @"
            SELECT COUNT(*)
            FROM trx_audit ta
            INNER JOIN app_user au ON ta.app_user_id = au.id
            LEFT JOIN app_user au2 ON ta.app_user_id_auditor2 = au2.id
            WHERE 
            CASE
                WHEN au.username = @username THEN ta.form_status_auditor1
                WHEN au2.username = @username THEN ta.form_status_auditor2
                ELSE ta.status
            END IN ('UNDER_REVIEW', 'VERIFIED')
            AND (@username IN (au.username, au2.username));";
        }
        else
        {
            sql = @"
            SELECT
                ta.id,
                ta.audit_level AS AuditLevel,
                ta.audit_type AS AuditType,
                ta.audit_schedule_date AS AuditScheduleDate,
                CASE
                    WHEN au.username = @username THEN COALESCE(ta.form_status_auditor1, ta.status)
                    WHEN au2.username = @username THEN ta.form_status_auditor2
                    ELSE ta.status
                END AS Status,
                CASE
                    WHEN au.username = @username THEN ta.form_type_auditor1
                    WHEN au2.username = @username THEN ta.form_type_auditor2
                    ELSE NULL
                END AS FormType,
                s.spbu_no AS SpbuNo,
                s.Address,
                s.latitude,
                s.longitude,
                ARRAY(
                    SELECT filepath FROM spbu_image si
                    WHERE si.spbu_id = s.id
                ) AS Images,
                CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM trx_feedback tf
                        inner join trx_audit txa on txa.id = tf.trx_audit_id
                        WHERE txa.spbu_id = ta.spbu_id and tf.feedback_type = 'BANDING' and tf.status not in ('APPROVE','REJECT','CLOSED')
                    ) THEN TRUE
                    ELSE FALSE
                END AS HasProgressBanding
            FROM trx_audit ta
            INNER JOIN spbu s ON s.id = ta.spbu_id
            INNER JOIN app_user au ON ta.app_user_id = au.id
            LEFT JOIN app_user au2 ON ta.app_user_id_auditor2 = au2.id
            WHERE 
            CASE
                WHEN au.username = @username THEN ta.form_status_auditor1
                WHEN au2.username = @username THEN ta.form_status_auditor2
                ELSE ta.status
            END NOT IN ('DRAFT','UNDER_REVIEW', 'VERIFIED', 'DELETED')
            AND (@username IN (au.username, au2.username))
            ORDER BY ta.audit_schedule_date ASC, s.spbu_no ASC
            LIMIT @limit OFFSET @offset;";

            countSql = @"
            SELECT COUNT(*)
            FROM trx_audit ta
            INNER JOIN app_user au ON ta.app_user_id = au.id
            LEFT JOIN app_user au2 ON ta.app_user_id_auditor2 = au2.id
            WHERE 
            CASE
                WHEN au.username = @username THEN ta.form_status_auditor1
                WHEN au2.username = @username THEN ta.form_status_auditor2
                ELSE ta.status
            END NOT IN ('DRAFT','UNDER_REVIEW', 'VERIFIED', 'DELETED')
            AND (@username IN (au.username, au2.username));";
        }

        var items = (await _conn.QueryAsync<TrxAuditListItemViewV2Model, SpbuViewV2Model, TrxAuditListItemViewV2Model>(
            sql,
            (audit, spbu) =>
            {
                audit.Spbu = spbu;
                return audit;
            },
            new { limit, offset, username },
            splitOn: "SpbuNo"
        )).ToList();

        var total = await _conn.ExecuteScalarAsync<int>(countSql, new { username });

        return (items, total);
    }

    public async Task<int> CountInProgressAsync(string username)
    {
        var countSql = @"
            SELECT COUNT(*)
            FROM trx_audit ta
            INNER JOIN app_user au ON ta.app_user_id = au.id
            LEFT JOIN app_user au2 ON ta.app_user_id_auditor2 = au2.id
            WHERE 
                CASE
                    WHEN au.username = @username THEN ta.form_status_auditor1
                    WHEN au2.username = @username THEN ta.form_status_auditor2
                    ELSE ta.status
                END IN ('IN_PROGRESS_INPUT', 'IN_PROGRESS_SUBMIT')
                AND (@username IN (au.username, au2.username));
        ";

        return await _conn.ExecuteScalarAsync<int>(countSql, new { username });
    }

    public async Task<(bool Success, string Message)> StartAuditAsync(string username, string auditId)
    {
        var trxAuditSql = @"
        SELECT a.id, a.status, a.app_user_id 
        FROM trx_audit a 
        INNER JOIN app_user au ON au.id = a.app_user_id
        LEFT JOIN app_user au2 ON a.app_user_id_auditor2 = au2.id
        WHERE a.id = @auditId AND (@username IN (au.username, au2.username))";

        var trxAudit = await _conn.QueryFirstOrDefaultAsync<dynamic>(trxAuditSql, new { auditId, username });
        if (trxAudit == null)
            return (false, "Audit tidak ditemukan atau bukan milik user ini");

        string newStatus = "IN_PROGRESS_INPUT";

        var countSql = "SELECT COUNT(1) FROM trx_audit where report_prefix = 'CB/AI/' and id != @auditId";
        var totalAuditCount = await _conn.ExecuteScalarAsync<int>(countSql, new { auditId });
        var reportPrefix = "CB/AI/";
        var reportNo = reportPrefix + (totalAuditCount + 1).ToString("D4");

        var countAuditor1Sql = @"select count(1) from trx_audit ta 
        inner join app_user au on au.id = ta.app_user_id 
        where au.username = @username and ta.id = @auditId";
        var countAuditor1 = await _conn.ExecuteScalarAsync<int>(countAuditor1Sql, new { username, auditId });

        if (countAuditor1 > 0)
        {            
            var updateSql = @"
            UPDATE trx_audit 
            SET status = @newStatus,
                form_status_auditor1 = @newStatus,
                updated_by = @username,
                updated_date = CURRENT_TIMESTAMP,
                audit_execution_time = CURRENT_TIMESTAMP,
                report_prefix = @reportPrefix,
                report_no = @reportNo
            WHERE id = @auditId";

            var affected = await _conn.ExecuteAsync(updateSql, new
            {
                newStatus,
                username,
                auditId,
                reportPrefix,
                reportNo
            });

            return affected > 0
                ? (true, "Audit berhasil dimulai")
                : (false, "Gagal memperbarui audit");
        }
        else
        {
            var updateSql = @"
            UPDATE trx_audit 
            SET status = @newStatus,
                form_status_auditor2 = @newStatus,
                updated_by = @username,
                updated_date = CURRENT_TIMESTAMP,
                audit_execution_time_auditor2 = CURRENT_TIMESTAMP
            WHERE id = @auditId";

            var affected = await _conn.ExecuteAsync(updateSql, new
            {
                newStatus,
                username,
                auditId
            });

            return affected > 0
                ? (true, "Audit berhasil dimulai")
                : (false, "Gagal memperbarui audit");
        }
    }

    public async Task<(bool Success, string Message)> CancelAuditAsync(string username, string auditId)
    {
        var trxAuditSql = @"
        SELECT a.id, a.status, a.app_user_id 
        FROM trx_audit a 
        INNER JOIN app_user au ON au.id = a.app_user_id
        LEFT JOIN app_user au2 ON a.app_user_id_auditor2 = au2.id
        WHERE a.id = @auditId AND (@username IN (au.username, au2.username))";

        var trxAudit = await _conn.QueryFirstOrDefaultAsync<dynamic>(trxAuditSql, new { auditId, username });
        if (trxAudit == null)
            return (false, "Audit tidak ditemukan atau bukan milik user ini");

        // Hapus data lama
        var existingChecklist = _context.TrxAuditChecklists.Where(x => x.TrxAuditId == auditId && x.CreatedBy == username);
        _context.TrxAuditChecklists.RemoveRange(existingChecklist);

        var existingQQ = _context.TrxAuditQqs.Where(x => x.TrxAuditId == auditId && x.CreatedBy == username);
        _context.TrxAuditQqs.RemoveRange(existingQQ);

        var existingMedia = _context.TrxAuditMedia.Where(x => x.TrxAuditId == auditId && x.CreatedBy == username);
        _context.TrxAuditMedia.RemoveRange(existingMedia);

        var countAuditor1Sql = @"select count(1) from trx_audit ta 
        inner join app_user au on au.id = ta.app_user_id 
        where au.username = @username and ta.id = @auditId";
        var countAuditor1 = await _conn.ExecuteScalarAsync<int>(countAuditor1Sql, new { username, auditId });

        if (countAuditor1 > 0)
        {
            var formStatusAuditor2Sql = @"select form_status_auditor2 from trx_audit ta where ta.id = @auditId";
            var formStatusAuditor2 = await _conn.ExecuteScalarAsync<string>(formStatusAuditor2Sql, new { auditId });

            var formTypeAuditor1Sql = @"select form_type_auditor1 from trx_audit ta where ta.id = @auditId";
            var formTypeAuditor1 = await _conn.ExecuteScalarAsync<string>(formTypeAuditor1Sql, new { auditId });
            
            // Hapus media fisik jika ada
            var updateSql = @"
                UPDATE trx_audit 
                SET report_prefix = '',
                    report_no = '',
                    audit_execution_time = null,
                    audit_mom_intro = '',
                    audit_mom_final = '',
                    form_status_auditor1 = 'NOT_STARTED',
                    updated_by = @username,
                    updated_date = CURRENT_TIMESTAMP ";
            if (formTypeAuditor1 == "FULL")
            {
                updateSql += @",
                audit_media_upload = 0,
                audit_media_total = 0 ";
            }

            if (formStatusAuditor2 == null || formStatusAuditor2 == "NOT_STARTED")
            {
                updateSql += @",
                status = 'NOT_STARTED' ";
            }

            updateSql += @"WHERE id = @auditId";

            var affected = await _conn.ExecuteAsync(updateSql, new
            {
                username,
                auditId
            });

            return affected > 0
                ? (true, "Audit berhasil di cancel")
                : (false, "Gagal cancel audit");
        }
        else
        {

            var formStatusAuditor1Sql = @"select form_status_auditor1 from trx_audit ta where ta.id = @auditId";
            var formStatusAuditor1 = await _conn.ExecuteScalarAsync<string>(formStatusAuditor1Sql, new { auditId });

            var updateSql = @"
            UPDATE trx_audit 
            SET audit_execution_time_auditor2 = null,
                form_status_auditor2 = 'NOT_STARTED',
                updated_by = @username,
                updated_date = CURRENT_TIMESTAMP ";

            if (formStatusAuditor1 != null && formStatusAuditor1 == "NOT_STARTED")
            {
                updateSql += @",
                status = 'NOT_STARTED' ";
            }

            updateSql += @"WHERE id = @auditId";

            var affected = await _conn.ExecuteAsync(updateSql, new
            {
                username,
                auditId
            });

            return affected > 0
                ? (true, "Audit berhasil di cancel")
                : (false, "Gagal cancel audit");
            
        }
    }
}
