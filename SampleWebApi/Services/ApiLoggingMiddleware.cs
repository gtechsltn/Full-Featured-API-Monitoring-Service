using Dapper;
using Microsoft.Data.SqlClient;
using SampleWebApi.Data;
using SampleWebApi.Models;
using System.Text;

namespace SampleWebApi.Services
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DapperContext _connection; // Store the connection string for Dapper

        public ApiLoggingMiddleware(RequestDelegate next, DapperContext connection)
        {
            _next = next;
            _connection = connection;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestLog = new RequestLog
            {
                Method = context.Request.Method,
                Endpoint = context.Request.Path,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                RequestTime = DateTime.UtcNow,
                RequestPayload = await ReadRequestBodyAsync(context.Request)
            };

            // Insert the request log using Dapper
            var requestId = await InsertRequestLogAsync(requestLog);

            // Capture the original response stream
            var originalResponseStream = context.Response.Body;
            using (var responseBodyStream = new MemoryStream())
            {
                context.Response.Body = responseBodyStream;

                await _next(context);

                // Log the response
                var responseLog = new ResponseLog
                {
                    RequestLogId = requestId,
                    StatusCode = context.Response.StatusCode,
                    ResponseTime = DateTime.UtcNow,
                    ResponsePayload = await ReadResponseBodyAsync(responseBodyStream)
                };

                // Insert the response log using Dapper
                await InsertResponseLogAsync(responseLog);

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalResponseStream);
            }
        }

        private async Task<int> InsertRequestLogAsync(RequestLog log)
        {
            using (var connection = _connection.CreateDbConnection())
            {
                var query = @"INSERT INTO RequestLogs (Method, Endpoint, IpAddress, RequestTime, RequestPayload)
                              VALUES (@Method, @Endpoint, @IpAddress, @RequestTime, @RequestPayload);
                              SELECT CAST(SCOPE_IDENTITY() as int);"; // Get the newly inserted record ID

                return await connection.QuerySingleAsync<int>(query, log);
            }
        }

        private async Task InsertResponseLogAsync(ResponseLog log)
        {
            using (var connection = _connection.CreateDbConnection())
            {
                var query = @"INSERT INTO ResponseLogs (RequestLogId, StatusCode, ResponseTime, ResponsePayload)
                              VALUES (@RequestLogId, @StatusCode, @ResponseTime, @ResponsePayload);";

                await connection.ExecuteAsync(query, log);
            }
        }

        
        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0; // Reset the stream position
                return body;
            }
        }

        private async Task<string> ReadResponseBodyAsync(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(stream))
            {
                var text = await reader.ReadToEndAsync();
                stream.Seek(0, SeekOrigin.Begin);
                return text;
            }
        }
    }
}
