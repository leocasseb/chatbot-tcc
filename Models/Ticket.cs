namespace ChatbotAPI.Models
{
    public class Ticket
    {
        public int id { get; set; }
        public string aluno { get; set; } = string.Empty;
        public string matricula { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public bool anonimo { get; set; } = false;
        public string curso { get; set; } = string.Empty;
        public string assunto { get; set; } = string.Empty;
        public string mensagem { get; set; } = string.Empty;
        public string data { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        public string status { get; set; } = "Pendente";
    }
}