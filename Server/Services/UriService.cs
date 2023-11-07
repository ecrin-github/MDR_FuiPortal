using Microsoft.AspNetCore.WebUtilities;

namespace MDR_FuiPortal.Server;

/*
public class UriService : IUriService
{
    private readonly string _baseUri;
    public UriService(string baseUri)
    {
        _baseUri = baseUri;
    }
    public Uri? GetPageUri(PaginationRequest filter, string route)
    {
        var endpointUri = new Uri(string.Concat(_baseUri, route));
        var modifiedUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "pageNum", filter.PageNum.ToString());
        modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());
        return new Uri(modifiedUri);
    }
}
*/