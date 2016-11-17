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
        private List<KontrAgent> listKontrAgents;

        public async Task<IEnumerable<KontrAgent>> Post()
        {
            string line;
            KontrAgent tempKontrAgent = null;

            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return null;
                }
                var provider = new MultipartMemoryStreamProvider();
                //путь к папке на сервере
                string root = System.Web.HttpContext.Current.Server.MapPath("~/Files/");
                await Request.Content.ReadAsMultipartAsync(provider);

                listKontrAgents = new List<KontrAgent>();

                foreach (var file in provider.Contents)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                    byte[] fileArray = await file.ReadAsByteArrayAsync();

                    using (FileStream fs = new FileStream(root + filename, FileMode.Create))
                    {
                        await fs.WriteAsync(fileArray, 0, fileArray.Length);
                    }
                    

                    //чтение из файла с созданием списка моделей
                    using (StreamReader sr = new StreamReader(root + filename, Encoding.GetEncoding("windows-1251")))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("ПолучательИНН="))
                            {
                                tempKontrAgent = new KontrAgent();
                                tempKontrAgent.Inn = line.Replace("ПолучательИНН=", "");
                            }
                            else if (line.StartsWith("Получатель1="))
                            {
                                tempKontrAgent.Name = line.Replace("Получатель1=", "");
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
                    File.Delete(root + filename);
                }
                return listKontrAgents;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public void CreateKontrAgent(int id, [FromBody]KontrAgent kontrAgent)
        {
            repo.Create(kontrAgent);
        }
    }
}
