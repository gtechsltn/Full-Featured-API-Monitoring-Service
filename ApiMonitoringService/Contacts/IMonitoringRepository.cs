using ApiMonitoringService.Models;

namespace ApiMonitoringService.Contacts
{
    public interface IMonitoringRepository
    {
        Task<IEnumerable<RequestLog>> GetRequestLogsAsync();
        Task<IEnumerable<ResponseLog>> GetResponseLogsAsync();
        Task<int> LogRequestAsync(RequestLog requestLog);
        Task LogResponseAsync(ResponseLog responseLog);
    }
}