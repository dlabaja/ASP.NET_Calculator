using System;
using FirebirdSql.Data.FirebirdClient;

namespace ASP.NET_Calculator.Models
{
    public class Firebird
    {
        public static void SetupConnection()
        {
            var con = new FbConnection(
                @"User=SYSDBA;Password=masterkey;Database=C:\Firebird_4_0\fb\examples\empbuild\employee.fdb;Dialect=3;Charset=UTF8;ServerType=1;");
            con.Open();
        }
    }
}