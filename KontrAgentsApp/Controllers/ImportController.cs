using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using KontrAgentsApp.Models;

namespace KontrAgentsApp.Controllers
{
    public class ImportController : ApiController
    {
        KontrAgentRepository repo = new KontrAgentRepository(); 

        public async Task<IHttpActionResult> Post()
        {
            string line; //для хранения одной строки, прочитанной из файла
            List<KontrAgent> listKontrAgents = new List<KontrAgent>(); ; //для хранения списка контрагентов, прочитанных из файла
            KontrAgent tempKontrAgent = null; //для хранения текущего контрагента, прочитанного из файла

            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return BadRequest();
                }
                var provider = new MultipartMemoryStreamProvider();

                //путь к папке Files на сервере для хранения временного файла, получаемого с клиента
                string root = System.Web.HttpContext.Current.Server.MapPath("~/Files/");
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.Contents)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                    byte[] fileArray = await file.ReadAsByteArrayAsync();

                    //создаем временный файл на сервере
                    using (FileStream fs = new FileStream(root + filename, FileMode.Create))
                    {
                        await fs.WriteAsync(fileArray, 0, fileArray.Length);
                    }                   

                    //чтение из файла с созданием списка контрагентов
                    using (StreamReader sr = new StreamReader(root + filename, Encoding.GetEncoding("windows-1251")))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("ПолучательИНН="))
                            {
                                tempKontrAgent = new KontrAgent();
                                tempKontrAgent.Inn = line.Replace("ПолучательИНН=", "");
                            }
                            else if (tempKontrAgent != null)
                            {
                                if (line.StartsWith("Получатель1="))
                                {
                                    tempKontrAgent.Name = line.Replace("Получатель1=", "");

                                    //если в списке из файла уже есть запись с такими ИНН и названием - не добавляем ее повторно
                                    if (listKontrAgents.FirstOrDefault(v => v.Name == tempKontrAgent.Name && v.Inn == tempKontrAgent.Inn) != null)
                                    {
                                        tempKontrAgent = null;
                                    }
                                    else //проверим еще на наличие дубликата в самой БД
                                    {
                                        if (repo.FindByInnName(tempKontrAgent.Inn, tempKontrAgent.Name, tempKontrAgent.Id) != null)
                                        {
                                            tempKontrAgent.Id = -1;
                                        }
                                    }
                                }
                                else if (line.StartsWith("Получатель2="))
                                {
                                    tempKontrAgent.Account = line.Replace("Получатель2=", "");
                                }
                                else if (line.StartsWith("Получатель3="))
                                {
                                    tempKontrAgent.BankName = line.Replace("Получатель3=", "");
                                }
                                else if (line.StartsWith("Получатель4="))
                                {
                                    tempKontrAgent.BankCity = line.Replace("Получатель4=", "");
                                    listKontrAgents.Add(tempKontrAgent);
                                }
                            }
                        }
                    }
                    File.Delete(root + filename); //удаляем файл из временной папки Files
                }
                return Ok(listKontrAgents);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //добавление в БД списка выбранных из файла пользователем контрагентов 
        [HttpPost]
        public IHttpActionResult CreateKontrAgent(int id, [FromBody]IEnumerable<KontrAgent> tempListKontrAgents)
        {
            try
            {
                foreach (KontrAgent kontrAgent in tempListKontrAgents)
                {
                    repo.Create(kontrAgent);
                }
                return Ok();
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
