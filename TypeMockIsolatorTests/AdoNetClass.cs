using System.Data.SqlClient;

namespace TypeMockIsolatorTests
{
    internal class AdoNetClass
    {
        private string connectionString;

        public AdoNetClass(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Execute()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select 'artur', 'ilin', '1991-05-08' into #persons";
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
