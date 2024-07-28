namespace BaseProject.Dtos;

public class SortColumn
{
    public static readonly string Asc = "asc";
    public static readonly string Desc = "desc";
    public static readonly string Ascending = "ascending";
    public static readonly string Descending = "descending";
    public string Column { get; set; }
    public string Direction { get; set; }
}