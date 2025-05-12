using System.Data.SqlClient;

namespace BattachApp.Models
{
    public class DBConnect
    {
        public readonly IConfiguration _config;
        public SqlConnection conn;
        public DBConnect() { }
        public DBConnect(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection GetDbConnection()
        {
            return conn = new SqlConnection(_config != null ? _config["ConnectionStrings:Sql"] : ""); 
        }

    }
}
