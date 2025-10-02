using Microsoft.AspNetCore.Mvc;
using ChatbotAPI.Models;

namespace ChatbotAPI.Controllers
{
    [ApiController]
    [Route("api/chatbot")]
    public class ChatbotController : ControllerBase
    {
        private readonly CursoRepositorio cursoRepo;
        private readonly TicketRepositorio ticketRepo;

        public ChatbotController()
        {
            cursoRepo = new CursoRepositorio();
            ticketRepo = new TicketRepositorio();
        }

        [HttpGet("cursos")]
        public IActionResult GetCursos()
        {
            var cursos = cursoRepo.Listar().Select(c => c.nome).ToList();
            return Ok(cursos);
        }

        [HttpGet("periodos/{curso}")]
        public IActionResult GetPeriodos(string curso)
        {
            var cursoObj = cursoRepo.Listar().FirstOrDefault(c => c.nome == curso);
            if (cursoObj == null) return NotFound();
            
            var periodos = new List<string>();
            for (int i = 1; i <= cursoObj.periodos; i++)
            {
                periodos.Add($"{i}º Período");
            }
            return Ok(periodos);
        }

        [HttpGet("perguntas/{curso}/{periodo}")]
        public IActionResult GetPerguntas(string curso, string periodo)
        {
            var perguntaRepo = new PerguntaRepositorio();
            var todasPerguntas = perguntaRepo.Listar();
            
            // Filtrar apenas perguntas principais (sem pai)
            var perguntas = todasPerguntas.Where(p => 
                (p.curso == curso || p.curso == "Todos") &&
                (p.periodo == periodo || p.periodo == "Todos") &&
                !p.perguntaPaiId.HasValue // Apenas perguntas principais
            ).ToList();
            
            return Ok(perguntas);
        }

        [HttpGet("subperguntas/{perguntaPaiId}")]
        public IActionResult GetSubperguntas(int perguntaPaiId)
        {
            var perguntaRepo = new PerguntaRepositorio();
            var todasPerguntas = perguntaRepo.Listar();
            
            // Buscar subperguntas da pergunta pai
            var subperguntas = todasPerguntas.Where(p => p.perguntaPaiId == perguntaPaiId).ToList();
            
            return Ok(subperguntas);
        }

        [HttpGet("pergunta/{id}")]
        public IActionResult GetPergunta(int id)
        {
            var perguntaRepo = new PerguntaRepositorio();
            var pergunta = perguntaRepo.ObterPorId(id);
            
            if (pergunta == null)
                return NotFound();
                
            return Ok(pergunta);
        }



        [HttpPost("ticket")]
        public IActionResult PostTicket([FromBody] Ticket ticket)
        {
            ticketRepo.Adicionar(ticket);
            return Ok(new { sucesso = true, id = ticket.id });
        }
    }
}