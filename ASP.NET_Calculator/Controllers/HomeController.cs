using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ASP.NET_Calculator.Models;

namespace ASP.NET_Calculator.Controllers
{
    public class HomeController : Controller
    {
        private string uid;

        public async Task<ActionResult> Index()
        {
            if (await EstablishSession() == null) return RedirectToAction("Index");
            return View();
        }

        [Route("[controller]")]
        public async Task<ActionResult> Calculate()
        {
            var result = new PostfixCalculator().CalculatePostfix(
                new PostfixCalculator().InfixToPostfix(new StringBuilder(Request.QueryString["val"])));
            uid = await EstablishSession();
            await Firebird.InsertResult(uid, result);
            var results = await Firebird.GetResults(uid);
            var data = new { Result = result, Results = results };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private async Task<string> EstablishSession()
        {
            string uid = HttpContext.Request.Cookies["UID"]?.Value;

            if (string.IsNullOrEmpty(uid))
            {
                var cookie = new HttpCookie("UID", Convert.ToString(await GenerateUID()));
                Response.Cookies.Add(cookie);
                return null;
            }

            return uid;
        }

        private async Task<int> GenerateUID()
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