using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KontrAgentsApp.Models;
using System.Data;

namespace KontrAgentsApp.Controllers
{
    public class KontrAgentsController : ApiController
    {
        KontrAgentRepository repo = new KontrAgentRepository();

        public IEnumerable<KontrAgent> GetKontrAgents()
        {
            return repo.GetKontrAgents();
        }

        public KontrAgent GetKontrAgent(int id)
        {
            KontrAgent kontrAgent = repo.Get(id);
            return kontrAgent;
        }

        [HttpPost]
        public void CreateKontrAgent([FromBody]KontrAgent kontrAgent)
        {
            repo.Create(kontrAgent);
        }

        [HttpPut]
        public void EditKontrAgent(int id, [FromBody]KontrAgent kontrAgent)
        {
            if (id == kontrAgent.Id)
            {
                repo.Update(kontrAgent);
            }
        }

        public void DeleteKontrAgent(int id)
        {
            repo.Delete(id);
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
