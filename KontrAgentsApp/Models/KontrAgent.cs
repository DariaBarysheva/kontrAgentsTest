namespace KontrAgentsApp.Models
{
    //Контрагент (получатель платежа)
    public class KontrAgent
    {
        public int Id { get; set; } //идентификатор
        public string Name { get; set; } //название
        public string Inn { get; set; } //ИНН
        public string Account { get; set; } //расчетный счет
        public string BankName { get; set; }  //название банка
        public string BankCity { get; set; }  //город банка    
    }
}