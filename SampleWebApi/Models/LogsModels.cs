namespace SampleWebApi.Models
{
    public class RequestLog
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public string Endpoint { get; set; }
        public string IpAddress { get; set; }
        public DateTime RequestTime { get; set; }
        public string RequestPayload { get; set; }
    }

    public class ResponseLog
    {
        public int Id { get; set; }
        public int RequestLogId { get; set; }
        public int StatusCode { get; set; }
        public string ResponsePayload { get; set; }
        public DateTime ResponseTime { get; set; }
    }

    public class ApiEndpoint
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EndpointUrl { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
