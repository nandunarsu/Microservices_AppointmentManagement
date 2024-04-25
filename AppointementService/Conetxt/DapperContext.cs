using System.Data;
using System.Data.SqlClient;

namespace AppointementService.Conetxt
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;

        private readonly string _connection;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = _configuration.GetConnectionString("AppointmentConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connection);
    }
}
