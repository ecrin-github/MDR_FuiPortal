using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MDR_FuiPortal.Shared
{

    public class SearchParams
    {
        public int type { get; set; }
        public int scope { get; set; }
        public string? pars { get; set; }
        public FilterParams? fp { get; set; }
        public string? fb_string { get; set; }

        public SearchParams()
        { }

        public SearchParams(int _type, int _scope, string _pars)
        {
            type = _type;
            scope = _scope;
            pars = _pars;
        }

        public SearchParams(int _type, int _scope, string _pars, FilterParams _fp)
        {
            type = _type;
            scope = _scope;
            pars = _pars;
            fp = _fp;
        }

        public string ObtainFBString()
        {
            string fb = "";
            if (type == 1)
            {
                fb = "Using Text";
                string scope_string =  scope switch
                {
                    1 => "titles and topics",
                    2 => "conditions",
                    3 => "titles, topics and conditions",
                    _ => ""
                };

                fb += ", searching in " + scope_string;
                fb += ", with parameters ‘" + pars + "’";

                if (fp is null || string.IsNullOrEmpty(fp.pars_list))
                {
                    fb += ", no filters applied";
                }
                else
                {
                    fb += ", with ";

                    if (fp.pars_list.Contains('T') && !string.IsNullOrEmpty(fp.study_type))
                    {
                        if (fp.study_type.StartsWith("type_id in ("))  // it should!
                        {
                            string Filters = fp.study_type[12..].Trim(')', ' ');
                            string[] selected = Filters.Split(',', StringSplitOptions.TrimEntries);
                            string study_type = " Study type ";
                            study_type += selected.Length > 1 ? "one of " : "= ";
                            for (int i = 0; i < selected.Length; i++)
                            {
                                switch (selected[i])
                                {
                                    case ("11"): study_type += "Interventional, "; break;
                                    case ("12"): study_type += "Observational, "; break;
                                    case ("13"): study_type += "Patient registry, "; break;
                                    case ("14"): study_type += "Expanded access, "; break;
                                    case ("15"): study_type += "Funded programme, "; break;
                                    case ("16"): study_type += "Other, "; break;
                                }
                            }
                            fb += study_type[..^2] + ";";
                        }
                    }

                    if (fp.pars_list.Contains('S') && !string.IsNullOrEmpty(fp.study_status))
                    {
                        if (fp.study_status.StartsWith("status_id in ("))  // it should!
                        {
                            string Filters = fp.study_status[14..].Trim(')', ' ');
                            string[] selected = Filters.Split(',', StringSplitOptions.TrimEntries);
                            string study_status = " Study status ";
                            study_status += selected.Length > 1 ? "one of " : "= ";
                            for (int i = 0; i < selected.Length; i++)
                            {
                                switch (selected[i])
                                {
                                    case ("16"): study_status += "Not yet recruiting, "; break;
                                    case ("14"): study_status += "Recruiting, "; break;
                                    case ("15"): study_status += "Active, not recruiting, "; break;
                                    case ("25"): study_status += "Ongoing, "; break;
                                    case ("21"): study_status += "Completed, "; break;
                                    case ("11"): study_status += "Withdrawn, "; break;
                                    case ("18"): study_status += "Suspended, "; break;
                                    case ("22"): study_status += "Terminated, "; break;
                                    case ("30"): study_status += "Other, "; break;
                                }
                            }
                            fb += study_status[..^2] + ";";
                        }
                    }

                    if (fp.pars_list.Contains('Y') && !string.IsNullOrEmpty(fp.start_year))
                    {
                        string? sy = " Start year " + fp.start_year
                            .Replace("lt", "<").Replace("gt", ">")
                            .Replace("eq", "=").Replace("bw", "between")
                            .Replace (",", " ").Replace("  ", " ");
                        if (sy.Contains("between"))
                        {
                            int last_space = sy.LastIndexOf(' ');
                            sy = sy[..last_space] + " and " + sy[(last_space + 1)..];
                        }
                        fb += sy + ";";
                    }

                    if (fp.pars_list.Contains('C') && !string.IsNullOrEmpty(fp.country_names))
                    {
                        string? cy = " Country ";
                        string[] selected = fp.country_names.Split(',');
                        cy += selected.Length > 1 ? "one of " : "= ";
                        cy += fp.country_names;

                        fb += cy + ";";
                    }

                    if (fp.pars_list.Contains('B') && !string.IsNullOrEmpty(fp.objects))
                    {
                        if (fp.objects.StartsWith(" ("))  // it should!
                        {
                            string Filters = fp.objects[2..].Trim(')', ' ');
                            string[] selected = Filters.Split(',', StringSplitOptions.TrimEntries);
                            string linked_obs = " Linked object";
                            linked_obs += selected.Length > 1 ? "s one of " : " = ";
                            for (int i = 0; i < selected.Length; i++)
                            {
                                switch (selected[i])
                                {
                                    case ("12"): linked_obs += "Registry results summary, "; break;
                                    case ("21"): linked_obs += "Journal article, "; break;
                                    case ("22"): linked_obs += "Study protocol, "; break;
                                    case ("23"): linked_obs += "Study overview, "; break;
                                    case ("24"): linked_obs += "Patient consent/IS, "; break;
                                    case ("25"): linked_obs += "Data collection forms, "; break;
                                    case ("26"): linked_obs += "Manual of procedures, "; break;
                                    case ("31"): linked_obs += "SAP, "; break;
                                    case ("32"): linked_obs += "Clinical study report, "; break;
                                    case ("33"): linked_obs += "Data summary, "; break;
                                    case ("34"): linked_obs += "IPD / Data, "; break;
                                    case ("36"): linked_obs += "Other study resource, "; break;
                                    case ("41"): linked_obs += "Other info. resource, "; break;
                                    case ("51"): linked_obs += "Website, "; break;
                                    case ("52"): linked_obs += "Software, "; break;
                                    case ("53"): linked_obs += "Samples description, "; break;
                                    case ("99"): linked_obs += "Other, "; break;
                                }
                            }
                            fb += linked_obs[..^2] + ";";
                        }
                    }

                    if (fp.pars_list.Contains('P') && !string.IsNullOrEmpty(fp.phase))
                    {
                        if (fp.phase.StartsWith("phase_id in ("))  // it should!
                        {
                            string Filters = fp.phase[13..].Trim(')', ' ');
                            string[] selected = Filters.Split(',', StringSplitOptions.TrimEntries);
                            string phase = " Phase ";
                            phase += selected.Length > 1 ? "one of " : "= ";
                            for (int i = 0; i < selected.Length; i++)
                            {
                                switch (selected[i])
                                {
                                    case ("105"): phase += "Early phase, "; break;
                                    case ("110"): phase += "Phase 1, "; break;
                                    case ("115"): phase += "Phase 1/2, "; break;
                                    case ("120"): phase += "Phase 2, "; break;
                                    case ("125"): phase += "Phase 2/3, "; break;
                                    case ("130"): phase += "Phase 3, "; break;
                                    case ("135"): phase += "Phase 4, "; break;
                                    case ("100"): phase += "Not applicable, "; break;
                                    case ("140"): phase += "Not provided, "; break;
                                }
                            }
                            fb += phase[..^2] + ";";
                        }
                    }

                    if (fp.pars_list.Contains('A') && !string.IsNullOrEmpty(fp.alloc))
                    {
                        if (fp.alloc.StartsWith("alloc_id in ("))  // it should!
                        {
                            string Filters = fp.alloc[13..].Trim(')', ' ');
                            string[] selected = Filters.Split(',', StringSplitOptions.TrimEntries);
                            string allocation = " Allocation ";
                            allocation += selected.Length > 1 ? "one of " : "= ";
                            for (int i = 0; i < selected.Length; i++)
                            {
                                switch (selected[i])
                                {
                                    case ("205"): allocation += "Randomised, "; break;
                                    case ("210"): allocation += "Nonrandomised, "; break;
                                    case ("200"): allocation += "Not applicable, "; break;
                                    case ("215"): allocation += "Not provided, "; break;
                                }
                            }
                            fb += allocation[..^2] + ";";
                        }
                    }

                }
            }

            if (type == 2)
            {
                fb = "Using Study identifier, ";
                fb += "type " + scope + ", ";
                fb += pars;
            }

            if (type == 3)
            {
                fb = "Using PubMed Id, ";
                fb += pars;
            }

            fb_string = fb;
            return fb;
       }
    }


    public class FilterParams
    {
        public string pars_list { get; set; } = "";
        public string? study_type { get; set; }
        public string? study_status { get; set; }
        public string? start_year { get; set; }
        public string? countries { get; set; }
        public string? country_names { get; set; }
        public string? objects { get; set; }
        public string? phase { get; set; }
        public string? alloc { get; set; }
    }


    /*
    public class SearchParamsPage     
    {
        public int scope { get; set; }
        public string? pars { get; set; }
        public int page_start { get; set; }
        public int page_size { get; set; }
        public bool count_only { get; set; }
        public FilterParams? fp { get; set; }

        public SearchParamsPage()
        { }

        public SearchParamsPage(int _scope, string _pars, int _page_start, int _page_size)
        {
            scope = _scope;
            pars = _pars;
            page_start = _page_start;
            page_size = _page_size;
        }

        public SearchParamsPage(int _scope, string _pars, int _page_start, int _page_size, FilterParams _fp)
        {
            scope = _scope;
            pars = _pars;
            page_start = _page_start;
            page_size = _page_size;
            fp = _fp;
        }
    }

    */

}
