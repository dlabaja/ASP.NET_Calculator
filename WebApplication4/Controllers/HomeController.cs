using System;
using System.Collections.Generic;
using System.Linq;
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
			return Content(new PostfixCalculator.PostfixCalculator().Calculate(
				new PostfixCalculator.PostfixCalculator().InfixToPostfix(Request.QueryString["val"])
				).ToString());
		}
	}
}