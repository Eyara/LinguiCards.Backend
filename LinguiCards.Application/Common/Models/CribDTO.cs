namespace LinguiCards.Application.Common.Models;

public class CribDTO
{
    public int Id { get; set; }
    
    public int Languageid { get; set; }
    
    public List<CribDescriptionDTO> CribDescriptions { get; set; }
}