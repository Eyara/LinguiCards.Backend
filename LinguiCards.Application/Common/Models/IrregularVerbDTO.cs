namespace LinguiCards.Application.Common.Models;

public class IrregularVerbDto
{
    public int Id { get; set; }
    public string BaseForm { get; set; }
    public string PastSimple { get; set; }
    public string PastParticiple { get; set; }
    public int LanguageId { get; set; }
}

