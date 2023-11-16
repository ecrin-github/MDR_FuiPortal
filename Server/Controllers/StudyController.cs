using MDR_FuiPortal.Server.Controllers;
using MDR_FuiPortal.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MDR_FuiPortal.Server;

[Route("api/[controller]")]
public class StudyController : ControllerBase
{
    private readonly IStudyRepo _studyRepo;

    public StudyController(IStudyRepo studyRepo)
    {
        _studyRepo = studyRepo ?? throw new ArgumentNullException(nameof(studyRepo));
    }


    [HttpGet("BySearch/{scope:int}/{pars}/{bucket:int}/{count_only}/{fpj}")]
    public async Task<List<string>> GetStudiesBySearch(int scope, string pars, int bucket, string count_only, string fpj)
    {
        FilterParams? fp = JsonSerializer.Deserialize<FilterParams>(fpj);
        bool countOnly = count_only == "t";

        if (fp is not null)
        {
            if (countOnly)
            {
                int count_res = await _studyRepo.FetchCountBySearchByBucket(scope, pars, bucket, fp);
                return new List<string>() { $"count:{count_res}" };
            }
            else
            {
                var res = await _studyRepo.FetchStudiesBySearchByBucket(scope, pars, bucket, fp);
                if (res?.Any() != true)
                {
                    string res_content = "null result";
                    return new List<string>() { res_content };
                }
                else
                {
                    return res.ToList();
                }
            }
        }
        else
        {
            string res_content = "null result";
            return new List<string>() { res_content };
        }
    }


    [HttpGet("BySearch/Summaries/{scope:int}/{pars}/{offset:int}/{limit:int}")]
    public async Task<List<string>> GetStudySummariesBySearch(int scope, string pars, int offset, int limit)
    {
        var res = await _studyRepo.FetchStudySummariesBySearch(scope, pars, offset, limit);
        if (res?.Any() == true)
        {
            return res;
        }
        else
        {
            return null;
        }
    }


    [HttpGet("BySearch/Count/{scope:int}/{pars}")]
    public async Task<int?> GetStudyCountBySearch(int scope, string pars)
    {
        var res = await _studyRepo.FetchStudyCountBySearch(scope, pars);
        return res;

    }



    [HttpGet("ByRegId/{type_id:int}/{reg_id}")]
    public async Task<List<string>> GetStudyByTypeAndId(int type_id, string reg_id)
    {
        var res =  await _studyRepo.FetchStudyByTypeAndId(type_id, reg_id);
        if (res?.Any() != true)
        {
            string res_content = "null result";
            return new List<string>() { res_content };
        }
        else
        {
            return res.ToList();
        }
    }


    [HttpGet("ByPMID/{pmid:int}")]
    public async Task<List<string>> FetchStudiesByPMID(int pmid)
    {
        var res = await _studyRepo.FetchStudiesByPMID(pmid);
        if (res?.Any() != true)
        {
            string res_content = "null result";
            return new List<string>() { res_content };
        }
        else
        {
            return res.ToList();
        }
    }


    [HttpGet("{study_id:int}")]
    public async Task<List<string>?> GetStudyJsonById(int study_id)
    {
        var res = await _studyRepo.FetchStudyById(study_id);
        return res is not null ? new List<string>() { res } : null;
    }


    [HttpGet("AllDetails/{study_id:int}")]
    public async Task<string?> GetStudyAllDetailsById(int study_id)
    {
        string? res = await _studyRepo.FetchStudyAllDetailsById(study_id);
        return res;
    }

    [HttpGet("StudyDetails/{study_id:int}")]
    public async Task<string?> GetStudyDetailsById(int study_id)
    {
        string? res =  await _studyRepo.FetchStudyDetailsById(study_id);
        return res;
    }

    [HttpGet("OmicsDIdata/{qtype}/{offset:int}/{limit:int}")]
    public async Task<string?> GetOmicsDIData(string qtype, int offset, int limit)
    {
        string? res = await _studyRepo.FetchOmicsDIData(qtype, offset, limit);
        return res;
    }


    [HttpGet("MDRId/{source_id:int}/{sd_sid}")]
    public async Task<int?> GetStudyId(int source_id, string sd_sid)
    {
        var res =  await _studyRepo.FetchStudyId(source_id, sd_sid);
        return res;
    }


    /*
     * To do
     * 
    [HttpGet("/iec/{study_id:int}")]
    public async Task<List<IECLine>?> GetStudyIEC(int study_id)
    {
        var res = await _studyRepo.FetchStudyIEC(study_id);
        var listres = res is not null ? res.ToList() : null;
        return listres;
    }
    */

}

