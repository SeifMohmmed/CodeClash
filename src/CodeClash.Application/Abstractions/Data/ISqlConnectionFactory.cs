using System.Data;

namespace CodeClash.Application.Abstractions.Data;
public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
