using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ASP.NET_Calculator.Models
{
    public class SessionManager
    {
        public static async Task<int> GenerateUID()
        {
            var uid = new Random().Next(int.MaxValue);
            while (await Firebird.GetValueCount("UID", uid.ToString()) != 0)
            {
                uid = new Random().Next(int.MaxValue);
            }
            return uid;
        }
    }
}