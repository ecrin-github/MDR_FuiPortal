﻿using Dapper;
using MDR_FuiPortal.Shared;
using Microsoft.Fast.Components.FluentUI.DesignTokens;
using Npgsql;
using System.Text;

namespace MDR_FuiPortal.Server;

public class StudyRepo : IStudyRepo
{
    private readonly string _dbConnString;
    private readonly string _aggsConnString;

    public StudyRepo(ICredentials creds)
    {
        _dbConnString = creds.GetConnectionString("mdr");
        _aggsConnString = creds.GetConnectionString("aggs");
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
        string search_pars = process_pars(search_string);
        string sql_string = $"select study_json from search.lexemes lx ";
        if (fp is not null && fp.pars_list != "")
        { 
            sql_string += ObtainSQLJoinClauses(fp);
        }
        sql_string += ObtainSQLWhereClauses(search_scope, search_pars, fp);
        return await GetIEnumerable<string>(sql_string);
    }


    public async Task<IEnumerable<string>?> FetchStudiesBySearchByBucket(int search_scope, string search_string,
        int bucket, FilterParams? fp)
    {
        string search_pars = process_pars(search_string);
        string sql_string = $"select study_json from search.lexemes lx ";
        if (fp is not null && fp.pars_list != "")
        {
            sql_string += ObtainSQLJoinClauses(fp);
        }
        sql_string += $" where lx.bucket = {bucket} ";
        sql_string += ObtainSQLWhereClauses(search_scope, search_pars, fp, true);

        return await GetIEnumerable<string>(sql_string);
    }


    public async Task<int> FetchCountBySearchByBucket(int search_scope, string search_string, int bucket, FilterParams? fp)
    {
        string search_pars = process_pars(search_string);
        string sql_string = $"select count(*) from search.lexemes lx ";
        if (fp is not null && fp.pars_list != "")
        {
            sql_string += ObtainSQLJoinClauses(fp);
        }
        sql_string += $" where lx.bucket = {bucket} ";
        sql_string += ObtainSQLWhereClauses(search_scope, search_pars, fp, true);

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


    public async Task<IEnumerable<string>?> FetchPageStudiesBySearch(int search_scope, string search_string,
        int page_start, int page_size, FilterParams? fp)
    {
        string search_pars = process_pars(search_string);
        string sql_string = $"select study_json from search.lexemes lx ";
        if (fp is not null && fp.pars_list != "")
        {
            sql_string += ObtainSQLJoinClauses(fp);
        }
        sql_string += ObtainSQLWhereClauses(search_scope, search_pars, fp);
        sql_string += $" order by study_id  offset {page_start} limit {page_size}";
        return await GetIEnumerable<string>(sql_string);
    }



    public async Task<int> FetchStudyCountBySearch(int search_scope, string search_string, FilterParams? fp)
    {
        string search_pars = process_pars(search_string);
        int total_count = 0;
        for (int bucket = 1; bucket < 21; bucket++)
        {
            string sql_string = $"select count(*) from search.lexemes lx ";
            if (fp is not null && fp.pars_list != "")
            {
                sql_string += ObtainSQLJoinClauses(fp);
            }
            sql_string += $" where lx.bucket = {bucket} ";

            sql_string += ObtainSQLWhereClauses(search_scope, search_pars, fp, true);

            using var conn = new NpgsqlConnection(_dbConnString);
            try
            {
                total_count += await conn.ExecuteScalarAsync<int>(sql_string);

            }
            catch (Exception e)
            {
                string s = e.Message;
                return 0;
            }
        }
        return total_count;
    }


    private string ObtainSQLJoinClauses(FilterParams? fp)
    {
        string sql_join_clauses = "";
        if (fp is not null && fp.pars_list != "")
        {
            if (fp.pars_list.Contains('T') || fp.pars_list.Contains('S') || fp.pars_list.Contains('Y')
                || fp.pars_list.Contains('A') || fp.pars_list.Contains('P'))
            {
                sql_join_clauses += " inner join search.studies ss on lx.study_id = ss.study_id ";
            }

            if (fp.pars_list.Contains('B'))
            {
                sql_join_clauses += " inner join search.object_types ob on lx.study_id = ob.study_id ";
            }

            if (fp.pars_list.Contains('C'))
            {
                sql_join_clauses += " inner join search.countries cs on lx.study_id = cs.study_id "; 
            }
        }
        return sql_join_clauses;

    }


    private string ObtainSQLWhereClauses(int search_scope, string search_pars, FilterParams? fp, bool preceding_term = false )
    {

        string sql_where_clauses = preceding_term ? " and " : " where ";
        bool prior_clause_added = false;

        if (search_pars != "ALL STUDIES")
        {
            //string s_string = process_search_string(search_string);
            string scope_string = "";
            if (search_scope == 1)
            { 
                scope_string = $" (tt_lex @@ to_tsquery('core.mdr_english_config2', '{search_pars}')) ";
            }
            else if (search_scope == 2)
            {
                scope_string = $" (conditions_lex @@ to_tsquery('core.mdr_english_config2', '{search_pars}')) ";
            }
            else if (search_scope == 3)
            {
                scope_string = $@"  (tt_lex @@ to_tsquery('core.mdr_english_config2', '{search_pars}') 
                                 or conditions_lex @@ to_tsquery('core.mdr_english_config2', '{search_pars}')) ";
            }
            prior_clause_added = true;
            sql_where_clauses += scope_string;
        }

        if (fp is not null && fp.pars_list != "")
        {
            if (fp.pars_list.Contains('T'))
            {
                string link_word = prior_clause_added ? " and " : "";
                sql_where_clauses += link_word + fp.study_type ?? "";
                prior_clause_added = true;
            }

            if (fp.pars_list.Contains('S'))
            {
                string link_word = prior_clause_added ? " and " : "";
                sql_where_clauses += link_word + fp.study_status ?? "";
                prior_clause_added = true;
            }

            if (fp.pars_list.Contains('Y'))
            {
                string year_pars = fp.start_year ?? "";
                string[] pars = (year_pars.Split(',', StringSplitOptions.TrimEntries));
                string link_word = prior_clause_added ? " and " : "";

                if (pars.Length == 2)
                {
                    string op = pars[0] switch
                    {
                        "eq" => "=",
                        "gt" => ">",
                        "lt" => "<",
                        _ => "",
                    };
                    sql_where_clauses += link_word + $" start_year {op} {pars[1]}";
                }
                if (pars.Length == 3 && pars[0] == "bw")
                {
                    sql_where_clauses += link_word + $" start_year between {pars[1]} and {pars[2]}";
                }                
                prior_clause_added = true;

            }

            if (fp.pars_list.Contains('P'))
            {
                string link_word = prior_clause_added ? " and " : "";
                sql_where_clauses += link_word + fp.phase;
                prior_clause_added = true;
            }

            if (fp.pars_list.Contains('A'))
            {
                string link_word = prior_clause_added ? " and " : "";
                sql_where_clauses += link_word + fp.alloc;
                prior_clause_added = true;
            }

            if (fp.pars_list.Contains('B'))
            {
                string link_word = prior_clause_added ? " and " : "";
                sql_where_clauses += link_word + $" ob.object_type_id in {fp.objects}";
                prior_clause_added = true;
            }

            if (fp.pars_list.Contains('C'))
            {
                string link_word = prior_clause_added ? " and " : "";
                sql_where_clauses += link_word + $" cs.country_id in {fp.countries}";
                prior_clause_added = true;
            }
        }

        return sql_where_clauses;

    }

