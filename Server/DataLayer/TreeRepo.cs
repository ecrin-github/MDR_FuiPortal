using Dapper;
using MDR_FuiPortal.Shared;
using Npgsql;

namespace MDR_FuiPortal.Server;

public class TreeRepo : ITreeRepo
{
    private readonly string _dbConnString;

    public TreeRepo(ICredentials creds)
    {
        _dbConnString = creds.GetConnectionString("mdr");
    }

    public async Task<IEnumerable<TreeLine>?> FetchTreeItems(string TreeName)
    {
        var sql_string = $@"select level as Level, tree_id as Id, title as Title, 
                            is_parent as IsParent, is_closed as IsClosed 
                            from tree.pages 
                            where tree = '{TreeName}'
                            order by tree_seq";

        await using var conn = new NpgsqlConnection(_dbConnString);
        try
        {
            var res = await conn.QueryAsync<TreeLine>(sql_string);
            return res;

        }
        catch (Exception e)
        {
            string s = e.Message;
            return null;
        }
    }

    public async Task<page_info?> GetPageContent(string tree_id)
    {
        await using var conn = new NpgsqlConnection(_dbConnString);
        string sql_string = $@"select page_header, last_edited
                            from tree.pages
	                        where tree_id = '{tree_id}'";
        page_info_header? header = await conn.QuerySingleOrDefaultAsync<page_info_header>(sql_string);

        if (header is null)
        {
            return null;
        }
        page_info pi = new(header.page_header, header.last_edited);

        sql_string = $@"select seq_num, type, parameters, content
                        from tree.content 
	                    where tree_page_id = '{tree_id}'
	                    order by seq_num";

        List<info_component> res = (await conn.QueryAsync<info_component>(sql_string)).ToList();
        if (res.Any())
        {
            pi.info_dyncs = res;
            return pi;
        }
        return null; // as fallback
    }
}
