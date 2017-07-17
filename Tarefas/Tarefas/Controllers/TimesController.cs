using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tarefas.Extended;
using Tarefas.Models;

namespace Tarefas.Controllers
{
    public class TimesController : Controller
    {
        private ContextoDB db = new ContextoDB();

        public ActionResult Entrar()
        {
            var usuarioTarefaModel = new UsuariosTarefasModel
            {
                NomeCompleto = User.GetFullName(),
                Apelido = User.GetNickName(),
                Email = User.GetEmail()
            };

            db.Usuarios.Add(usuarioTarefaModel);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            var apelido = User.GetNickName();

            var times = db.Times.Where(w => w.Dono == apelido).ToList();

            return View(times);
        }

        public ActionResult Cadastro(int? id)
        {
            var timeModel = new TimeModel();

            // seta o dono do time
            timeModel.Dono = User.GetNickName();

            if (id != null || id > 0)
            {
                timeModel = db.Times.Find(id);

                if (timeModel == null)
                {
                    return HttpNotFound();
                }
            }

            return View(timeModel);
        }

        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastro([Bind(Include = "Id,Nome,Dono")] TimeModel timeModel)
        {
            if (ModelState.IsValid)
            {
                // garante o nome do time, caso tenha sido alterado na tela.
                timeModel.Dono = User.GetNickName();
                var novo = timeModel.Id == 0;

                if (!novo)
                {
                    db.Entry(timeModel).State = EntityState.Modified;
                }
                else
                {
                    db.Times.Add(timeModel);
                }

                db.SaveChanges();

                if (novo)
                {
                    AddOrDeleteUsuarioTime(timeModel.Id, db.Usuarios.First(f => f.Apelido == timeModel.Dono).Id);
                }

                return RedirectToAction("Index");
            }
            return View(timeModel);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            var excluiu = true;
            var msg = string.Empty;

            using (var transac = db.Database.BeginTransaction())
            {
                try
                {
                    var sqlExclusaoTarefas = $@"DELETE FROM TarefaModel WHERE TimeModelId = {id}";
                    var sqlExclusaoUsuarios = $@"DELETE FROM UsuariosTimesModel WHERE TimeModelId = {id}";

                    TimeModel timeModel = db.Times.Find(id);
                    db.Times.Remove(timeModel);

                    db.Database.ExecuteSqlCommand(sqlExclusaoTarefas);
                    db.Database.ExecuteSqlCommand(sqlExclusaoUsuarios);

                    db.SaveChanges();

                    transac.Commit();
                }
                catch (Exception ex)
                {
                    msg = string.IsNullOrWhiteSpace(ex.Message)
                                ? ex.InnerException.Message
                                : ex.Message;

                    excluiu = false;

                    transac.Rollback();
                }
            }

            return Json(new { excluiu, msg });
        }

        #region Gerenciamento de Usuários
        public ActionResult UsuariosTime(int id)
        {
            var usuarios = UsuariosPorTime(id);

            ViewBag.Time = id;

            return View(usuarios);
        }

        public ActionResult BuscarUsuarios(int id)
        {
            ViewBag.Time = id;

            return View();
        }

        public ActionResult PesquisaUsuario(string apelido)
        {
            var dono = User.GetNickName();

            var sql = string.Format($@"SELECT * FROM UsuariosTarefasModel WHERE Apelido LIKE '%{apelido}%' AND Apelido NOT LIKE '{dono}'");

            var usuarios = db.Database.SqlQuery<UsuariosTarefasModel>(sql).ToList();

            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CarregarUsuariosDoTime(int id)
        {
            var usuarios = UsuariosPorTime(id).Select(s => new { s.Apelido });

            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUsuarioTime(int id, int idUsuario)
        {
            var gravou = true;
            var msg = string.Empty;

            if (id > 0 && idUsuario > 0)
            {
                try
                {
                    AddOrDeleteUsuarioTime(id, idUsuario);
                }
                catch (Exception ex)
                {
                    msg = string.IsNullOrWhiteSpace(ex.Message)
                            ? ex.InnerException.Message
                            : ex.Message;

                    gravou = false;
                }

                return Json(new { gravou, msg });
            }

            return Json(new { gravou = false, msg = "Parâmetros incorretos!" });
        }

        private void AddOrDeleteUsuarioTime(int timeId, int usuarioId)
        {
            var sql = string.Format($@"
IF EXISTS (SELECT * FROM UsuariosTimesModel WHERE TimeModelId = {timeId} AND UsuariosTarefasModelId = {usuarioId})
BEGIN
    DELETE UsuariosTimesModel WHERE TimeModelId = {timeId} AND UsuariosTarefasModelId = {usuarioId}
END
ELSE
BEGIN
    INSERT INTO UsuariosTimesModel VALUES ({usuarioId}, {timeId})
END
");

            db.Database.ExecuteSqlCommand(sql);
        }

        private List<UsuariosTarefasModel> UsuariosPorTime(int id)
        {
            var sql = $@"
                SELECT u.* FROM UsuariosTimesModel ut
                JOIN UsuariosTarefasModel u ON ut.UsuariosTarefasModelId = u.Id
                WHERE ut.TimeModelId = {id}";

            return db.Database.SqlQuery<UsuariosTarefasModel>(sql).ToList();
        }
        #endregion

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
