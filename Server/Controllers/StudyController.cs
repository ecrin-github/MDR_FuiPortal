using MDR_FuiPortal.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MDR_FuiPortal.Server.Controllers;

[Route("api/[controller]")]

public class StudyController :  BaseApiController
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


    [HttpGet("ByPageSearch/{spj}")]
    public async Task<List<string>> GetPageFromJsonAsync(string spj)
    {
        SearchParamsPage? sp = JsonSerializer.Deserialize<SearchParamsPage>(spj);
        if (sp is not null)
        {
            var res = await _studyRepo.FetchPageStudiesBySearch(sp.scope, sp.pars!, sp.page_start, sp.page_size, sp.fp);
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
        else
        {
            string res_content = "null result";
            return new List<string>() { res_content };
        }
    }



    [HttpGet("SearchTotal/{spj}")]
    public async Task<int> GetTotalNumBySearch(string spj)
    {
        SearchParams? sp = JsonSerializer.Deserialize<SearchParams>(spj);
        if (sp is not null)
        {
            return await (_studyRepo.FetchStudyCountBySearch(sp.scope, sp.pars!, sp.fp));
        }
        else
        {
            return 0;
        }
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


    /*
    [HttpGet("{study_id:int}")]
    public async Task<List<StudyDetails>?> GetStudyDetails(int study_id)
    {
        var res = await _studyRepo.FetchStudyDetails(study_id);
        var listres = res is not null ? new List<StudyDetails>() { res } : null;
        return listres;
    }
    */

    // temp for early testing of the principle

    [HttpGet("{study_id:int}")]
    public async Task<List<string>?> GetStudyJsonById(int study_id)
    {
        var res = await _studyRepo.FetchStudyById(study_id);
        return res is not null ? new List<string>() { res } : null;
    }


    [HttpGet("/iec/{study_id:int}")]
    public async Task<List<IECLine>?> GetStudyIEC(int study_id)
    {
        var res = await _studyRepo.FetchStudyIEC(study_id);
        var listres = res is not null ? res.ToList() : null;
        return listres;
    }

}

