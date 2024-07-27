using BaseProject.DataContext;
using BaseProject.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<HomeController> _logger;
        private readonly ProjectDbContext _projectDbContext;

        public HomeController(ILogger<HomeController> logger, ProjectDbContext projectDbContext)
        {
            _logger = logger;
            _projectDbContext = projectDbContext;
        }

        [HttpGet("GetWeatherForecast")]
        public async Task<IActionResult> GetWeatherForecast()
        {
            var data = Enumerable.Range(1, 5).Select(index => new
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
             .ToArray();
            var result = await Task.FromResult(data);
            return Ok(result);
        }

        [HttpGet("GetSubUnits")]
        public async Task<IActionResult> GetSubUnits()
        {
            var query = _projectDbContext.SubUnits.AsNoTracking().AsQueryable();
            var data = await query.ToListAsync();
            return Ok(data);
        }

        [HttpPost("AddSubUnit")]
        public async Task<IActionResult> AddSubUnit([FromBody] string name)
        {
            var subUnit = new SubUnit()
            {
                SubName = name
            };
            await _projectDbContext.SubUnits.AddAsync(subUnit);
            await _projectDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}