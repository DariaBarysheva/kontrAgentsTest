using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace KontrAgentsApp.Models
{
    //Класс, содержащий логику работу с БД
    public class KontrAgentRepository
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //Извлечение данных о контрагентах из БД
        public List<KontrAgent> GetKontrAgents()
        {
            List<KontrAgent> kontrAgents = new List<KontrAgent>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                kontrAgents = db.Query<KontrAgent>("SELECT * FROM KontrAgents").ToList();
            }
            return kontrAgents;
        }

        //Получение одного контрагента по уникальному идентификатору
        public KontrAgent Get(int id)
        {
            KontrAgent kontrAgent = null;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                kontrAgent = db.Query<KontrAgent>("SELECT * FROM KontrAgents WHERE Id = @id", new { id }).FirstOrDefault();
            }
            return kontrAgent;
        }

        //Добавление нового контрагента
        public KontrAgent Create(KontrAgent kontrAgent)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "INSERT INTO KontrAgents (Name, Inn, Account, BankName, BankCity) VALUES(@Name, @Inn, @Account, @BankName, @BankCity); SELECT CAST(SCOPE_IDENTITY() as int);";
                int kontrAgentId = db.Query<int>(sqlQuery, kontrAgent).FirstOrDefault();
                kontrAgent.Id = kontrAgentId;
            }
            return kontrAgent;
        }

        //Обновление одного контрагента
        public void Update(KontrAgent kontrAgent)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "UPDATE KontrAgents SET Name = @Name, Inn = @Inn, Account = @Account, BankName = @BankName, BankCity = @BankCity WHERE Id = @Id";
                db.Execute(sqlQuery, kontrAgent);
            }
        }

        //Удаление одного контрагента
        public void Delete(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "DELETE FROM KontrAgents WHERE Id = @id";
                db.Execute(sqlQuery, new { id });
            }
        }
    }
}