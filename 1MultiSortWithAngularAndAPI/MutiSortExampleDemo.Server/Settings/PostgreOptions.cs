namespace BaseProject.Settings;

public class PostgreOptions
{
    public const string PostgreOption = "PostgreOption";
    public string? LocalConnectionString { get; set; }
    public string? ServerConnectionString { get; set; }
}
