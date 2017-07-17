using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tarefas.Extended;
using Tarefas.Models;

namespace Tarefas.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var usaTarefas = false;

            if (User.Identity.IsAuthenticated)
            {
                var db = new ContextoDB();

                var apelido = User.GetNickName();

                usaTarefas = db.Usuarios.Where(w => w.Apelido == apelido).Count() > 0; 
            }

            ViewBag.UsaTarefas = usaTarefas;

            return View();
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