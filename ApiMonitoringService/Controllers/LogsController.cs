using ApiMonitoringService.Contacts;
using ApiMonitoringService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiMonitoringService.Controllers
{
    
    public class LogsController : Controller
    {
        private readonly IMonitoringRepository _repository;

        public LogsController(ILogger<HomeController> logger,IMonitoringRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        private readonly ILogger<HomeController> _logger;

     

        [HttpGet]
        public async Task<IActionResult> Requests()
        {
            var logs =  await _repository.GetRequestLogsAsync();

            return View(logs);
        }


        [HttpGet]
        public async Task<IActionResult> Responses()
        {
            var logs =  await _repository.GetResponseLogsAsync();

            return View(logs);
        }
    }
}
