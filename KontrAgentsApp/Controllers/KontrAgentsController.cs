using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KontrAgentsApp.Models;
using System.Data;
using System.Web;
using System.Collections.Specialized;

namespace KontrAgentsApp.Controllers
{
    public class KontrAgentsController : ApiController
    {
        KontrAgentRepository repo = new KontrAgentRepository();

        //полчение всех контрагентов из БД
        public IHttpActionResult GetKontrAgents()
        {
            try
            {
                IEnumerable<KontrAgent> tempList = repo.GetKontrAgents();
                return Ok(tempList);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //поиск контрагента по идентификатору
        public IHttpActionResult GetKontrAgent(int id)
        {
            try
            {
                KontrAgent kontrAgent = repo.Get(id);
                return Ok(kontrAgent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //поиск контрагента по ИНН и названию
        [HttpPost]
        public IHttpActionResult GetKontrAgentByInnName(int id, [FromBody]KontrAgent kontrAgent)
        {
            try
            {
                KontrAgent foundKontrAgent = repo.FindByInnName(kontrAgent.Inn, kontrAgent.Name, kontrAgent.Id);
                return Ok(foundKontrAgent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //добавление нового контрагента
        [HttpPost]
        public IHttpActionResult CreateKontrAgent([FromBody]KontrAgent kontrAgent)
        {
            try
            {
                repo.Create(kontrAgent);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //обновление данных контрагента
        [HttpPut]
        public IHttpActionResult EditKontrAgent(int id, [FromBody]KontrAgent kontrAgent)
        {
            try
            {
                if (id == kontrAgent.Id)
                {
                    repo.Update(kontrAgent);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //удаление контрагента по идентификатору
        public IHttpActionResult DeleteKontrAgent(int id)
        {
            try
            {
                repo.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        /*public ActionResult Index()
        {
            return View(repo.GetKontrAgents());
        }

        public ActionResult Details(int id)
        {
            KontrAgent kontrAgent = repo.Get(id);
            if (kontrAgent != null)
                return View(kontrAgent);
            return HttpNotFound();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(KontrAgent kontrAgent)
        {
            repo.Create(kontrAgent);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            KontrAgent kontrAgent = repo.Get(id);
            if (kontrAgent != null)
                return View(kontrAgent);
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Edit(KontrAgent kontrAgent)
        {
            repo.Update(kontrAgent);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            KontrAgent kontrAgent = repo.Get(id);
            if (kontrAgent != null)
                return View(kontrAgent);
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            repo.Delete(id);
            return RedirectToAction("Index");
        }*/

    }
}
