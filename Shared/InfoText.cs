namespace MDR_FuiPortal.Shared;

public class page_info_header
{
    public string? page_header { get; set; }
    public string? last_edited { get; set; }
}

public class page_info
{
    public string? page_header { get; set; }
    public string? last_edited { get; set; }
    public List<info_component>? info_dyncs { get; set; }

    public page_info(string? _page_header, string? _last_edited)
    {
        page_header = _page_header;
        last_edited = _last_edited;
    }

    public page_info()
    { }
}

public class info_component
{
    public int seq_num { get; set; }
    public string? type { get; set; }
    public string? parameters { get; set; }
    public string? content { get; set; }

    public info_component(int _seq_num, string _type, string? _parameters, string? _content)
    {
        seq_num = _seq_num;
        type = _type;
        parameters = _parameters;
        content = _content;
    }

    public info_component()
    { }
}

public class TreeLine
{
    // Data representing individual tree-lines 
    // in the About and Guide pages

    public string? Id { get; set; }
    public string? Title { get; set; }
    public int Level { get; set; }
    public bool? IsParent { get; set; }
    public bool? IsClosed { get; set; }
}

/*
public class PageContent
{
    public string? Content { get; set; }

    public PageContent(string? _Content)
    {
        Content = _Content;
    }

    public PageContent()
    { }
}
*/
