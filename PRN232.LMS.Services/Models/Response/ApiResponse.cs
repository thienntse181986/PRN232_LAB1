namespace PRN232.LMS.Services.Models.Response;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Request processed successfully")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, object? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

public class PagedResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<T>? Data { get; set; }
    public PaginationMeta? Pagination { get; set; }
    public object? Errors { get; set; }

    public static PagedResponse<T> Ok(IEnumerable<T> data, PaginationMeta pagination, string message = "Request processed successfully")
        => new() { Success = true, Message = message, Data = data, Pagination = pagination };

    public static PagedResponse<T> Fail(string message, object? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

public class PaginationMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}
