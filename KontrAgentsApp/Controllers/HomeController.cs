using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontrAgentsApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Список контрагентов";

            return View();
        }

        public ActionResult Import()
        {
            ViewBag.Title = "Импорт контрагентов";

            return View();
        }
    }
}
