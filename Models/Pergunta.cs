namespace ChatbotAPI.Models
{
    public class Pergunta
    {
        public int id { get; set; }
        public string texto { get; set; } = string.Empty;
        public string resposta { get; set; } = string.Empty;
        public string curso { get; set; } = string.Empty; // "Todos" para todos os cursos
        public string periodo { get; set; } = string.Empty; // "Todos" para todos os períodos
        public int? perguntaPaiId { get; set; } // Para árvore de perguntas
        public List<int> subPerguntasIds { get; set; } = new List<int>();
        public string dataExpiracao { get; set; } // Data de expiração (opcional)
        public bool temporaria { get; set; } = false; // Se é uma resposta temporária
    }
}