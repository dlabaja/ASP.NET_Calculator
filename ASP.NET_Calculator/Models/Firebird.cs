using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace ASP.NET_Calculator.Models
{
    public class Firebird
    {
        private static string connectionString = @"User=SYSDBA;Password=masterkey;Database=C:\fb\examples\empbuild\employee.fdb;Dialect=3;Charset=UTF8;ServerType=1;ClientLibrary=C:\fb\fbclient.dll";

        public async static Task InsertResult(string uid, double result)
        {
            using (var con = new FbConnection(connectionString))
            {
                await con.OpenAsync();
                var query = @"INSERT INTO ""RESULTS"" (""UID"" ,""Result"", ""ResultTime"") VALUES (@Value1, @Value2, @Value3);";
                using (var command = new FbCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Value1", uid);
                    command.Parameters.AddWithValue("@Value2", result);
                    command.Parameters.AddWithValue("@Value3", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                    await command.ExecuteNonQueryAsync();
                }
                await con.CloseAsync();
            }
        }

        public async static Task<int> GetValueCount(string field, string value)
        {
            var count = 0;
            using (var con = new FbConnection(connectionString))
            {
                await con.OpenAsync();
                var query = $@"SELECT COUNT(*) FROM ""RESULTS"" WHERE ""{field}"" = @Value;";
                using (var command = new FbCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Value", value);
                    count = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
                await con.CloseAsync();
                return count;
            }
        }

        public async static Task<Dictionary<double, string>> GetResults(string uid)
        {
            var results = new Dictionary<double, string>();

            using (var con = new FbConnection(connectionString))
            {
                await con.OpenAsync();
                var query = @"SELECT FIRST 5 * FROM ""RESULTS"" WHERE ""UID"" = @Value";
                using (var command = new FbCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Value", uid);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(double.Parse(reader["Result"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture), reader["ResultTime"].ToString());
                        }
                    }
                }

                await con.CloseAsync();
            }

            return results;
        }
    }
}