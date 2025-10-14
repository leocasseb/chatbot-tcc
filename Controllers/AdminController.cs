using Microsoft.AspNetCore.Mvc;
using ChatbotAPI.Models;

namespace ChatbotAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly CursoRepositorio cursoRepo;
        private readonly TicketRepositorio ticketRepo;
        private readonly PerguntaRepositorio perguntaRepo;


        public AdminController()
        {
            cursoRepo = new CursoRepositorio();
            ticketRepo = new TicketRepositorio();
            perguntaRepo = new PerguntaRepositorio();
        }

        [HttpGet("cursos")]
        public IActionResult GetCursos()
        {
            try
            {
                var cursos = cursoRepo.Listar();
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        [HttpPost("cursos")]
        public IActionResult PostCurso([FromBody] Curso curso)
        {
            if (curso.periodos > 20)
            {
                return BadRequest(new { erro = "O curso não pode ter mais de 20 períodos" });
            }
            
            cursoRepo.Adicionar(curso);
            return Ok(new { sucesso = true });
        }

        [HttpDelete("cursos/{id}")]
        public IActionResult DeleteCurso(int id)
        {
            try
            {
                var curso = cursoRepo.Obter(id);
                if (curso == null)
                    return BadRequest(new { erro = "Curso não encontrado" });
                
                cursoRepo.Remover(id);
                return Ok(new { sucesso = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        [HttpGet("tickets")]
        public IActionResult GetTickets()
        {
            var tickets = ticketRepo.Listar();
            return Ok(tickets);
        }

        [HttpPost("tickets")]
        public IActionResult PostTicket([FromBody] Ticket ticket)
        {
            ticketRepo.Adicionar(ticket);
            return Ok(new { sucesso = true });
        }

        [HttpDelete("tickets/{id}")]
        public IActionResult DeleteTicket(int id)
        {
            ticketRepo.Remover(id);
            return Ok(new { sucesso = true });
        }

        [HttpPut("tickets/{id}/resolver")]
        public IActionResult ResolverTicket(int id)
        {
            ticketRepo.MarcarResolvido(id);
            return Ok(new { sucesso = true });
        }

        [HttpGet("perguntas")]
        public IActionResult GetPerguntas()
        {
            return Ok(perguntaRepo.Listar());
        }

        [HttpPost("perguntas")]
        public IActionResult PostPergunta([FromBody] Pergunta pergunta)
        {
            perguntaRepo.Adicionar(pergunta);
            return Ok(new { sucesso = true });
        }

        [HttpPut("perguntas/{id}")]
        public IActionResult PutPergunta(int id, [FromBody] Pergunta pergunta)
        {
            pergunta.id = id;
            perguntaRepo.Atualizar(pergunta);
            return Ok(new { sucesso = true });
        }

        [HttpDelete("perguntas/{id}")]
        public IActionResult DeletePergunta(int id)
        {
            perguntaRepo.Remover(id);
            return Ok(new { sucesso = true });
        }


    }
}