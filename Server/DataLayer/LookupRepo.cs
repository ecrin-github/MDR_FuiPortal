using MDR_FuiPortal.Shared;
using Npgsql;
using Dapper.Contrib;
using Dapper;

namespace MDR_FuiPortal.Server;

public class LookUpRepo : ILookUpRepo
{
    private readonly string _dbConnString;

    public LookUpRepo(ICredentials creds)
    {
        _dbConnString = creds.GetConnectionString("context");
    }

    public async Task<IEnumerable<Country>?> FetchCountries()
    {
        var sqlString = @"select geoname_id as id, country_name as name
                           from ctx.countries
                           where rank = 1";
        using var conn = new NpgsqlConnection(_dbConnString);

        try
        {
            var res = await conn.QueryAsync<Country>(sqlString);
            return res;
        }
        catch(Exception e)
        {
            string s = e.Message;
            return null;
        }

    }
}

