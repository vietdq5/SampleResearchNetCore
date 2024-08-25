namespace BaseProject.Settings;

public class ElasticsearchOptions
{
    public const string ElasticsearchOption = "ElasticsearchOptions";
    public string Uri { get; set; }
    public string DefaultIndex { get; set; }
    public string EmployeeIndex { get; set; }
}