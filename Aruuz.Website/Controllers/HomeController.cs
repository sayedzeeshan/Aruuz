using Aruuz.Controllers;
using Aruuz.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Aruuz.Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new Input());
            //return RedirectToAction("Error", "Home");
        }

        public ActionResult Error()
        {
            return View();
            //return RedirectToAction("Error", "Home");
        }

        public ActionResult API()
        {
            return View();
            //return RedirectToAction("Error", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Input data)
        {
            if (!String.IsNullOrEmpty(data.text))
            {
                Input input = new Input();
                input.text = data.text;
                input.isChecked = data.isChecked;
                Session["inp"] = input;
                return RedirectToAction("Result", "Taqti");
            }
            return View(1);
        }
      

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Test()
        {
            ViewBag.Message = "Test page.";

            return View();
        }
    }
}