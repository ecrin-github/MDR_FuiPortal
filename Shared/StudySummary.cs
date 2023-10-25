using System;
using System.Diagnostics.CodeAnalysis;

namespace MDR_FuiPortal.Shared;

public class StudyJson
{
    string? study_json { get; set; }
}


public class StudySearchResult
{


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

 
public class ObjectSummary
{
    public int oid { get; set; }
    public string? ob_name { get; set; }
    public int? typeid { get; set; }
    public string? typename { get; set; }
    public string? url { get; set; }
    public int? res_type_id { get; set; }
    public string? res_icon { get; set; }
    public string? year_pub { get; set; }
    public string? acc_icon { get; set; }
    public string? prov { get; set; }
}


public class DisplayStudy
{
    public int study_id { get; set; }
    public string study_name { get; set; } = null!;
    public string short_title { get; set; } = null!;
    public string description { get; set; } = null!;
    public string dss { get; set; } = null!;
    public string start_date { get; set; } = null!;
    public string type_name { get; set; } = null!;
    public string status_name { get; set; } = null!;
    public string gender_elig { get; set; } = null!;
    public string min_age { get; set; } = null!;
    public string max_age { get; set; } = null!;
    public string feature_list { get; set; } = null!;
    public string has_objects { get; set; } = null!;
    public string country_list { get; set; } = null!;
    public string condition_list { get; set; } = null!;
    public string provenance { get; set; } = null!;
    public List<ObjectSummary>? objects { get; set; }



}

public class StudyDetails
{
    int Id { get; set; }
    string? Name { get; set; }
    string? Description { get; set; }
    string? DataSharingStatement { get; set; }
    int TypeId { get; set; }
    int StatusId { get; set; }
    int StartYear { get; set; }
    int StartMonth { get; set; }
    string? MinAge { get; set; }
    string? MaxAge { get; set; }
    string? GenderEligibility { get; set; }
    string? Provenance { get; set; }

    List<string>? StudyIdentifiers { get; set; }
    List<string>? Titles { get; set; }
    List<string>? Conditions { get; set; }
    List<string>? Topics { get; set; }
    List<string>? Features { get; set; }
    List<string>? Countries { get; set; }
    List<string>? StudyPeople { get; set; }
    List<string>? StudyOrganisations { get; set; }
    List<string>? StudyRelationships { get; set; }
    List<ObjectSummary>? ObjectDetails { get; set; }

    // Inclusion / exclusion criteria - availabe as button
}


public class ObjectDetails
{
    int Id { get; set; }
    int Type { get; set; }
    string? Name { get; set; }
    string? Url { get; set; }
    string? AccessDetails { get; set; }
    string? ManagingOrganisation { get; set; }
    string? ResourceSystem { get; set; }
    string? ResourceDetails { get; set; }
    int? PublishedType { get; set; }
    string? Provenance { get; set; }
    int access_type { get; set; }

    List<string>? Dates { get; set; }
    List<string>? ObjectIdentifiers { get; set; }
    List<string>? ObjectPeople { get; set; }
    List<string>? ObjectOrganisations { get; set; }
}


public class IECLine
{

}

