namespace LinguiCards.Domain.Entities;

public class CribDescription
{
    public int Id { get; set; }
    public string Header { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public int Type { get; set; }
    public int CribId { get; set; }
}