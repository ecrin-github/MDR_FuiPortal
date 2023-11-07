namespace MDR_FuiPortal.Server.Controllers;

/*
public class PagedResponseBuilder
{
    public static PagedApiResponse<T> CreatePagedResponse<T>(List<T> pagedData,
                    PaginationRequest validFilter, IUriService uriService,
                    int totalRecords, string route)
    {
        // This code and the uriService code taken from 
        // https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/

        // Initial construction of the response object
        var response = new PagedApiResponse<T>(pagedData, validFilter.PageNum, validFilter.PageSize);

        // get total pages, rounded up, with last page often < page size
        var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        // get URIs for related pages, using the uriService...
        response.NextPage =
            validFilter.PageNum >= 1 && validFilter.PageNum < roundedTotalPages
                ? uriService.GetPageUri(new PaginationRequest(validFilter.PageNum + 1, validFilter.PageSize), route)
                : null;
        response.PreviousPage =
            validFilter.PageNum - 1 >= 1 && validFilter.PageNum <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationRequest(validFilter.PageNum - 1, validFilter.PageSize), route)
                : null;
        response.FirstPage = uriService.GetPageUri(new PaginationRequest(1, validFilter.PageSize), route);
        response.LastPage = uriService.GetPageUri(new PaginationRequest(roundedTotalPages, validFilter.PageSize), route);
        response.TotalPages = roundedTotalPages;
        response.TotalRecords = totalRecords;
        return response;
    }

}
*/
