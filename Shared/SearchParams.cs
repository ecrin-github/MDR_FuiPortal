namespace MDR_FuiPortal.Shared
{
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


    public class FilterParams
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
}
