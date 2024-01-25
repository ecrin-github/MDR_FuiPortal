using Dapper;
using MDR_FuiPortal.Shared;
using Npgsql;
using System.Text;
using System.Xml.Linq;

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
                scope_string = $@"  (  (tt_lex @@ to_tsquery('core.mdr_english_config2', '{search_pars}')) 
                                 OR    (conditions_lex @@ to_tsquery('core.mdr_english_config2', '{search_pars}')) ) ";
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


    public async Task<string?> FetchStudyDetailsById(int study_id)
    {
        // for testing 2044457 = TNT trial

        string sql_study = @$"select full_study
                           from search.studies_json s
                           where s.id = {study_id}";

        return await GetSingleRecord<string>(sql_study);
    }


    public async Task<string?> FetchStudyAllDetailsById(int study_id)
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
            IEnumerable<string>? object_data = await GetIEnumerable<string>(sql_objects);
            if (object_data?.Any() == true)
            {
                string final_res = "{\"full_study\": " + study_data + ", \"full_objects\": [";
                int num_to_get = object_data.Count();
                StringBuilder sb = new StringBuilder();
                int n = 1;
                foreach (string s in object_data)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (n == 1)
                        {
                            sb.Append(s);
                        }
                        else
                        {
                            sb.Append(", " + s);
                        }
                        n++;
                    }
                }
                sb.Append("]}");
                final_res += sb.ToString();
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


    public async Task<int?> FetchStudyCountBySearch(int scope, string pars)
    {
        int TotalRecords = 0;
        string search_pars = process_pars(pars);
        string search_string = ObtainSearchParsWhereClauses(scope, search_pars);
        
        using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            for (int b = 1; b < 21; b++)
            {
                string sql_string = $"select count(*) from search.lexemes lx ";
                sql_string += $" where lx.bucket = {b} ";
                sql_string += search_string;
                int res = await conn.QueryFirstOrDefaultAsync<int>(sql_string);
                TotalRecords += res;
            }
            return TotalRecords;
        }
        catch
        {
            return null;
        }
    }


    public async Task<List<string>?> FetchStudySummariesBySearch(int scope, string pars, int start_number, int amount)
    {
        if (amount > 1000)
        {
            amount = 1000;
        }
        int last_number = start_number + amount - 1;
        int running_total = 0;

        List<string> search_results = new();
        string search_pars = process_pars(pars);
        using var conn = new NpgsqlConnection(_dbConnString);
        string search_string = ObtainSearchParsWhereClauses(scope, search_pars);

        try
        {
            for (int b = 1; b < 21; b++)
            {
                string sql_string = $"select study_json from search.lexemes lx ";
                sql_string += $" where lx.bucket = {b} ";
                sql_string += search_string;
                sql_string += $" order by study_id ";
                IEnumerable<string>? res = (await GetIEnumerable<string>(sql_string));

                if (res?.Any() == true)
                {
                    List<string> res_list = res.ToList();
                    int bucket_amount = res.Count();      
                    //int new_running_total = running_total + bucket_amount;

                    if (running_total < start_number)  // not yet reached start of rerquired set
                    {
                        if (running_total + bucket_amount < start_number)
                        {
                            // do nothing - still have not reached the requested records!
                        }
                        else
                        {
                            int bucket_start_pos = start_number - running_total;
                            if (running_total + bucket_amount > last_number)
                            {
                                int bucket_end_pos = last_number - running_total;

                                // This bucket load contains both start and end of the required studies
                                // Add records in this bucket between the bucket_start_pos and the bucket_end_pos

                                search_results = res_list.GetRange(bucket_start_pos, amount);
                                break;
                            }
                            else
                            {
                                // Add all of this bucket load to the existing set of search results;
                                // starting at the bucket_start_pos;
                                int number_to_add = bucket_amount - bucket_start_pos;
                                search_results = res_list.GetRange(bucket_start_pos, number_to_add);
                            }
                        }
                    }
                    else
                    {
                        // study data collection must have already begun

                        if (running_total + bucket_amount > last_number)
                        {
                            // This bucket load contains both start and end of the required studies
                            // Add records in this bucket between the bucket_start_pos and the bucket_end_pos

                            int amount_to_add = last_number - running_total;
                            search_results.AddRange(res_list.GetRange(0, amount_to_add));
                            break;
                        }
                        else
                        {
                            // Add all of this bucket load to the existing set of search results;

                            search_results.AddRange(res_list);
                        }

                    }
                    running_total = running_total + bucket_amount;
                }
            }
            if (search_results?.Any() == true)
            {
                return search_results;  // get to here from breaking out of loop or running out of buckets
            }
            else
            {
                return null;
            }
        }
        catch
        {
            return null;
        }
    }


    private string ObtainSearchParsWhereClauses(int search_scope, string search_pars)
    {
        string scope_string = "";
        if (search_pars != "ALL STUDIES")
        {
            if (search_scope == 1)
            {
                scope_string = $" AND (tt_lex @@ to_tsquery('core.mdr_english_config2', '{search_pars}')) ";
            }
            else if (search_scope == 2)
            {
                scope_string = $" AND (conditions_lex @@ to_tsquery('core.mdr_english_config2', '{search_pars}')) ";
            }
            else if (search_scope == 3)
            {
                scope_string = $@" AND
                          (               (tt_lex @@ to_tsquery('core.mdr_english_config2', '{search_pars}')) 
                              OR  (conditions_lex @@ to_tsquery('core.mdr_english_config2', '{search_pars}')) )";
            }
        }
        return scope_string;
    }



        public async Task<string?> FetchOmicsDIData(string qtype, int offset, int limit)
    {
        string sql_string = "";
        if (qtype == "CRC")
        {
            sql_string = @$"select count(*) as rec_num, string_agg(x.c19p, '') as entries
                         from
                         (
                             select sj.c19p 
                             from search.lexemes lx 
                             inner join search.studies_json sj 
                             on lx.study_id = sj.id 
                             where
                             tt_lex @@ to_tsquery('core.mdr_english_config2', '(bowel & cancer) | (colorectal & cancer)') 
                             or conditions_lex @@ to_tsquery('core.mdr_english_config2', '(bowel & cancer) | (colorectal & cancer)')
                             order by sj.id 
                             offset {offset} limit {limit}
                         ) x";
        }
        else if (qtype == "COVID")
        {
            sql_string = @$"select count(*) as rec_num, string_agg(x.c19p, '') as entries
                         from
                         (
                             select sj.c19p 
                             from search.lexemes lx 
                             inner join search.studies_json sj 
                             on lx.study_id = sj.id 
                             where
                             tt_lex @@ to_tsquery('core.mdr_english_config2', 'covid | coronavirus | SARS-2') 
                             or conditions_lex @@ to_tsquery('core.mdr_english_config2', 'covid | coronavirus | SARS-2')
                             order by sj.id 
                             offset {offset} limit {limit}
                         ) x";
        }

        if (sql_string != "")
        {
            using var conn = new NpgsqlConnection(_dbConnString);
            try
            {
                QRes? res = await conn.QuerySingleOrDefaultAsync<QRes>(sql_string);
                if (res is not null)
                {
                    string date_string = DateTime.Now.ToString("yyyy-MM-dd");
                    string release_string = DateTime.Now.ToString("MMMyy");
                    string xml_string = $@"<database>
                              <name>ECRIN MDR</name>
                              <description>The MDR aggregates metadata describing clinical studies and the  data objects (both inputs and outputs) associated with them. It derives its data from trial registries, bibliographic resources and data repositories.</description>
	                          <url>https://newmdr.ecrin.org</url>
                              <release>{release_string}</release>
                              <release_date>{date_string}</release_date>
                              <entry_count>{res.rec_num}</entry_count>
	                          <keywords>clinical research, clinical trials, interventional studies, cohort studies, observational health research</keywords>
                              ";
                    var entries = "<entries>";
                    entries += res.entries;
                    entries += "</entries>";

                    var updEntriesSb = new StringBuilder(); 
                    updEntriesSb.Append("<entries>");
                    
                    var xDoc = XDocument.Parse(entries);
                    foreach (var entry in xDoc.Descendants("entry"))
                    {
                        var location = (string)entry.Descendants("field")
                            .First(x => (string)x.Attribute("name") == "location");
                        var split_string = location.Split(',');
                        if (split_string.Length <= 1)
                        {
                            updEntriesSb.Append(entry);
                            continue;
                        }

                        var additionalFields = entry.Descendants("additional_fields").First();
                        additionalFields.Descendants("field")
                            .First(x => (string)x.Attribute("name") == "location").Remove();
                        
                        var locationsStringBuilder = new StringBuilder();
                        foreach (var location_string in split_string)
                        {
                            locationsStringBuilder.Append($"<field name=\"location\">{location_string}</field>");
                            additionalFields.Add(new XElement("field", new XAttribute("name", "location"), location_string.Trim()));
                        }
                        
                        updEntriesSb.Append(entry);
                    }
                    
                    updEntriesSb.Append("</entries>");

                    
                    xml_string += updEntriesSb.ToString();
                    xml_string += "</database>";
                    return xml_string;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                string s = e.Message;
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    private class QRes
    {
        public int rec_num { get; set; }
        public string? entries { get; set; }
    }



    public async Task<IEnumerable<IECLine>?> FetchStudyIEC(int study_id)
    {
        string sql_string = " ";
        return await GetIEnumerable<IECLine>(sql_string);
    }
}