using System.Collections;
using System.Runtime.CompilerServices;
using Dapper;
using MDR_FuiPortal.Shared;
using Npgsql;
using System.Text;
using System.Xml.Linq;
using ServiceStack;
using ServiceStack.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MDR_FuiPortal.Server;

public class StudyRepo : IStudyRepo
{
    private readonly string _dbConnString;
    private readonly string _aggsConnString;
    private readonly ILookUpRepo _lookUpRepo;

    public StudyRepo(ICredentials creds, ILookUpRepo lookUpRepo)
    {
        _lookUpRepo = lookUpRepo;
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
    
    private async Task<string?> GetSingleRecord(string sql_string)
    {
        using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            var res = await conn.QueryFirstOrDefaultAsync<string>(sql_string);
            
            var summary = JsonSerializer.Deserialize<StudySummary>(res);
            var details = await FetchStudyDetailsById(summary!.study_id);
            var deserializedDetails = JsonSerializer.Deserialize<JSONFullStudy>(details!);
            summary.study_identifiers = deserializedDetails?.study_identifiers;
            
            return JsonSerializer.Serialize(summary);
        }
        catch (Exception e)
        {
            string s = e.Message;
            return string.Empty;
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

    private async Task<IEnumerable<string>?> GetIEnumerable(string sql_string)
    {
        using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            var res = await conn.QueryAsync<string>(sql_string);
            if (!res.Any())
            {
                return res;
            }

            var ids = "(";
            for (int i = 0; i < res.Count(); i++)
            {
                var s = JsonSerializer.Deserialize<StudySummary>(res.ElementAt(i));
                ids += s.study_id.ToString();
                if (i != (res.Count() - 1))
                {
                    ids += ", ";
                }
                else
                {
                    ids += ")";
                }
            }
            
            var details = await FetchStudyDetailsByIds(ids);
            var deserializedDetails = details.Select(d => JsonSerializer.Deserialize<JSONFullStudy>(d)).ToList();
            var summariesAsString = new List<string>();
            foreach (var r in res)
            {
                var summary = JsonSerializer.Deserialize<StudySummary>(r);
                var info = deserializedDetails.FirstOrDefault(x => x.id == summary.study_id);
                summary.study_identifiers = info == null ? new List<study_identifier>() : info.study_identifiers;
                summariesAsString.Add(JsonSerializer.Serialize(summary));
            }

            return summariesAsString;
        }
        catch (Exception e)
        {
            string s = e.Message;
            return null;
        }
    }

    public async Task<IEnumerable<string>?> FetchStudyByRegId(string reg_Id)
    {
        var sql_string = $@"select study_json from search.idents
                              where ident_value = '{reg_Id}'";
        return await GetIEnumerable(sql_string);
    }

    public async Task<IEnumerable<string>?> FetchStudyByTypeAndId(int type_id, string reg_Id)
    {
        var sql_string = $@"select study_json from search.idents
                              where ident_type = {type_id} and ident_value = '{reg_Id}'";
        return await GetIEnumerable(sql_string);
    }


    public async Task<IEnumerable<string>?> FetchStudiesByPMID(int pmid)
    {
        var sql_string = $@"select study_json from search.pmids
                              where pmid = {pmid}";
        return await GetIEnumerable(sql_string);
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
        return await GetIEnumerable(sql_string);
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

        return await GetIEnumerable(sql_string);
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
        return await GetIEnumerable(sql_string);
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
        return await GetSingleRecord(sql_string);

    }


    public async Task<string?> FetchStudyDetailsById(int study_id)
    {
        // for testing 2044457 = TNT trial

        string sql_study = @$"select full_study
                           from search.studies_json s
                           where s.id = {study_id}";

        return await GetSingleRecord<string>(sql_study);
    }
    
    public async Task<(int count, IEnumerable<string>? res)> GetStudiesByCountriesListAsync(IList<string> countries, 
        int pageSize, int pageNumber)
    {
        var countriesFromDb = await _lookUpRepo.FetchCountries();
        if (countriesFromDb.Count() == 0)
        {
            throw new InvalidOperationException(
                "Can not get countries from DB to compare with the countries list from the query");
        }
        
        var countriesIds = "(";
        for (int i = 0; i < countries.Count(); i++)
        {
            var countryName = countries[i].Trim();
            var countryFromDb = countriesFromDb.FirstOrDefault(x => x.name!.Equals(countryName));
            countriesIds += countryFromDb.id;
            if (i != (countries.Count() - 1))
            {
                countriesIds += ", ";
            }
            else
            {
                countriesIds += ")";
            }
        }

        var sql_total_numbers = @$"select count(*)
                           from search.countries s
                           where s.country_id in {countriesIds}";

        var r = await GetIEnumerable<int>(sql_total_numbers);
        
        var offset = CalculateOffset(pageNumber, pageSize);
        
        string sql_study = @$"select distinct study_id
                           from search.countries s
                           where s.country_id in {countriesIds}
                           order by study_id limit {pageSize} offset {offset}";

        var res = await GetIEnumerable<int>(sql_study);

        var resList = res.ToList();
        
        var ids = "(";
        for (var k = 0; k < resList.Count(); k++)
        {
            ids += resList[k];
            if (k != (res.Count() - 1))
            {
                ids += ", ";
            }
            else
            {
                ids += ")";
            }
        }

        var paginatedResult = await FetchStudyDetailsByIds(ids);
        
        return (r.FirstOrDefault(0), paginatedResult);
    }
    
    public async Task<(int count, IEnumerable<Dictionary<string, string>>? res)> GetStudyIdsByCountriesListAsync(IList<string> countries, 
        int pageSize, int pageNumber)
    {
        var countriesFromDb = await _lookUpRepo.FetchCountries();
        if (countriesFromDb.Count() == 0)
        {
            throw new InvalidOperationException(
                "Can not get countries from DB to compare with the countries list from the query");
        }
        
        var countriesIds = "(";
        for (int i = 0; i < countries.Count(); i++)
        {
            var countryName = countries[i].Trim();
            var countryFromDb = countriesFromDb.FirstOrDefault(x => x.name!.Equals(countryName));
            countriesIds += countryFromDb.id;
            if (i != (countries.Count() - 1))
            {
                countriesIds += ", ";
            }
            else
            {
                countriesIds += ")";
            }
        }

        var sql_total_numbers = @$"select count(*)
                           from search.countries s
                           where s.country_id in {countriesIds}";

        var r = await GetIEnumerable<int>(sql_total_numbers);
        
        var offset = CalculateOffset(pageNumber, pageSize);
        
        string sql_study = @$"select study_id, ident_value from search.idents
                           where study_id in (select study_id from search.countries where country_id in {countriesIds})
                           order by study_id limit {pageSize} offset {offset}";

        var res = await GetIEnumerable<object>(sql_study);

        var resList = res.ToList();

        var studyResult = new List<Dictionary<string, string>>();

        foreach (var studyRecord in resList)
        {
            var dictValue = new Dictionary<string, string>();
            if (studyRecord is not ICollection<KeyValuePair<string, object>> castValue) continue;
            dictValue.Add("mdr_study_id", castValue.First().Value.ToString());
            dictValue.Add("identifier_value", castValue.Last().Value.ToString());
            
            studyResult.Add(dictValue);
        }

        // var outputResult = new List<string>();
        // outputResult.Add(JsonSerializer.Serialize(studyResult));
        
        return (r.FirstOrDefault(0), studyResult);
    }

    public async Task<(int count, IEnumerable<string>? res)> GetStudiesByCountryAndStartYearAndType(IList<string> countries, 
        int startYearFrom, int startYearTo, string studyType,int pageSize, int pageNumber)
    {
        var countriesFromDb = await _lookUpRepo.FetchCountries();
        if (countriesFromDb.Count() == 0)
        {
            throw new InvalidOperationException(
                "Can not get countries from DB to compare with the countries list from the query");
        }
        
        var countriesIds = "(";
        for (int i = 0; i < countries.Count(); i++)
        {
            var countryName = countries[i].Trim();
            var countryFromDb = countriesFromDb.FirstOrDefault(x => x.name!.Equals(countryName));
            countriesIds += countryFromDb.id;
            if (i != (countries.Count() - 1))
            {
                countriesIds += ", ";
            }
            else
            {
                countriesIds += ")";
            }
        }        
        var offset = CalculateOffset(pageNumber, pageSize);
        int study_type = 0;
        switch (studyType.ToUpper()) 
        {
            case "INTERVENTIONAL":
                study_type = 11;
                break;
            case "OBSERVATIONAL":
                study_type = 12;
                break;
            case "PATIENT_R":
                study_type = 13;
                break;
            case "EXPANDED_A":
                study_type = 14;
                break;
            case "FUNDED_P":
                study_type = 15;
                break;
            case "OTHER":
                study_type = 16;
                break;
        }
        string s = "";
        if (startYearTo == 0)
        {
            s = $"s.study_start_year = {startYearFrom}";
        } else
        {
            s = $"s.study_start_year between {startYearFrom} and {startYearTo}";
        }
        var sql_total_numbers = @$"select count(*)
                                from core.studies s inner join core.study_countries sc on s.id = sc.study_id 
                                where s.study_type_id = {study_type} and {s} and sc.country_id in {countriesIds}";

        var r = await GetIEnumerable<int>(sql_total_numbers);

        string sql_study = @$"select distinct s.id from core.studies s inner join core.study_countries sc on s.id = sc.study_id 
                            where s.study_type_id = {study_type} and {s} and sc.country_id in {countriesIds}
                            order by s.id limit {pageSize} offset {offset}";

        var res = await GetIEnumerable<int>(sql_study);

        var resList = res.ToList();
        
        var ids = "(";
        for (var k = 0; k < resList.Count(); k++)
        {
            ids += resList[k];
            if (k != (res.Count() - 1))
            {
                ids += ", ";
            }
            else
            {
                ids += ")";
            }
        }

        var paginatedResult = await FetchStudyDetailsByIds(ids);
        
        return (r.FirstOrDefault(0), paginatedResult);
    }

    public async Task<IDictionary<string, long>> GetTotalStudiesAndObjectsAsync()
    {
        using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            var sqlString = $"select count(*) from core.studies union select count(*) from core.data_objects";
            var res = await conn.QueryAsync<int>(sqlString);
            return new Dictionary<string, long>()
            {
                {"studiesCount", res.First()},
                {"objectsCount", res.Last()}
            };
        }
        catch (Exception e)
        {
            var s = e.Message;
            return new Dictionary<string, long>()
            {
                {"studiesCount", 0},
                {"objectsCount", 0}
            };
        }
    }

