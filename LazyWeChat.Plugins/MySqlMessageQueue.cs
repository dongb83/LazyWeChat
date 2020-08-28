using LazyWeChat.Abstract;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace LazyWeChat.Plugins
{
    public class MySqlMessageQueue : IMessageQueue
    {
        private readonly string _connectionString;
        private static readonly object thisLock = new object();

        public MySqlMessageQueue(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> Pop()
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var commandText = "select top 1 * from records where status = 1 order by createdAt desc";
                var command = new MySqlCommand(commandText, conn);
                var reader = await command.ExecuteReaderAsync();
                var id = "";
                var message = "";
                if (reader.Read())
                {
                    id = reader["id"].ToString();
                    message = reader["messageObject"].ToString();
                }
                commandText = "update records set status = 0 where id = @id";
                command = new MySqlCommand(commandText, conn);
                command.Parameters.AddWithValue("id", id);
                _ = command.ExecuteNonQueryAsync();
                return message;
            }
        }

        public async Task Push(string item)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                var commandText = "insert into records(id, messageObject, createdAt, status)values(uuid(), @messageObject, now(), 1)";
                var command = new MySqlCommand(commandText, conn);
                command.Parameters.AddWithValue("messageObject", item);

                _ = await command.ExecuteNonQueryAsync();
            }
        }
    }
}
