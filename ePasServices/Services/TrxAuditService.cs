﻿using Dapper;
using ePasServices.Data;
using ePasServices.Models;
using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class TrxAuditService : ITrxAuditService
{
    private readonly NpgsqlConnection _conn;
    private readonly PostgreDbContext _context;

    public TrxAuditService(IConfiguration config, PostgreDbContext context)
    {
        _conn = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        _context = context;
    }

    public async Task<(List<TrxAuditListItemViewModel> Data, int Total)> GetTrxAuditListAsync(int page, int limit, bool history, string username)
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
                ta.status AS Status,
                s.spbu_no AS SpbuNo,
                s.address,
                s.latitude,
                s.longitude,
                ARRAY(
                    SELECT filepath FROM spbu_image si
                    WHERE si.spbu_id = s.id
                ) AS Images
            FROM trx_audit ta
            INNER JOIN spbu s ON s.id = ta.spbu_id
            INNER JOIN app_user au ON ta.app_user_id = au.id
            WHERE ta.status IN ('UNDER_REVIEW', 'VERIFIED')
            AND au.username = @username
            ORDER BY ta.audit_schedule_date ASC
            LIMIT @limit OFFSET @offset;";

            countSql = @"
            SELECT COUNT(*)
            FROM trx_audit ta
            INNER JOIN app_user au ON ta.app_user_id = au.id
            WHERE ta.status IN ('UNDER_REVIEW', 'VERIFIED')
            AND au.username = @username;";
        }
        else
        {
            sql = @"
            SELECT
                ta.id,
                ta.audit_level AS AuditLevel,
                ta.audit_type AS AuditType,
                ta.audit_schedule_date AS AuditScheduleDate,
                ta.status AS Status,
                s.spbu_no AS SpbuNo,
                s.Address,
                s.latitude,
                s.longitude,
                ARRAY(
                    SELECT filepath FROM spbu_image si
                    WHERE si.spbu_id = s.id
                ) AS Images
            FROM trx_audit ta
            INNER JOIN spbu s ON s.id = ta.spbu_id
            INNER JOIN app_user au ON ta.app_user_id = au.id
            WHERE ta.status NOT IN ('UNDER_REVIEW', 'VERIFIED')
            AND au.username = @username
            ORDER BY ta.audit_schedule_date ASC, s.spbu_no ASC
            LIMIT @limit OFFSET @offset;";

            countSql = @"
            SELECT COUNT(*)
            FROM trx_audit ta
            INNER JOIN app_user au ON ta.app_user_id = au.id
            WHERE ta.status NOT IN ('UNDER_REVIEW', 'VERIFIED')
            AND au.username = @username;";
        }

        var items = (await _conn.QueryAsync<TrxAuditListItemViewModel, SpbuViewModel, TrxAuditListItemViewModel>(
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
            WHERE ta.status IN ('IN_PROGRESS_INPUT', 'IN_PROGRESS_SUBMIT')
              AND au.username = @username;
        ";

        return await _conn.ExecuteScalarAsync<int>(countSql, new { username });
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

        //var introSql = @"
        //SELECT mq.id 
        //FROM trx_audit ta  
        //LEFT JOIN master_questioner mq ON ta.master_questioner_intro_id = mq.id 
        //WHERE mq.category = 'INTRO'
        //  AND ta.audit_type = mq.type
        //  AND ta.id = @auditId
        //ORDER BY mq.version DESC 
        //LIMIT 1";

        //string? introId = await _conn.ExecuteScalarAsync<string?>(introSql, new { auditId });

        //var checklistSql = @"
        //SELECT mq.id 
        //FROM trx_audit ta  
        //LEFT JOIN master_questioner mq ON ta.master_questioner_checklist_id = mq.id 
        //WHERE mq.category = 'CHECKLIST'
        //  AND ta.audit_type = mq.type
        //  AND ta.id = @auditId
        //ORDER BY mq.version DESC 
        //LIMIT 1";

        //string? checklistId = await _conn.ExecuteScalarAsync<string?>(checklistSql, new { auditId });

        var countSql = "SELECT COUNT(1) FROM trx_audit where report_prefix = 'CB/AI/'";
        var totalAuditCount = await _conn.ExecuteScalarAsync<int>(countSql);
        var reportPrefix = "CB/AI/";
        var reportNo = reportPrefix + (totalAuditCount + 1).ToString("D4");
        var audit_execution_time = DateTime.Now;

        var updateSql = @"
        UPDATE trx_audit 
        SET status = @newStatus,
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

    public async Task<(bool Success, string Message)> CancelAuditAsync(string username, string auditId)
    {
        var trxAuditSql = @"
        SELECT a.id, a.status, a.app_user_id 
        FROM trx_audit a 
        INNER JOIN app_user u ON u.id = a.app_user_id
        WHERE a.id = @auditId AND u.username = @username";

        var trxAudit = await _conn.QueryFirstOrDefaultAsync<dynamic>(trxAuditSql, new { auditId, username });
        if (trxAudit == null)
            return (false, "Audit tidak ditemukan atau bukan milik user ini");

        // Hapus data lama
        var existingChecklist = _context.TrxAuditChecklists.Where(x => x.TrxAuditId == auditId);
        _context.TrxAuditChecklists.RemoveRange(existingChecklist);

        var existingQQ = _context.TrxAuditQqs.Where(x => x.TrxAuditId == auditId);
        _context.TrxAuditQqs.RemoveRange(existingQQ);

        var existingMedia = _context.TrxAuditMedia.Where(x => x.TrxAuditId == auditId);
        _context.TrxAuditMedia.RemoveRange(existingMedia);

        var updateSql = @"
        UPDATE trx_audit 
        SET report_prefix = '',
            report_no = '',
            audit_execution_time = null,
            audit_media_upload = 0, 
            audit_media_total = 0, 
            audit_mom_intro = '',
            audit_mom_final = '',
            status = 'NOT_STARTED',
            updated_by = @username,
            updated_date = CURRENT_TIMESTAMP
        WHERE id = @auditId";

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
