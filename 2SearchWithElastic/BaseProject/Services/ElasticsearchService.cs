using BaseProject.Entities;
using BaseProject.Settings;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;

namespace BaseProject.Services;

public interface IElasticsearchService
{
    Task<IndexResponse?> IndexEmployee(Employee employee);

    Task<IReadOnlyCollection<Employee>?> SearchEmployees(string searchTerm, int from = 0, int size = 10);
}

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly ILogger<ElasticsearchService> _logger;
    private readonly string _indexName;

    public ElasticsearchService(ElasticsearchClient client, IOptions<ElasticsearchOptions> options, ILogger<ElasticsearchService> logger)
    {
        _elasticClient = client;
        _indexName = options.Value.EmployeeIndex;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Employee>?> SearchEmployees(string searchTerm, int from = 0, int size = 10)
    {
        try
        {
            //var response = await _elasticClient.SearchAsync<Employee>(s => s
            //.Index(_indexName)
            //.From(from)

            //.Size(size)
            //.Query(q => q
            //.MultiMatch(mm => mm
            //.Fields(Fields.FromExpressions<Employee>([p => p.LastName, p => p.FirstName]))
            //.Query(searchTerm)
            //.Type(TextQueryType.BestFields)
            //.Fuzziness(new Fuzziness("AUTO")))));
            var response = await _elasticClient.SearchAsync<Employee>(s => s
       .Query(q => q
           .Bool(b => b
               .Should(
                   sh => sh
                       .MultiMatch(mm => mm
                           .Fields(Fields.FromExpressions<Employee>([p => p.LastName, p => p.FirstName]))
                           .Query(searchTerm)
                           .Type(TextQueryType.BestFields)
                           .Operator(Operator.And)
                           .Fuzziness(new Fuzziness("AUTO"))
                       ),
                   sh => sh
                       .Match(m => m
                           .Field(f => f.FirstName)
                           .Query(searchTerm)
                           .Fuzziness(new Fuzziness("AUTO"))
                       ),
                   sh => sh
                       .Match(m => m
                           .Field(f => f.LastName)
                           .Query(searchTerm)
                           .Fuzziness(new Fuzziness("AUTO"))
                       )
               )
           )
       ));
            return response.Documents;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}.\n{ex.StackTrace}");
            return default;
        }
    }

    public async Task<IndexResponse?> IndexEmployee(Employee employee)
    {
        try
        {
            var response = await _elasticClient.IndexAsync(employee, i => i
                .Index(_indexName)
                .Id(employee.Id)
            );

            if (!response.IsValidResponse)
            {
                _logger.LogError($"Failed to index employee. Status code: {response.ApiCallDetails.HttpStatusCode}, Debug info: {response.DebugInformation}");
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}.\n{ex.StackTrace}");
            return default;
        }
    }
}