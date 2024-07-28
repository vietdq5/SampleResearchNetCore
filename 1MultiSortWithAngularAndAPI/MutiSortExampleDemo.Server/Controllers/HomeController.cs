using BaseProject.DataContext;
using BaseProject.Dtos;
using BaseProject.Entities;
using BaseProject.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text;

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

        [HttpGet("GetEmployees")]
        public async Task<IActionResult> GetEmployees([FromQuery] List<SortColumn> sortColumns)
        {
            // Fetch data from your data source (e.g., database)
            var query = _projectDbContext.Employees.AsQueryable();

            // Apply sorting
            if (sortColumns != null && sortColumns.Count > 0)
            {
                query = query.OrderByDynamic<Employee>(sortColumns);
            }
            var data = await query
                .AsNoTracking()
                .Select(s => new
                {
                    s.FirstName,
                    s.LastName,
                    s.JobTitle
                })
                .ToListAsync();
            return Ok(data);
        }


        /// <summary>
        /// v2 dung lib System.Linq.Dynamic.Core
        /// format query
        /// http://localhost:5210/Home/GetEmployeesV2?sortColumns[0].column=lastName&sortColumns[0].direction=asc&sortColumns[1].column=firstName&sortColumns[1].direction=desc
        /// </summary>
        /// <param name="sortColumns"></param>
        /// <returns></returns>
        [HttpGet("GetEmployeesV2")]
        public async Task<IActionResult> GetEmployeesV2([FromQuery] List<SortColumn> sortColumns)
        {
            // Fetch data from your data source (e.g., database)
            var query = _projectDbContext.Employees.AsQueryable();
            if (sortColumns?.Count > 0)
            {
                //List<string> columnQueries = [];
                //foreach (var item in sortColumns)
                //{
                //    var sortValue = item.Direction == SortColumn.Asc ? SortColumn.Ascending : SortColumn.Descending;
                //    columnQueries.Add($"{item.Column} {sortValue}");
                //}
                //var queryOrder = string.Join(",", columnQueries);
                // reduce code
                var queryOrder = string.Join(",", sortColumns.Select(item => $"{item.Column} {(item.Direction == SortColumn.Asc ? SortColumn.Ascending : SortColumn.Descending)}"));
                if (!string.IsNullOrEmpty(queryOrder))
                {
                    query = query.OrderBy(queryOrder);
                }
            }

            var data = await query
                .AsNoTracking()
                .Select(s => new
                {
                    s.FirstName,
                    s.LastName,
                    s.JobTitle
                })
                .ToListAsync();
            return Ok(data);
        }
    }
}