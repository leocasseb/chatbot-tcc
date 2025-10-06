using System.Text.Json;
using ChatbotAPI.Models;

public class TicketRepositorio
{
    private readonly string arquivo = Path.Combine(Path.GetTempPath(), "chatbot-data", "tickets.json");
    private List<Ticket> tickets;

    public TicketRepositorio()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(arquivo) ?? "data");
        if (File.Exists(arquivo))
        {
            var json = File.ReadAllText(arquivo);
            tickets = JsonSerializer.Deserialize<List<Ticket>>(json) ?? new List<Ticket>();
        }
        else
        {
            tickets = new List<Ticket>();
            Salvar();
        }
    }

    public List<Ticket> Listar() => tickets;

    public void Adicionar(Ticket ticket)
    {
        ticket.id = tickets.Count > 0 ? tickets.Max(x => x.id) + 1 : 1;
        tickets.Add(ticket);
        Salvar();
    }

    public void Remover(int id)
    {
        tickets.RemoveAll(t => t.id == id);
        Salvar();
    }

    public void MarcarResolvido(int id)
    {
        var ticket = tickets.FirstOrDefault(t => t.id == id);
        if (ticket != null)
        {
            ticket.status = "Resolvido";
            Salvar();
        }
    }

    private void Salvar()
    {
        var json = JsonSerializer.Serialize(tickets, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(arquivo, json);
    }
}