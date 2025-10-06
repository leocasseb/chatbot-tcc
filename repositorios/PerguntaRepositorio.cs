using System.Text.Json;
using ChatbotAPI.Models;

public class PerguntaRepositorio
{
    private readonly string arquivo = Path.Combine(Path.GetTempPath(), "chatbot-data", "perguntas.json");
    private List<Pergunta> perguntas;

    public PerguntaRepositorio()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(arquivo) ?? "data");
        if (File.Exists(arquivo))
        {
            var json = File.ReadAllText(arquivo);
            perguntas = JsonSerializer.Deserialize<List<Pergunta>>(json) ?? new List<Pergunta>();
        }
        else
        {
            perguntas = CriarDadosIniciais();
            Salvar();
        }
    }

    public List<Pergunta> Listar() 
    {
        RemoverExpiradas();
        return perguntas;
    }

    public Pergunta ObterPorId(int id) 
    {
        RemoverExpiradas();
        return perguntas.FirstOrDefault(p => p.id == id);
    }

    private void RemoverExpiradas()
    {
        var hoje = DateTime.Now.ToString("dd/MM/yyyy");
        var expiradas = perguntas.Where(p => p.temporaria && 
            !string.IsNullOrEmpty(p.dataExpiracao) && 
            DateTime.TryParseExact(p.dataExpiracao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dataExp) &&
            dataExp < DateTime.Now.Date).ToList();
        
        if (expiradas.Any())
        {
            foreach (var exp in expiradas)
            {
                perguntas.Remove(exp);
            }
            Salvar();
        }
    }

    public void Adicionar(Pergunta pergunta)
    {
        pergunta.id = perguntas.Count > 0 ? perguntas.Max(x => x.id) + 1 : 1;
        perguntas.Add(pergunta);
        
        // Se tem pergunta pai, adiciona na lista de subperguntas
        if (pergunta.perguntaPaiId.HasValue)
        {
            var pai = perguntas.FirstOrDefault(p => p.id == pergunta.perguntaPaiId.Value);
            if (pai != null && !pai.subPerguntasIds.Contains(pergunta.id))
            {
                pai.subPerguntasIds.Add(pergunta.id);
            }
        }
        
        Salvar();
    }

    public void Atualizar(Pergunta pergunta)
    {
        var index = perguntas.FindIndex(p => p.id == pergunta.id);
        if (index >= 0)
        {
            perguntas[index] = pergunta;
            Salvar();
        }
    }

    public void Remover(int id)
    {
        perguntas.RemoveAll(p => p.id == id);
        Salvar();
    }

    private void Salvar()
    {
        var json = JsonSerializer.Serialize(perguntas, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(arquivo, json);
    }

    private List<Pergunta> CriarDadosIniciais()
    {
        return new List<Pergunta>
        {
            new Pergunta { id = 1, texto = "Informações Gerais", curso = "Todos", periodo = "Todos", subPerguntasIds = new List<int> { 2, 3, 4 } },
            new Pergunta { id = 2, texto = "Horário de funcionamento da biblioteca", resposta = "A biblioteca funciona de segunda a sexta das 8h às 22h, e aos sábados das 8h às 17h.", curso = "Todos", periodo = "Todos", perguntaPaiId = 1 },
            new Pergunta { id = 3, texto = "Localização da secretaria", resposta = "A secretaria fica no térreo do prédio principal, sala 101.", curso = "Todos", periodo = "Todos", perguntaPaiId = 1 },
            new Pergunta { id = 4, texto = "Como solicitar histórico escolar", resposta = "Você pode solicitar o histórico na secretaria ou pelo portal do aluno online.", curso = "Todos", periodo = "Todos", perguntaPaiId = 1 },
            new Pergunta { id = 5, texto = "Informações do Curso", curso = "Ciência da Computação", periodo = "Todos", subPerguntasIds = new List<int> { 6, 7 } },
            new Pergunta { id = 6, texto = "Localização das salas de aula", resposta = "As aulas de Ciência da Computação acontecem no 2º andar, salas 201 a 210.", curso = "Ciência da Computação", periodo = "Todos", perguntaPaiId = 5 },
            new Pergunta { id = 7, texto = "Laboratório de informática", resposta = "O laboratório fica na sala 205, disponível das 7h às 22h.", curso = "Ciência da Computação", periodo = "Todos", perguntaPaiId = 5 }
        };
    }
}