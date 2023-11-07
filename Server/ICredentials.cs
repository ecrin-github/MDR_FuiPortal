namespace MDR_FuiPortal.Server;

public interface ICredentials
{
    string GetConnectionString(string db_name);
}
