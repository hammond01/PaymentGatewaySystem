using PaymentGateway.Domain.Entities.Audit;
using PaymentGateway.Domain.Repositories;
using PaymentGateway.Domain.Request;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using PaymentGateway.Ultils.Extension;

namespace PaymentGateway.Persistence.Repositories;

public class AuditRepository : IAuditServices
{
    private readonly IDataAccess _db;

    public AuditRepository(IDataAccess db)
    {
        _db = db;
    }

    public void InsertAuditLogs(AuditRequest auditRequest)
    {
        try
        {
            var auditModel = new AuditModel
            {
                AuditLogId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now.ToString("dd/MM/yyyy"),
                UserId = auditRequest.UserId,
                Action = auditRequest.Action,
                ActionStatus = auditRequest.ActionStatus,
                ActionIp = auditRequest.ActionIp,
                ControllerName = auditRequest.ControllerName,
                PaymentStatus = auditRequest.PaymentStatus
            };
            var query = Extension.GetInsertQuery("Audit", "AuditLogId", "UserId", "Action", "CreatedAt",
                "ActionStatus", "ActionIp", "ControllerName", "PaymentStatus");
            _db.SaveData(query, auditModel);
        }
        catch
        {
            throw new Exception("Internal server error!");
        }
    }
}