using Npgsql;

namespace MDR_FuiPortal.Server;

public class Credentials : ICredentials
{
    private readonly string _host;
    private readonly string _username;
    private readonly string _password;
    private readonly int _port;

    public Credentials(IConfiguration settings)
    {
        // all asserted as non-null

        _host = settings["mdr_host"]!;
        _username = settings["mdr_user"]!;
        _password = settings["mdr_password"]!;
        string? PortAsString = settings["port"];
        if (string.IsNullOrWhiteSpace(PortAsString))
        {
            _port = 5432;  // default
        }
        else
        {
            _port = int.TryParse(PortAsString, out int port_num) ? port_num : 5432;
        }
    }

    public string GetConnectionString(string database_name)
    {
        NpgsqlConnectionStringBuilder builder = new()
        {
            Host = _host,
            Username = _username,
            Password = _password,
            Port = _port,
            Database = database_name,
            IncludeErrorDetail = true
        };
        return builder.ConnectionString;
    }
}


/*
private readonly Dictionary<string, Cred> _creds = new Dictionary<string, Cred>();

    public Creds(IConfiguration settings)
    {
        _creds.Add("mdr", new Cred
        {
            host_name = settings["mdr_host"],
            user_name = settings["mdr_user"],
            pass_word = settings["mdr_password"],
            db_name = "mdr"
        }); ;

        _creds.Add("context", new Cred
        {
            host_name = settings["mdr_host"],
            user_name = settings["mdr_user"],
            pass_word = settings["mdr_password"],
            db_name = "context"
        });

        _creds.Add("aggs", new Cred
        {
            host_name = settings["mdr_host"],
            user_name = settings["mdr_user"],
            pass_word = settings["mdr_password"],
            db_name = "aggs"
        });
    }

    public string GetConnectionString(string db_name)
    {
        Cred c = _creds[db_name];
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = c.host_name,
            Username = c.user_name,
            Password = c.pass_word,
            Database = c.db_name
        };
        return builder.ConnectionString;
    }

    private class Cred
    {
        public string? host_name { get; init; }
        public string? user_name { get; init; }
        public string? pass_word { get; init; }
        public string? db_name { get; init; }
    }
}
*/


