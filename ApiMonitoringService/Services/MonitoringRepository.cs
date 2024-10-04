using ApiMonitoringService.Contacts;
using ApiMonitoringService.Data;
using ApiMonitoringService.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ApiMonitoringService.Services
{
    public class MonitoringRepository : IMonitoringRepository
    {
        private readonly DapperContext _connection;

        public MonitoringRepository(DapperContext connection)
        {
            _connection = connection;
        }

        private IDbConnection Connection => _connection.CreateDbConnection();

        public async Task<int> LogRequestAsync(RequestLog requestLog)
        {
            var query = @"
                INSERT INTO RequestLogs (Method, Endpoint, IpAddress, RequestTime, RequestPayload)
                VALUES (@Method, @Endpoint, @IpAddress, @RequestTime, @RequestPayload);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var connection = Connection)
            {
                return await connection.ExecuteScalarAsync<int>(query, requestLog);
            }
        }

        public async Task LogResponseAsync(ResponseLog responseLog)
        {
            var query = @"
                INSERT INTO ResponseLogs (RequestLogId, StatusCode, ResponsePayload, ResponseTime)
                VALUES (@RequestLogId, @StatusCode, @ResponsePayload, @ResponseTime);";

            using (var connection = Connection)
            {
                await connection.ExecuteAsync(query, responseLog);
            }
        }

        public async Task<IEnumerable<RequestLog>> GetRequestLogsAsync()
        {
            var query = "SELECT * FROM RequestLogs ORDER BY RequestTime DESC";
            using (var connection = Connection)
            {
                return await connection.QueryAsync<RequestLog>(query);
            }
        }

        public async Task<IEnumerable<ResponseLog>> GetResponseLogsAsync()
        {
            var query = "SELECT * FROM ResponseLogs ORDER BY ResponseTime DESC";
            using (var connection = Connection)
            {
                return await connection.QueryAsync<ResponseLog>(query);
            }
        }
    }
}
