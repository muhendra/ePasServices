using Dapper;
using ePasServices.Data;
using ePasServices.Services.Interfaces;
using ePasServices.ViewModels;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class TrxFeedbackService : ITrxFeedbackService
{
    private readonly NpgsqlConnection _conn;
    private readonly PostgreDbContext _context;

    public TrxFeedbackService(IConfiguration config, PostgreDbContext context)
    {
        _conn = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        _context = context;
    }

    public async Task<string> GenerateTicketNoAsync(string trxAuditId, string feedbackType)
    {
        const string reportNoQuery = @"SELECT report_no FROM trx_audit WHERE id = @trxAuditId";
        var reportNo = await _conn.ExecuteScalarAsync<string>(reportNoQuery, new { trxAuditId });

        if (string.IsNullOrEmpty(reportNo))
            throw new Exception("Report number not found for trxAuditId: " + trxAuditId);

        const string countQuery = @"SELECT COUNT(*) FROM trx_feedback WHERE trx_audit_id = @trxAuditId and feedback_type = @feedbackType";
        var count = await _conn.ExecuteScalarAsync<int>(countQuery, new { trxAuditId, feedbackType });

        var feedbackIndex = count + 1;
        var formattedIndex = feedbackIndex.ToString("D3");

        var ticketNo = $"{reportNo}/{feedbackType[0]}/{formattedIndex}";
        return ticketNo;
    }
    
    public async Task<(List<TrxFeedbackListItemViewModel> Data, int Total)> GetTrxFeedbackListAsync(
        int page, 
        int limit, 
        string username, 
        string trxAuditId, 
        string feedbackType)
    {
        feedbackType = feedbackType.ToUpperInvariant();
        
        var offset = (page - 1) * limit;

        const string sql = @"
            SELECT 
                tf.id,
                tf.ticket_no AS TicketNo,
                tf.feedback_type AS FeedbackType,
                tfp.description AS Description,
                tf.status AS Status,
                tf.created_date AS CreatedDate,
                STRING_AGG(mqd.number::text, ', ') AS Numbers
            FROM trx_feedback tf
            INNER JOIN app_user au 
                ON au.id = tf.app_user_id
            INNER JOIN trx_feedback_point tfp 
                ON tfp.trx_feedback_id = tf.id
            LEFT JOIN trx_feedback_point_element tfpe 
                ON tfpe.trx_feedback_point_id = tfp.id
            LEFT JOIN master_questioner_detail mqd 
                ON mqd.id = tfpe.master_questioner_detail_id
            WHERE au.username = @username and tf.trx_audit_id = @trxAuditId and tf.feedback_type = @feedbackType
            GROUP BY tf.id, tf.ticket_no, tf.feedback_type, tfp.description, tf.status, tf.created_date
            ORDER BY tf.created_date DESC
            LIMIT @limit OFFSET @offset;
        ";

        const string countSql = @"
            SELECT COUNT(DISTINCT tf.id)
            FROM trx_feedback tf
            INNER JOIN app_user au 
                ON au.id = tf.app_user_id
            INNER JOIN trx_feedback_point tfp 
                ON tfp.trx_feedback_id = tf.id
            LEFT JOIN trx_feedback_point_element tfpe 
                ON tfpe.trx_feedback_point_id = tfp.id
            LEFT JOIN master_questioner_detail mqd 
                ON mqd.id = tfpe.master_questioner_detail_id
            WHERE au.username = @username and tf.trx_audit_id = @trxAuditId and tf.feedback_type = @feedbackType;
        ";

        var items = (await _conn.QueryAsync<TrxFeedbackListItemViewModel>(
            sql,
            new { limit, offset, username, trxAuditId, feedbackType}
        )).ToList();

        var total = await _conn.ExecuteScalarAsync<int>(countSql, new { username, trxAuditId, feedbackType});

        return (items, total);
    }
}
