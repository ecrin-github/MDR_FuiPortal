namespace MDR_FuiPortal.Server;

/*
public class ApiResponse<T>
{
    public int Total { get; set; }
    public int StatusCode { get; set; }
    public IList<string>? Messages { get; set; }
    public List<T>? Data { get; set; }
}

public class EmptyApiResponse
{
    public int Total { get; set; }
    public int StatusCode { get; set; }
    public IList<string>? Messages { get; set; }
    public List<string>? Data { get; set; } = null;
}


public class PagedApiResponse<T>
{
    public int StatusCode { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Uri? FirstPage { get; set; }
    public Uri? LastPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public Uri? NextPage { get; set; }
    public Uri? PreviousPage { get; set; }
    public int Total { get; set; }
    public IList<string>? Messages { get; set; }
    public List<T> Data { get; set; }

    public PagedApiResponse(List<T> data, int pageNumber, int pageSize)
    {
        // This constructor only called after valid set of paged data returned
        // Used as the first stage in constructing a full paged response

        PageNumber = pageNumber;
        PageSize = pageSize;
        Data = data;
        Total = data.Count;
        Messages = null;
        StatusCode = 200;
    }
}

public class PaginationQuery
{
    public string? pagenum { get; set; }
    public string? pagesize { get; set; }
}

public class PaginationRequest
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }

    public PaginationRequest(int pageNumber, int pageSize)
    {
        PageNum = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize > 100 ? 100 : pageSize;
    }
}

public class Statistic
{
    public string? StatType { get; set; }
    public int? StatValue { get; set; }

    public Statistic() { }

    public Statistic(string? statType, int? statValue)
    {
        StatType = statType;
        StatValue = statValue;
    }
}
*/

