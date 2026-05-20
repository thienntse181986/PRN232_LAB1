namespace PRN232.LMS.Services.Models.Request;

/// <summary>
/// Common query parameters for list APIs supporting search, sort, paging, fields, and expand.
/// </summary>
public class QueryParameters
{
    private int _page = 1;
    private int _size = 10;
    private const int MaxSize = 100;

    /// <summary>Page number (1-based)</summary>
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// <summary>Page size (max 100)</summary>
    public int Size
    {
        get => _size;
        set => _size = value > MaxSize ? MaxSize : value < 1 ? 10 : value;
    }

    /// <summary>Search keyword (applied to string fields)</summary>
    public string? Search { get; set; }

    /// <summary>
    /// Sort fields separated by comma. Prefix with '-' for descending.
    /// Example: "fullName,-dateOfBirth"
    /// </summary>
    public string? Sort { get; set; }

    /// <summary>
    /// Comma-separated list of fields to include in response.
    /// Example: "studentId,fullName,email"
    /// </summary>
    public string? Fields { get; set; }

    /// <summary>
    /// Comma-separated list of related data to expand.
    /// Example: "student,course"
    /// </summary>
    public string? Expand { get; set; }
}
