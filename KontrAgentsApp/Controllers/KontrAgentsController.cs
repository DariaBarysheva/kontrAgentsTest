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
    //контроллер для главной страницы
    public class KontrAgentsController : ApiController
    {
        KontrAgentRepository repo = new KontrAgentRepository();

        //получение всех контрагентов из БД
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
        /*[HttpPost]
        public IHttpActionResult GetKontrAgentByInnName(int id, [FromBody]KontrAgent kontrAgent)*/
        //требуется указать ограничение, какой маршрут применим к данному методу; иначе будет ошибка при вызове GET api/kontragents
        [Route("api/kontragents/getkontragentbyinnname")]
        public IHttpActionResult GetKontrAgentByInnName([FromUri]KontrAgent kontrAgent)
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
    }
}
