using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MapOnlyExample.Service;

namespace MapOnlyExample.WebDemo.Controllers
{
    public class HomeController : Controller
    {
        private UserService userService;
        public HomeController()
        {
            userService = new UserService();;
        }
        public ActionResult Index()
        {
            var users = userService.GetUserListing(string.Empty);
            return View(users);
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
    }
}