    private async Task<IEnumerable<string>?> FetchStudyDetailsByIds(string ids)
    {
        // for testing 2044457 = TNT trial

        string sql_study = @$"select full_study
                           from search.studies_json s
                           where s.id in {ids}";

        return await GetIEnumerable<string>(sql_study);
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
            IEnumerable<string>? object_data = await GetIEnumerable(sql_objects);
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
                IEnumerable<string>? res = (await GetIEnumerable(sql_string));

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
                        // Check if the entry has a 'location' field
                        // If it does, split it by commas and create multiple 'location' fields
                        // If it does not, just append the entry as is
                        var location = (string)entry.Descendants("field")
                            .FirstOrDefault(x => (string)x.Attribute("name") == "location");
                        if (string.IsNullOrWhiteSpace(location))
                        {
                            // If location is null or empty, just append the entry as is
                            updEntriesSb.Append(entry);
                        }
                        else
                        {
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

    public async Task<int> GetTotalStudiesCount()
    {
        using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            var sqlString = $"select count(*) from core.studies";
            var res = await conn.QueryAsync<int>(sqlString);
            return res.First();
        }
        catch (Exception e)
        {
            var s = e.Message;
            return 0;
        }
    }

    public async Task<IDictionary<string, long>> GetStudyCountByStudyType()
    {
        var data = new Dictionary<string, long>();
        using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.Append("select");
            sqlQuery.Append("(select count(id) from core.studies where study_type_id = 11) as Interventional,");
            sqlQuery.Append("(select count(id) from core.studies where study_type_id = 12) as Observational,");
            sqlQuery.Append("(select count(id) from core.studies where study_type_id = 13) as Patient_R,");
            sqlQuery.Append("(select count(id) from core.studies where study_type_id = 14) as Expanded_A,");
            sqlQuery.Append("(select count(id) from core.studies where study_type_id = 15) as Funded_P,");
            sqlQuery.Append("(select count(id) from core.studies where study_type_id = 16) as Other");
            
            var res = await conn.QueryAsync<object>(sqlQuery.ToString());
            foreach (var r in (IEnumerable)res.First())
            {
                if (r is KeyValuePair<string, object> entry)
                {
                    switch (entry.Key.ToUpper())
                    {
                        case "INTERVENTIONAL":
                            data.Add("Interventional", (long)entry.Value);
                            break;
                        case "OBSERVATIONAL":
                            data.Add("Observational", (long)entry.Value);
                            break;
                        case "PATIENT_R":
                            data.Add("Patient registry", (long)entry.Value);
                            break;
                        case "EXPANDED_A":
                            data.Add("Expanded access", (long)entry.Value);
                            break;
                        case "FUNDED_P":
                            data.Add("Funded programme", (long)entry.Value);
                            break;
                        case "OTHER":
                            data.Add("Other", (long)entry.Value);
                            break;
                    }
                }
            }
            
            return data;
        }
        catch (Exception e)
        {
            var s = e.Message;
            return null;
        }
    }

