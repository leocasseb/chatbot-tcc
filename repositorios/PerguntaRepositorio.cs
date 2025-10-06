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
            perguntas = new List<Pergunta>();
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
}