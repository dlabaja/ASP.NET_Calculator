using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PostfixCalculator;

namespace WebApplication4.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		[Route("[controller]")]
		public ActionResult Calculate()
		{
			return Content(new PostfixCalculator.PostfixCalculator().CalculatePostfix(
				new PostfixCalculator.PostfixCalculator().InfixToPostfix(new StringBuilder(Request.QueryString["val"]))
				).ToString());
		}
	}
}