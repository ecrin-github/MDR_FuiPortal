using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDR_FuiPortal.Shared;


public class database
{
    public string name { get; set; } = "ECRIN MDR";
    public string description { get; set; } = "The MDR aggregates metadata describing clinical studies and the data objects associated with them. It derives its data from trial registries, bibliographic resources and data repositories.";

    public string release { get; set; } = "";
    public string release_date { get; set; } = "";
    public int entry_count { get; set; } = 0;
    public string keywords { get; set; } = "clinical research, clinical trials, interventional studies, cohort studies, observational health research";

    public List<entry> entries { get; set; } = new();
}


public class entry
{
    public int? id { get; set; }
    public string? name { get; set; }
    public string? description { get; set; }

    public List<dbref>? cross_references { get; set; }
    public List<date>? dates { get; set; }
    public List<field>? additional_fields { get; set; }
}


public class dbref
{
    public string? dbname { get; set; }
    public string? dbkey { get; set; } 
}

public class date
{
    public string? type { get; set; }
    public string? value { get; set; }
}

public class field
{
    public string? name { get; set; }
    public string? value { get; set; }
}
