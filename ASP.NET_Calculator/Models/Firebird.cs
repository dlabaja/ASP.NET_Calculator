using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace ASP.NET_Calculator.Models
{
    public static class Firebird
    {
        public static readonly string ConnectionString = $@"Server=localhost;Port=3050;Database=C:\Users\dlabajaj\Downloads\ASP.NET_Calculator\ASP.NET_Calculator\DB\results.fdb;User=SYSDBA;Password=masterkey;";

        public static async Task InsertResult(string uid, double result)
        {
            using (var con = new FbConnection(ConnectionString))
            {
                await con.OpenAsync();
                const string query =
                    @"INSERT INTO ""RESULTS"" (""UID"" ,""Result"", ""ResultTime"") VALUES (@Value1, @Value2, @Value3);";
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

        public static async Task<int> GetValueCount(string field, string value)
        {
            using (var con = new FbConnection(ConnectionString))
            {
                await con.OpenAsync();
                var query = $@"SELECT COUNT(*) FROM ""RESULTS"" WHERE ""{field}"" = @Value;";
                int count;
                using (var command = new FbCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Value", value);
                    count = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                await con.CloseAsync();
                return count;
            }
        }

        public static async Task<List<ResultObject>> GetResults(string uid)
        {
            var results = new List<ResultObject>();

            using (var con = new FbConnection(ConnectionString))
            {
                await con.OpenAsync();
                const string query = @"SELECT FIRST 5 * FROM ""RESULTS"" WHERE ""UID"" = @Value ORDER BY ""INDEX"" DESC;";
                using (var command = new FbCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Value", uid);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new ResultObject(double.Parse(reader["Result"].ToString()), reader["ResultTime"].ToString()));
                        }
                    }
                }

                await con.CloseAsync();
            }

            return results;
        }

        public struct ResultObject
        {
            public double result;
            public string resultTime;

            public ResultObject(double result, string resultTime)
            {
                this.result = result;
                this.resultTime = resultTime;
            }
        }
    }
}