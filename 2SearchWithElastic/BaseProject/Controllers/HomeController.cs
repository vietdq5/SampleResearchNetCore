using BaseProject.Entities;
using BaseProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;
    private readonly IElasticsearchService _elasticsearchService;

    public HomeController(ILogger<HomeController> logger, IElasticsearchService elasticsearchService)
    {
        _logger = logger;
        _elasticsearchService = elasticsearchService;
    }

    [HttpPost("employee")]
    public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
    {
        var result = await _elasticsearchService.IndexEmployee(employee);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var results = await _elasticsearchService.SearchEmployees(query);
        return Ok(results);
    }
}