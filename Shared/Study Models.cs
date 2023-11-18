namespace MDR_FuiPortal.Shared;

public class StudyJson
{
    string? study_json { get; set; }
}


public class StudySummary
{
    public int study_id { get; set; }
    public string? study_name { get; set; }
    public string? description { get; set; }
    public string? dss { get; set; }
    public int? start_year { get; set; }
    public int? start_month { get; set; }
    public int? type_id { get; set; }
    public string? type_name { get; set; }
    public int? status_id { get; set; }
    public string? status_name { get; set; }
    public string? gender_elig { get; set; }
    public string? min_age { get; set; }
    public string? max_age { get; set; }
    public int? phase_id { get; set; }
    public int? alloc_id { get; set; }
    public string? feature_list { get; set; }
    public string? has_objects { get; set; }
    public string? country_list { get; set; }
    public string? condition_list { get; set; }
    public string? provenance { get; set; }
    public List<ObjectSummary>? objects { get; set; }
}


public class IECLine
{

}


public class JSONFullStudyData
{
    public JSONFullStudy? full_study { get; set; }
    public JSONFullObject[]? full_objects { get; set; }
}



public class JSONFullStudy
{
    public string? file_type { get; set; }
    public int id { get; set; }
    public string? display_title { get; set; }
    public string? brief_description { get; set; }
    public string? data_sharing_statement { get; set; }
    public int? study_start_year { get; set; }
    public int? study_start_month { get; set; }
    public Lookup? study_type { get; set; }
    public Lookup? study_status { get; set; }
    public string? study_enrolment { get; set; }
    public Lookup? study_gender_elig { get; set; }
    public age_param? min_age { get; set; }
    public age_param? max_age { get; set; }
    public string? provenance_string { get; set; }

    public List<study_identifier>? study_identifiers { get; set; }
    public List<study_title>? study_titles { get; set; }
    public List<study_person>? study_people { get; set; }
    public List<study_organisation>? study_organisations { get; set; }
    public List<study_topic>? study_topics { get; set; }
    public List<study_feature>? study_features { get; set; }
    public List<study_condition>? study_conditions { get; set; }
    public List<study_icd>? study_icds { get; set; }
    public List<study_country>? study_countries { get; set; }
    public List<study_location>? study_locations { get; set; }
    public List<study_relationship>? study_relationships { get; set; }
    public List<int>? linked_data_objects { get; set; }
}


public class age_param
{
    public int? value { get; set; }
    public int? unit_id { get; set; }
    public string? unit_name { get; set; }
}

public class study_identifier
{
    public int id { get; set; }
    public string? identifier_value { get; set; }
    public Lookup? identifier_type { get; set; }
    public Organisation? source { get; set; }
    public string? identifier_date { get; set; }
    public string? identifier_link { get; set; }
}

public class study_title
{
    public int id { get; set; }
    public Lookup? title_type { get; set; }
    public string? title_text { get; set; }
    public string? lang_code { get; set; }
    public string? comments { get; set; }
}


public class study_person
{
    public int id { get; set; }
    public Lookup? contrib_type { get; set; }
    public string? person_full_name { get; set; }
    public string? orcid_id { get; set; }
    public string? person_affiliation { get; set; }
    public Organisation? affiliation_org { get; set; }
}


public class study_organisation
{
    public int id { get; set; }
    public Lookup? contrib_type { get; set; }
    public Organisation? org_details { get; set; }
}


public class study_topic
{
    public int id { get; set; }
    public Lookup? topic_type { get; set; }
    public string? original_value { get; set; }
    public CTData? ct_data { get; set; }
    public MeshData? mesh_data { get; set; }
}


public class study_condition
{
    public int id { get; set; }
    public string? original_value { get; set; }
    public CTData? ct_data { get; set; }
}

public class study_icd
{
    public int id { get; set; }
    public ICDData? icd_data { get; set; }
}

public class study_feature
{
    public int id { get; set; }
    public Lookup? feature_type { get; set; }
    public Lookup? feature_value { get; set; }
}


public class study_country
{
    public int id { get; set; }
    public int? country_id { get; set; }
    public string? country_name { get; set; }
    public Lookup? status { get; set; }
}


public class study_location
{
    public int id { get; set; }
    public Organisation? facility { get; set; }
    public int? city_id { get; set; }
    public string? city_name { get; set; }
    public int? country_id { get; set; }
    public string? country_name { get; set; }
    public Lookup? status { get; set; }
}


public class study_relationship
{
    public int id { get; set; }
    public Lookup? relationship_type { get; set; }
    public int? target_study_id { get; set; }
}


public class studyIEC
{
    public int id { get; set; }

}