    private string process_pars(string input_pars)
    {
        string output_pars = input_pars.Replace(" and ", " & ");
        output_pars = output_pars.Replace(" or ", " | ");
        output_pars = output_pars.Replace(" not(", " !(");
        return output_pars;
    }
            

    public async Task<string?> FetchStudyById(int study_id)
    {
        string sql_string = $@"select s.study_json
                             from search.lexemes s
                             where s.study_id = {study_id}";
        return await GetSingleRecord<string>(sql_string);

    }


    public async Task<string>? FetchStudyAllDetailsById(int study_id)
    {
        // for testing 2044457 = TNT trial

        string sql_study = @$"select full_study
                           from search.studies_json s
                           where s.id = {study_id}";

        string sql_objects = @$"select full_object
                           from search.objects_json b
                           inner join core.study_object_links k
                           on k.object_id = b.id
                           where k.study_id = {study_id}";

        // k.study_id = 2044457

        string? study_data = await GetSingleRecord<string>(sql_study);
        if (study_data is not null)
        {
            string res = "{\"full_study\": " + study_data;

            IEnumerable<string>? object_data = await GetIEnumerable<string>(sql_objects);
            if (object_data?.Any() == true)
            {
                int num_to_get = object_data.Count();
                StringBuilder sb = new StringBuilder(", " + "\"full_objects\": [");
                int n = 1;
                foreach (string s in object_data)
                {
                    sb.Append(s);
                    if (n != num_to_get)
                    {
                        sb.Append(", ");
                    }
                    n++;
                }
                sb.Append("]");
                string final_res = res + sb.ToString() + "}";
                return final_res;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }


    public async Task<string?> FetchStudyDetailsById(int study_id)
    {
        // for testing 2044457 = TNT trial

        string sql_study = @$"select full_study
                           from search.studies_json s
                           where s.id = {study_id}";

        return await GetSingleRecord<string>(sql_study);
    }



    public async Task<int?> FetchStudyId(int source_id, string sd_sid)
    {
        // source_id = 100126 and sd_sid = 'ISRCTN04968978'

        string sql_string = @$"select study_id from nk.study_ids
                           where source_id = {source_id} and sd_sid = '{sd_sid}'";
        
        using var conn = new NpgsqlConnection(_aggsConnString);
        try
        {
            return await conn.QueryFirstOrDefaultAsync<int>(sql_string);
        }
        catch
        {
            return null;
        }

    }



    public async Task<IEnumerable<IECLine>?> FetchStudyIEC(int study_id)
    {
        string sql_string = " ";
        return await GetIEnumerable<IECLine>(sql_string);
    }
}