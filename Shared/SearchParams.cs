namespace MDR_FuiPortal.Shared
{
    /*
    public class SearchParams
    {
        public int scope { get; set; }
        public string? pars { get; set; }
        public int bucket { get; set; }
        public bool count_only { get; set; }
        public FilterParams? fp { get; set; }

        public SearchParams()
        { }

        public SearchParams(int _scope, string _pars, int _bucket, bool _count_only)
        {
            scope = _scope;
            pars = _pars;
            bucket = _bucket;
            count_only = _count_only;
        }

        public SearchParams(int _scope, string _pars, int _bucket, bool _count_only, FilterParams _fp)
        {
            scope = _scope;
            pars = _pars;
            bucket = _bucket;
            count_only = _count_only;
            fp = _fp;
        }
    }


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



    public class OldFilterParams
    {
        public bool has_any { get; set; } = false;
        public int study_type_num { get; set; } = 0;
        public int study_status_num { get; set; } = 0;
        public int start_year_num { get; set; } = 0;
        public int countries_num { get; set; } = 0;
        public int objects_num { get; set; } = 0;
        public int phase_num { get; set; } = 0;
        public int alloc_num { get; set; } = 0;
        public string? study_type { get; set; }
        public string? study_status { get; set; }
        public string? start_year { get; set; }
        public string? countries { get; set; }
        public string? objects { get; set; }
        public string? phase { get; set; }
        public string? alloc { get; set; }
    }
    */

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
}
