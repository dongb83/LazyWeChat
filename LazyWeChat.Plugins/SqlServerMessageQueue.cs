using LazyWeChat.Abstract;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace LazyWeChat.Plugins
{
    public class SqlServerMessageQueue : IMessageQueue
    {
        private readonly string _connectionString;

        public SqlServerMessageQueue(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> Pop()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var commandText = "select top 1 * from records where status = 1 order by createdAt desc";
                var command = new SqlCommand(commandText, conn);
                var reader = await command.ExecuteReaderAsync();
                var id = "";
                var message = "";
                if (reader.Read())
                {
                    id = reader["id"].ToString();
                    message = reader["messageObject"].ToString();
                }
                commandText = "update records set status = 0 where id = @id";
                command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("id", id);
                _ = command.ExecuteNonQueryAsync();
                return message;
            }
        }

        public async Task Push(string item)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var commandText = "insert into records(messageObject)values(@messageObject)";
                var command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("messageObject", item);

                _ = await command.ExecuteNonQueryAsync();
            }
        }
    }
}
