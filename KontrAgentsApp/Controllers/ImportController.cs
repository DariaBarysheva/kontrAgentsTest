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
    //контроллер для страницы "Импорт контрагентов"
    public class ImportController : ApiController
    {
        KontrAgentRepository repo = new KontrAgentRepository(); 

        //получение с клиента файла с контрагентами, парсинг файла и возврат списка найденных записей
        public async Task<IHttpActionResult> Post()
        {
            string line; //для хранения одной строки, прочитанной из файла
            List<KontrAgent> listKontrAgents = new List<KontrAgent>(); ; //для хранения списка контрагентов, прочитанных из файла
            KontrAgent tempKontrAgent = null; //для хранения текущего контрагента, прочитанного из файла
            KontrAgent dubl = null; //для хранения данных о дубликате в БД для записи в файле

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
                                tempKontrAgent.Inn = line.Replace("ПолучательИНН=", "").Trim();
                            }
                            else if (tempKontrAgent != null)
                            {
                                if (line.StartsWith("Получатель1="))
                                {
                                    tempKontrAgent.Name = line.Replace("Получатель1=", "").Trim();

                                    //если в списке из файла уже есть запись с такими ИНН и названием - не добавляем ее повторно
                                    if (listKontrAgents.FirstOrDefault(v => v.Name == tempKontrAgent.Name && v.Inn == tempKontrAgent.Inn) != null)
                                    {
                                        tempKontrAgent = null;
                                    }
                                    else //проверим еще на наличие дубликата в самой БД
                                    {
                                        if ((dubl = repo.FindByInnName(tempKontrAgent.Inn, tempKontrAgent.Name, tempKontrAgent.Id)) != null)
                                        {
                                            tempKontrAgent.Id = dubl.Id; //если есть дубликат - вернем его ID на случай, если пользователь захочет обновить ранее добавленную запись в БД
                                        }
                                    }
                                }
                                else if (line.StartsWith("Получатель2="))
                                {
                                    tempKontrAgent.Account = line.Replace("Получатель2=", "").Trim();
                                }
                                else if (line.StartsWith("Получатель3="))
                                {
                                    tempKontrAgent.BankName = line.Replace("Получатель3=", "").Trim();
                                }
                                else if (line.StartsWith("Получатель4="))
                                {
                                    tempKontrAgent.BankCity = line.Replace("Получатель4=", "").Trim();
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
        //требуется указать ограничения на маршрут, применимый к данному методу, т.к. два метода POST без параметров
        [Route("api/import/createkontragent")]
        public IHttpActionResult CreateKontrAgent([FromBody]IEnumerable<KontrAgent> tempListKontrAgents)
        {
            int editCount = 0; //для подсчета обновленных записей
            int addCount = 0; //для подсчета добавленных записей
            try
            {
                foreach (KontrAgent kontrAgent in tempListKontrAgents)
                {
                    if (kontrAgent.Id != 0) //нужно обновить запись
                    {
                        repo.Update(kontrAgent);
                        editCount++;
                    }
                    else //нужно добавить запись
                    {
                        repo.Create(kontrAgent);
                        addCount++;
                    }
                }
                return Ok("Операция загрузки выполнена успешно. В процессе загрузки было создано " + addCount.ToString() + " записей и обновлено " + editCount.ToString() + " записей");
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
