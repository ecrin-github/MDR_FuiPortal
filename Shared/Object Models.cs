/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
*/

namespace MDR_FuiPortal.Shared
{

    public class JSONFullObject
    {
        public string? file_type { get; set; }
        public int id { get; set; }
        public string? doi { get; set; }
        public string? display_title { get; set; }
        public string? version { get; set; }
        public Lookup? object_class { get; set; }
        public Lookup? object_type { get; set; }
        public int? publication_year { get; set; }
        public Organisation? managing_organisation { get; set; }
        public string? lang_code { get; set; }
        public Lookup? access_type { get; set; }
        public object_access? access_details { get; set; }
        public int? eosc_category { get; set; }
        public string? provenance_string { get; set; }

        public record_keys? dataset_record_keys { get; set; }
        public Deidentification? dataset_deident_level { get; set; }
        public Consent? dataset_consent { get; set; }

        public List<object_instance>? object_instances { get; set; }
        public List<object_title>? object_titles { get; set; }
        public List<object_person>? object_people { get; set; }
        public List<object_organisation>? object_organisations { get; set; }
        public List<object_date>? object_dates { get; set; }
        public List<object_topic>? object_topics { get; set; }
        public List<object_description>? object_descriptions { get; set; }
        public List<object_identifier>? object_identifiers { get; set; }
        public List<object_right>? object_rights { get; set; }
        public List<object_relationship>? object_relationships { get; set; }
        public List<int>? linked_studies { get; set; }
    }


    public class object_access
    {
        public string? description { get; set; }
        public string? url { get; set; }
        public string? url_last_checked { get; set; }
    }

    public class record_keys
    {
        public int? record_keys_type_id { get; set; }
        public string? record_keys_type { get; set; }
        public string? record_keys_details { get; set; }
    }

    public class Deidentification
    {
        public int? deident_type_id { get; set; }
        public string? deident_type { get; set; }
        public bool? deident_direct { get; set; }
        public bool? deident_hipaa { get; set; }
        public bool? deident_dates { get; set; }
        public bool? deident_nonarr { get; set; }
        public bool? deident_kanon { get; set; }
        public string? deident_details { get; set; }
    }

    public class Consent
    {
        public int? consent_type_id { get; set; }
        public string? consent_type { get; set; }
        public bool? consent_noncommercial { get; set; }
        public bool? consent_geog_restrict { get; set; }
        public bool? consent_research_type { get; set; }
        public bool? consent_genetic_only { get; set; }
        public bool? consent_no_methods { get; set; }
        public string? consent_details { get; set; }
    }

    public class object_instance
    {
        public int id { get; set; }
        public Lookup? system { get; set; }
        public access_details? access_details { get; set; }
        public resource_details? resource_details { get; set; }
    }

    public class access_details
    {
        public string? url { get; set; }
        public bool? direct_access { get; set; }
        public string? url_last_checked { get; set; }
    }


    public class resource_details
    {
        public int? type_id { get; set; }
        public string? type_name { get; set; }
        public float? size { get; set; }
        public string? size_unit { get; set; }
        public string? comments { get; set; }
    }

    public class object_title
    {
        public int id { get; set; }
        public Lookup? title_type { get; set; }
        public string? title_text { get; set; }
        public string? lang_code { get; set; }
        public string? comments { get; set; }
    }

    public class object_date
    {
        public int id { get; set; }
        public Lookup? date_type { get; set; }
        public bool? date_is_range { get; set; }
        public string? date_as_string { get; set; }
        public sdate_as_ints? start_date { get; set; }
        public edate_as_ints? end_date { get; set; }
        public string? comments { get; set; }
    }

    public class sdate_as_ints
    {
        public int? start_year { get; set; }
        public int? start_month { get; set; }
        public int? start_day { get; set; }
    }

    public class edate_as_ints
    {
        public int? end_year { get; set; }
        public int? end_month { get; set; }
        public int? end_day { get; set; }
    }

    public class object_topic
    {
        public int id { get; set; }
        public Lookup? topic_type { get; set; }
        public string? original_value { get; set; }
        public CTData? ct_data { get; set; }
        public MeshData? mesh_data { get; set; }
    }

    public class object_person
    {
        public int id { get; set; }
        public Lookup? contrib_type { get; set; }
        public string? person_full_name { get; set; }
        public string? orcid_id { get; set; }
        public string? person_affiliation { get; set; }
        public Organisation? affiliation_org { get; set; }
    }

    public class object_organisation
    {
        public int id { get; set; }
        public Lookup? contrib_type { get; set; }
        public Organisation? org_details { get; set; }
    }

    public class object_description
    {
        public int id { get; set; }
        public Lookup? description_type { get; set; }
        public string? description_label { get; set; }
        public string? description_text { get; set; }
        public string? lang_code { get; set; }
    }

    public class object_identifier
    {
        public int id { get; set; }
        public string? identifier_value { get; set; }
        public Lookup? identifier_type { get; set; }
        public Organisation? source { get; set; }
        public string? identifier_date { get; set; }
    }

    public class object_right
    {
        public int id { get; set; }
        public string? rights_name { get; set; }
        public string? rights_uri { get; set; }
        public string? comments { get; set; }
    }

    public class object_relationship
    {
        public int id { get; set; }
        public Lookup? relationship_type { get; set; }
        public int? target_object_id { get; set; }
    }

}
