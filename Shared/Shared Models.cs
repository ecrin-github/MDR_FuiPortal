namespace MDR_FuiPortal.Shared
{
    public class Lookup
    {
        public int? id { get; set; }
        public string? name { get; set; }
    }

    // these three small composite classes also used within the data model, for topic and condition data

    public class MeshData
    {
        public string? mesh_code { get; set; }
        public string? mesh_value { get; set; }
    }

    public class ICDData
    {
        public string? icd_code { get; set; }
        public string? icd_name { get; set; }
    }


    public class CTData
    {
        public int? ct_type_id { get; set; }
        public string? ct_type { get; set; }
        public string? ct_code { get; set; }
    }

    public class Organisation
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? ror_id { get; set; }
    }

}
