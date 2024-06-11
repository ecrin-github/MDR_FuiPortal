using MDR_FuiPortal.Shared;

namespace MDR_FuiPortal.Server;


public interface IStudyRepo
{
    public Task<IEnumerable<string>?> FetchStudyByRegId(string reg_Id);

    public Task<IEnumerable<string>?> FetchStudyByTypeAndId(int type_id, string reg_Id);

    public Task<IEnumerable<string>?> FetchStudiesByPMID(int pmid);

    public Task<IEnumerable<string>?> FetchStudiesBySearchByBucket(int search_scope, string search_string, int bucket, FilterParams? fp);

    public Task<int> FetchCountBySearchByBucket(int search_scope, string search_string, int bucket, FilterParams? fp);

    public Task<int?> FetchStudyCountBySearch(int scope, string pars);

    public Task<List<string>?> FetchStudySummariesBySearch(int scope, string pars, int offset, int limit);

    public Task<string?> FetchStudyById(int study_id);    
    
    public Task<string?> FetchStudyDetailsById(int study_id);

    public Task<string?> FetchStudyAllDetailsById(int study_id);    
    

    public Task<int?> FetchStudyId(int source_id, string sd_sid);

    public Task<string?> FetchOmicsDIData(string qtype, int offset, int limit);

    Task<int> GetTotalStudiesCount();
    
    Task<IDictionary<string, long>> GetStudyCountByStudyType();
    
    Task<IDictionary<string, long>> GetStudyCountByStudyStartYear();
    
    Task<(int count, IEnumerable<string>? res)> GetStudiesByCountriesListAsync(IList<string> countries, int pageSize, int pageNumber);
    
    Task<IDictionary<string, long>> GetTotalStudiesAndObjectsAsync();

    //public Task<IEnumerable<string>?> FetchPageStudiesBySearch(int search_scope, string search_string,
    //    int page_start, int page_size, FilterParams? fp);

    //public Task<int> FetchStudyCountBySearch(int search_scope, string search_string, FilterParams? fp);

    //public Task<IEnumerable<IECLine>?> FetchStudyIEC(int study_id);
}
