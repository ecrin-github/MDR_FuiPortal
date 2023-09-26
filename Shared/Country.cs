using System;

namespace MDR_FuiPortal.Shared;

public class Country
{
    // As used chiefly within the country dropdown
    // in the select Study Location input box

    public int id { get; set; }
    public string? name { get; set; }

    public Country()
    {}

    public Country(int _id, string _name)
    {
        id = _id;
        name = _name;
    }
}

