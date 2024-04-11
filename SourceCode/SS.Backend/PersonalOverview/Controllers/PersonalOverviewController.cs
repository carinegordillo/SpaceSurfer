using Microsoft.AspNetCore.Mvc;

namespace PersonalOverview.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonalOverviewController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<PersonalOverviewController> _logger;

        public PersonalOverviewController(ILogger<PersonalOverviewController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<ReservationInformation> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new ReservationInformation
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
