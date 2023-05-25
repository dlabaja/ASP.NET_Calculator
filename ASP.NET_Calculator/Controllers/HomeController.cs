using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ASP.NET_Calculator.Models;

namespace ASP.NET_Calculator.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            if (await GetUID() == null) return RedirectToAction("Index");
            return View();
        }

        [Route("[controller]")]
        public async Task<ActionResult> Calculate()
        {
            var result = new PostfixCalculator().CalculatePostfix(
                new PostfixCalculator().InfixToPostfix(new StringBuilder(Request.QueryString["val"])));
            await Firebird.InsertResult(await GetUID(), result);
            return Content(result.ToString());
        }

        [Route("[controller]")]
        public async Task<ActionResult> LastResults()
        {
            var uid = await GetUID();
            var results = await Firebird.GetResults(uid);
            return Json(new { Results = results }, JsonRequestBehavior.AllowGet);
        }

        public async Task<string> GetUID()
        {
            var uid = HttpContext.Request.Cookies["UID"]?.Value;
            if (string.IsNullOrEmpty(uid))
            {
                uid = Convert.ToString(await SessionManager.GenerateUID());
                Response.Cookies.Add(new HttpCookie("UID", uid));
                return null;
            }

            return uid;
        }
    }
}