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

        public static async Task InsertResult(string uid, string expression, double result)
        {
            using (var con = new FbConnection(ConnectionString))
            {
                await con.OpenAsync();
                const string query =
                    @"INSERT INTO ""RESULTS"" (""UID"", ""Expression"", ""Result"", ""DateTime"") VALUES (@Value1, @Value2, @Value3, @Value4);";
                using (var command = new FbCommand(query, con))
                {
                    command.Parameters.AddWithValue("@Value1", uid);
                    command.Parameters.AddWithValue("@Value2", expression);
                    command.Parameters.AddWithValue("@Value3", result);
                    command.Parameters.AddWithValue("@Value4", DateTime.Now);
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
                            results.Add(new ResultObject(reader["Expression"].ToString(), reader.GetDouble(reader.GetOrdinal("Result")), reader["DateTime"].ToString()));
                        }
                    }
                }

                await con.CloseAsync();
            }

            return results;
        }

        public struct ResultObject
        {
            public string expression;
            public double result;
            public string dateTime;

            public ResultObject(string expression, double result, string dateTime)
            {
                this.expression = expression;
                this.result = result;
                this.dateTime = dateTime;
            }
        }
    }
}