    public async Task<IDictionary<string, long>> GetStudyCountByStudyStartYear()
    {
        var data = new Dictionary<string, long>();
        using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.Append("select study_start_year, count(*) from core.studies group by study_start_year");
            
            var res = await conn.QueryAsync<object>(sqlQuery.ToString());
            List<string> dataYear = new List<string>();
            List<long> dataCount = new List<long>();
            long yearNotKnown = 0;
            if (res is IEnumerable<object> r)
            {
                foreach(IDictionary<string, object> r1 in r) {
                    foreach(var r2 in r1) {
                        if(r2 is KeyValuePair<string, object> entry)
                        {
                            if (entry.Key.ToUpper().Equals("STUDY_START_YEAR"))
                            {
                                int year = Convert.ToInt32(entry.Value);
                                if (entry.Value == null || year > 2025)
                                {
                                    dataYear.Add("Others");
                                }
                                else 
                                {
                                    string val = entry.Value.ToString();
                                    dataYear.Add((string)val);
                                }
                            }
                            if (entry.Key.ToUpper().Equals("COUNT"))
                            {
                                dataCount.Add((long)entry.Value);
                            }

                        }
                    }
                }
                for (var i = 0; i < dataYear.Count; i++)
                {
                    if (dataYear[i].Contains("Others")) {
                        yearNotKnown = yearNotKnown + dataCount[i];
                    } else {
                        data.Add(dataYear[i], dataCount[i]);
                    }
                }
                data.Add("0000", (long)yearNotKnown);
            }            
            return data;
        }
        catch (Exception e)
        {
            var s = e.Message;
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
    
    
    private static int CalculateOffset(int pageNumber, int pageSize)
    {
        var startingIndex = (pageNumber - 1) * pageSize;
        if (startingIndex == 1 && pageSize == 1)
        {
            return 0;
        }
        
        return startingIndex;
    }

    /// <inheritdoc />
    private static int CalculateTotalPages(int totalRecords, int pageSize)
    {
        return (int)Math.Ceiling((double)(totalRecords / pageSize));
    }
}