namespace MDR_FuiPortal.Shared;

public class DialogData
{
    public SearchParams? SearchParams { get; set; }
    public IList<string> PageSet { get; set; } = new List<string>();
    public IList<string> BaseSet { get; set; } = new List<string>();
    public IList<string> BucketSet { get; set; } = new List<string>();
}

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsLiving { get; set; }
}