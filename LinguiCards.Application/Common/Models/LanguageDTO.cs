namespace LinguiCards.Application.Common.Models;

public class LanguageDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }

    public int UserId { get; set; }
    public int LanguageDictionaryId { get; set; }
}

public class LanguageAddDto
{
    public string Name { get; set; }
    public int LanguageDictionaryId { get; set; }
}