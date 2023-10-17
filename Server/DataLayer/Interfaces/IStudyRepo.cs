using MDR_FuiPortal.Shared;
namespace MDR_FuiPortal.Server;


public interface IStudyRepo
{
    public Task<string?> FetchStudyByTypeAndId(int type_id, string reg_Id);

    public Task<IEnumerable<string>?> FetchStudiesByPMID(int pmid);

    public Task<IEnumerable<string>?> FetchStudiesBySearch(int search_scope, string search_string, int bucket);

     public Task<string?> FetchStudyById(int study_id);

    public Task<IEnumerable<IECLine>?> FetchStudyIEC(int study_id);
}
