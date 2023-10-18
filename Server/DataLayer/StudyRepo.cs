using Dapper;
using MDR_FuiPortal.Shared;
using Npgsql;
using System.Data.SqlTypes;

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


    public async Task<IEnumerable<string>?> FetchStudyByTypeAndId(int type_id, string reg_Id)
    {
        var sql_string = $@"select study_json from search.idents
                              where ident_type = {type_id} and ident_value = '{reg_Id}'";
        return await GetIEnumerable<string>(sql_string);
    }


    public async Task<IEnumerable<string>?> FetchStudiesByPMID(int pmid)
    {
        var sql_string = $@"select study_json from search.pmids
                              where pmid = {pmid}";
        return await GetIEnumerable<string>(sql_string);
    }


    public async Task<IEnumerable<string>?> FetchStudiesBySearch(int search_scope, string search_string,
        int bucket, FilterParams? fp)
    {
        string sql_string = $"select study_json from search.lexemes lx ";
        if (fp is not null && fp.has_any)
        { 
            sql_string += ObtainSQLJoinClauses(fp);
        }
        sql_string += ObtainSQLWhereClauses(search_scope, search_string, bucket, fp);
        return await GetIEnumerable<string>(sql_string);
    }


    public async Task<int> FetchCountBySearch(int search_scope, string search_string, int bucket, FilterParams? fp)
    {
        string sql_string = $"select count(*) from search.lexemes lx ";
        if (fp is not null && fp.has_any)
        {
            sql_string += ObtainSQLJoinClauses(fp);
        }
        sql_string += ObtainSQLWhereClauses(search_scope, search_string, bucket, fp);
        
        using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            var res = await conn.ExecuteScalarAsync<int>(sql_string);
            return res;

        }
        catch (Exception e)
        {
            string s = e.Message;
            return 0;
        }
    }

    private string ObtainSQLJoinClauses(FilterParams? fp)
    {
        string sql_join_clauses = "";
        if (fp is not null && fp.has_any)
        {
            if (fp.study_type_num > 0 || fp.study_status_num > 0 || fp.start_year_num > 0
                || fp.alloc_num > 0 || fp.phase_num > 0)
            {
                sql_join_clauses += " inner join search.studies ss on lx.study_id = ss.study_id ";
            }

            if (fp.objects_num > 0)
            {
                sql_join_clauses += " inner join search.object_types ob on lx.study_id = ob.study_id ";
            }

            if (fp.countries_num > 0)
            {
                sql_join_clauses += " inner join search.countries cs on lx.study_id = cs.study_id "; 
            }
        }
        return sql_join_clauses;

    }

    private string ObtainSQLWhereClauses(int search_scope, string search_string,  int bucket, FilterParams? fp)
    {
        string sql_where_clauses = $" where lx.bucket = {bucket} ";
        if (search_string != "ALL STUDIES")
        {
            string s_string = process_search_string(search_string);
            sql_where_clauses += get_scope_string(search_scope, s_string);
        }
        if (fp is not null && fp.has_any)
        {
            sql_where_clauses += get_simple_filter_string(fp);

            if (fp.objects_num > 0)
            {
                sql_where_clauses += $" and ob.object_type_id in {fp.objects}";
            }

            if (fp.countries_num > 0)
            {
                sql_where_clauses += $" and cs.country_id in {fp.countries}";
            }
        }
        return sql_where_clauses;

    }

    private string process_search_string(string input)
    {
        string output = input.Replace(" or ", " | ");
        output = output.Replace(" and ", " & ");
        output = output.Replace(" not (", " !(").Replace(" not(", " !(");

        // Replace remaining spaces with '&', unless part of an existing | or & space

        int pars_length = output.Length;
        string st = "";
        for (int i = 0; i < pars_length; i++)
        {
            if (output[i] == ' ')
            {
                if (output[i - 1] != '|' && output[i + 1] != '|'
                && output[i - 1] != '&' && output[i + 1] != '&')
                {
                    st = st + " & ";
                }
                else
                {
                    st = st + output[i];
                }
            }
            else
            {
                st = st + output[i];
            }
        }
        return st;
    }
        
    private string get_scope_string(int search_scope, string s_string)
    {
        if (search_scope == 1)
        {
            return $" and (tt_lex @@ to_tsquery('core.mdr_english_config2', '{s_string}')) ";
        }
        else if (search_scope == 2)
        {
            return $" and (conditions_lex @@ to_tsquery('core.mdr_english_config2', '{s_string}')) ";
        }
        else if (search_scope == 3)
        {
            return $@" and (tt_lex @@ to_tsquery('core.mdr_english_config2', '{s_string}') 
                                 or conditions_lex @@ to_tsquery('core.mdr_english_config2', '{s_string}')) ";
        }
        else
        {
            return "";
        }
    }

    private string get_simple_filter_string(FilterParams fp)
    {
        string filter_string = "";

        if (fp.study_type_num > 0)
        {
            filter_string += " and " + fp.study_type ?? "";
        }

        if (fp.study_status_num > 0)
        {
            filter_string += " and " + fp.study_status ?? "";
        }

        if (fp.start_year_num > 0)
        {
            string year_pars = fp.start_year ?? "";
            string[] pars = (year_pars.Split(',', StringSplitOptions.TrimEntries));

            if (pars.Length == 2)
            {
                filter_string += $" and start_year {pars[0]} {pars[1]}";
            }
            if (pars.Length == 3)
            {
                filter_string += $" and start_year {pars[0]} {pars[1]} and {pars[2]}";
            }
        }

        if (fp.phase_num > 0)
        {
            filter_string += " and " + fp.phase;
        }

        if (fp.alloc_num > 0)
        {
            filter_string += " and " + fp.alloc;
        }

        return filter_string;
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