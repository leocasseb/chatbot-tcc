using System.Text.Json;
using ChatbotAPI.Models;

public class CursoRepositorio
{
    private readonly string arquivo = Path.Combine(Path.GetTempPath(), "chatbot-data", "cursos.json");
    private List<Curso> cursos;

    public CursoRepositorio()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(arquivo) ?? "data");
            if (File.Exists(arquivo))
            {
                var json = File.ReadAllText(arquivo);
                cursos = JsonSerializer.Deserialize<List<Curso>>(json) ?? new List<Curso>();
            }
            else
            {
                cursos = CriarDadosIniciais();
                Salvar();
            }
        }
        catch (Exception)
        {
            cursos = CriarDadosIniciais();
        }
    }

    public List<Curso> Listar() => cursos;

    public Curso Obter(int id) => cursos.FirstOrDefault(c => c.id == id);

    public void Adicionar(Curso curso)
    {
        curso.id = cursos.Count > 0 ? cursos.Max(x => x.id) + 1 : 1;
        cursos.Add(curso);
        Salvar();
    }

    public void Remover(int id)
    {
        cursos.RemoveAll(c => c.id == id);
        Salvar();
    }

    private void Salvar()
    {
        var json = JsonSerializer.Serialize(cursos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(arquivo, json);
    }

    private List<Curso> CriarDadosIniciais()
    {
        return new List<Curso>
        {
            new Curso { id = 1, nome = "Ciência da Computação", periodos = 8 },
            new Curso { id = 2, nome = "Engenharia de Software", periodos = 8 },
            new Curso { id = 3, nome = "Sistemas de Informação", periodos = 8 }
        };
    }
}