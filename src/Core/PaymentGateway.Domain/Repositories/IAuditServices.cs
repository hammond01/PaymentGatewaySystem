using PaymentGateway.Domain.Request;

namespace PaymentGateway.Domain.Repositories
{
    public interface IAuditServices
    {
        void InsertAuditLogs(AuditRequest auditModel);
    }
}
