using Dapper;
using MDR_FuiPortal.Shared;
using Npgsql;

namespace MDR_FuiPortal.Server;

public class StudyRepo : IStudyRepo
{
    private readonly string _dbConnString;

    public StudyRepo(ICreds creds)
    {
        _dbConnString = creds.GetConnectionString("mdr");
    }

    private async Task<T?> GetSingleRecord<T>(string sql_string)
    {
        using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            var res = await conn.QueryFirstOrDefaultAsync<T>(sql_string);
            return res;
        }
        catch (Exception e)
        {
            string s = e.Message;
            return default(T);
        }

    }

    private async Task<IEnumerable<T>?> GetIEnumerable<T>(string sql_string)
    {
        using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            var res = await conn.QueryAsync<T>(sql_string);
            return res;

        }
        catch (Exception e)
        {
            string s = e.Message;
            return null;
        }

    }


    public async Task<string?> FetchStudyByTypeAndId(int type_id, string reg_Id)
    {
        var sql_string = $@"select study_json from search.idents
                              where ident_type = {type_id} and ident_value = '{reg_Id}'";
        return await GetSingleRecord<string>(sql_string);

    }


    public async Task<IEnumerable<string>?> FetchStudiesByPMID(int pmid)
    {
        var sql_string = $@"select study_json from search.pmids
                              where pmid = {pmid}";
        return await GetIEnumerable<string>(sql_string);
    }


    public async Task<IEnumerable<string>?> FetchStudiesBySearch(int search_scope, string search_string, int bucket)
    {
        string sql_string = $"select study_json from search.lexemes where bucket = {bucket} ";
        if (search_scope == 1)
        {
            sql_string += $" and (tt_lex @@ to_tsquery('core.mdr_english_config2', '{search_string}')) ";
        }
        else if (search_scope == 2)
        {
            sql_string += $" and (conditions_lex @@ to_tsquery('core.mdr_english_config2', '{search_string}')) ";
        }
        else if (search_scope == 3)
        {
            sql_string += $@" and (tt_lex @@ to_tsquery('core.mdr_english_config2', '{search_string}') 
                                 or conditions_lex @@ to_tsquery('core.mdr_english_config2', '{search_string}')) ";
        }

        return await GetIEnumerable<string>(sql_string);
    }


    public async Task<string?> FetchStudyById(int study_id)
    {
        // for now...
        // ...in the real system a full json equivalent of the study need to be returned

        string sql_string = $@"select s.study_json
                             from search.lexemes s
                             where s.study_id = {study_id}";
        return await GetSingleRecord<string>(sql_string);

    }


    public async Task<IEnumerable<IECLine>?> FetchStudyIEC(int study_id)
    {
        string sql_string = " ";
        return await GetIEnumerable<IECLine>(sql_string);

    }
}