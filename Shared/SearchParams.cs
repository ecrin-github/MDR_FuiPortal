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
                    if (fp.pars_list.Contains('T'))
                    {
                        fb += fp.study_type + ", ";
                    }
                    if (fp.pars_list.Contains('S'))
                    {
                        fb += fp.study_status + ", ";
                    }
                    if (fp.pars_list.Contains('Y'))
                    {
                        string? sy = "start year " + fp.start_year?
                            .Replace("lt", "<").Replace("gt", ">")
                            .Replace("eq", "=").Replace("bw", "between")
                            .Replace (",", " ").Replace("  ", " ");
                        fb += sy + ", ";
                    }
                    if (fp.pars_list.Contains('C'))
                    {
                        fb += " countries in " + fp.countries + ", ";
                    }
                    if (fp.pars_list.Contains('B'))
                    {
                        fb += " objects in " + fp.objects + ", ";
                    }
                    if (fp.pars_list.Contains('P'))
                    {
                        fb += fp.phase + ", ";
                    }
                    if (fp.pars_list.Contains('A'))
                    {
                        fb += fp.alloc + ", ";
                    }
                    fb = fb[..^2];
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
