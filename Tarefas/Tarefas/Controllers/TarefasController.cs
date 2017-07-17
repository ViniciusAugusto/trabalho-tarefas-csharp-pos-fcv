using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tarefas.Extended;
using Tarefas.Hubs;
using Tarefas.Models;

namespace Tarefas.Controllers
{
    [System.Web.Mvc.Authorize]
    public class TarefasController : Controller
    {
        private ContextoDB db = new ContextoDB();

        public ActionResult Index()
        {
            if (!TemTime())
            {
                return RedirectToAction("Index", "Times");
            }

            return View();
        }

        public ActionResult Cadastro(int? id)
        {
            if (!TemTime())
            {
                return RedirectToAction("Index", "Times");
            }

            var usuarioLogado = User.GetNickName();

            var tarefaModel = new TarefaModel();
            var times = db.Times.Where(w => w.Dono == usuarioLogado);

            if (id != null && id > 0)
            {
                tarefaModel = db.Tarefas.Find(id);
            }
            else
            {
                tarefaModel.Criador = usuarioLogado;
                tarefaModel.Responsavel = tarefaModel.Criador;
                tarefaModel.DataCriacao = DateTime.Now;
                tarefaModel.TimeModelId = times.First().Id;
            }


            var sql = $@"
                SELECT u.* FROM UsuariosTimesModel ut
                JOIN UsuariosTarefasModel u ON ut.UsuariosTarefasModelId = u.Id
                WHERE ut.TimeModelId = {tarefaModel.TimeModelId}";

            var usuariosTime = db.Database.SqlQuery<UsuariosTarefasModel>(sql).ToList();
            usuariosTime.Insert(0, new UsuariosTarefasModel {
                Apelido = "Nenhum Responsável"
            });

            ViewBag.Responsavel = new SelectList(usuariosTime, "Apelido", "Apelido", tarefaModel.Responsavel);
            ViewBag.TimeModelId = new SelectList(times, "Id", "Nome", tarefaModel.TimeModelId);

            return View(tarefaModel);
        }

        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastro([Bind(Include = "Id,Titulo,Descricao,Criador,Responsavel,DataCriacao,DataConclusao,DataAgendamento,TimeModelId")] TarefaModel tarefaModel)
        {
            if (ModelState.IsValid)
            {
                if (tarefaModel.Id > 0)
                {
                    db.Entry(tarefaModel).State = EntityState.Modified;
                }
                else
                {
                    // garantimos que o criador é realmente a pessoa que está logada
                    tarefaModel.Criador = User.GetNickName();

                    db.Tarefas.Add(tarefaModel);
                }

                db.SaveChanges();

                if (!string.IsNullOrWhiteSpace(tarefaModel.Responsavel) && tarefaModel.Responsavel != User.GetNickName())
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.All.indicadoPraUmaTarefa(tarefaModel.Responsavel, tarefaModel.Titulo, tarefaModel.Criador);
                }

                return RedirectToAction("Index");
            }
            ViewBag.TimeModelId = new SelectList(db.Times, "Id", "Nome", tarefaModel.TimeModelId);
            return View(tarefaModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            var excluiu = true;
            var msg = string.Empty;

            try
            {
                TarefaModel tarefasModel = db.Tarefas.Find(id);
                db.Tarefas.Remove(tarefasModel);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                msg = string.IsNullOrWhiteSpace(ex.Message)
                            ? ex.InnerException.Message
                            : ex.Message;

                excluiu = false;
            }

            return Json(new
            {
                excluiu,
                msg
            });
        }

        public ActionResult FiltrarTarefas(bool concluidas = false)
        {
            var tarefas = db.Tarefas
                .Where(w => w.DataConclusao.HasValue == concluidas)
                .ToList()
                .Select(s => new
                {
                    s.Id,
                    s.Titulo,
                    s.Criador,
                    Responsavel = string.IsNullOrWhiteSpace(s.Responsavel) ? string.Empty : s.Responsavel,
                    DataCriacao = s.DataCriacao.ToString("dd/MM/yyyy"),
                    DataAgendamento = s.DataAgendamento.HasValue ? s.DataAgendamento.Value.ToString("dd/MM/yyyy") : "",
                    DataConclusao = s.DataConclusao.HasValue ? s.DataConclusao.Value.ToString("dd/MM/yyyy") : "",
                    s.Time.Nome
                });

            return Json(tarefas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarUsuarios(int id)
        {
            var dono = User.GetNickName();

            var sql = $@"
                SELECT u.* FROM UsuariosTimesModel ut
                JOIN UsuariosTarefasModel u ON ut.UsuariosTarefasModelId = u.Id
                WHERE ut.TimeModelId = {id}";

            var usuarios = db.Database
                .SqlQuery<UsuariosTarefasModel>(sql)
                .Select(s => s.Apelido)
                .ToList();

            usuarios.Insert(0, dono);

            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConcluirTarefa(int id, bool concluido)
        {
            var concluiu = true;
            var msg = string.Empty;

            try
            {
                // o parametro "concluido" informa se a tarefa já foi concluída, sendo assim
                // a tarefa deverá ser reaberta
                var tarefaModel = db.Tarefas.Find(id);

                if (concluido)
                {
                    tarefaModel.DataConclusao = null;
                }
                else
                {
                    tarefaModel.DataConclusao = DateTime.Now;
                }

                db.Entry(tarefaModel).State = EntityState.Modified;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                msg = string.IsNullOrWhiteSpace(ex.Message)
                            ? ex.InnerException.Message
                            : ex.Message;

                concluiu = false;
            }

            return Json(new
            {
                concluiu,
                msg
            });
        }

        private bool TemTime()
        {
            var dono = User.GetNickName();
            return db.Times.Where(w => w.Dono == dono).Count() > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
