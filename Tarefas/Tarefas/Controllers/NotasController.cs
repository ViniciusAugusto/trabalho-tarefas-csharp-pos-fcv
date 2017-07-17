using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tarefas.Extended;
using Tarefas.Hubs;
using Tarefas.Models;

namespace Tarefas.Controllers
{
    public class NotasController : Controller
    {

        private ContextoDB db = new ContextoDB();

        // GET: Notas
        public ActionResult Index()
        {
            var notas = db.Notas;

            return View(notas);
        }

        // GET: Notas/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Notas/Create
        public ActionResult Create(int? id)
        {
            var notasModel = new NotasModel();

            // seta o dono do time
            notasModel.Usuario = User.GetNickName();

            if (id != null || id > 0)
            {
                notasModel = db.Notas.Find(id);

                if (notasModel == null)
                {
                    return HttpNotFound();
                }
            }

            return View(notasModel);
        }

        // POST: Notas/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Texto,dataHora,Usuario")] NotasModel notasModel)
        {
            if (ModelState.IsValid)
            {
                notasModel.Usuario = User.GetNickName();
                var novo = notasModel.Id == 0;
                if (!novo)
                {
                    db.Entry(notasModel).State = EntityState.Modified;
                }
                else
                {
                    db.Notas.Add(notasModel);
                }

                db.SaveChanges();

                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.All.novaNotaCadastrada(notasModel.Usuario, notasModel.Texto, notasModel.dataHora);

                return RedirectToAction("Index");
            }
            return View(notasModel);
        }


        [HttpPost]
        public ActionResult Excluir(int id)
        {
            var excluiu = true;
            var msg = string.Empty;
            
            try
            {
                NotasModel notasModel = db.Notas.Find(id);
                db.Notas.Remove(notasModel);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                msg = string.IsNullOrWhiteSpace(ex.Message)
                            ? ex.InnerException.Message
                            : ex.Message;

                excluiu = false;
            }
            

            return Json(new { excluiu, msg });
        }
    }
}
