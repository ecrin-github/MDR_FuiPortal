using MDR_FuiPortal.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace MDR_FuiPortal.Server.Controllers;

[Route("api/[controller]")]

public class StudyController :  BaseApiController
{
    private readonly IStudyRepo _studyRepo;

    public StudyController(IStudyRepo studyRepo)
    {
        _studyRepo = studyRepo ?? throw new ArgumentNullException(nameof(studyRepo));
    }

    [HttpGet("BySearch/{search_scope:int}/{search_string}/{bucket:int}")]
    public async Task<List<string>> GetStudiesBySearch(int search_scope, string search_string, int bucket)
    {
        var res = await _studyRepo.FetchStudiesBySearch(search_scope, search_string, bucket);
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


    [HttpGet("ByRegId/{type_id:int}/{reg_id}")]
    public async Task<List<string>> GetStudyByTypeAndId(int type_id, string reg_id)
    {
        var res =  await _studyRepo.FetchStudyByTypeAndId(type_id, reg_id);
        if (res is null)
        {
            res = "null result";
        }
        return new List<string>() { res };
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

