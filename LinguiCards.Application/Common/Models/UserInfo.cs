﻿namespace LinguiCards.Application.Common.Models;

public class UserInfo
{
    public List<LanguageStat> LanguageStats { get; set; }
}

public class LanguageStat
{
    public string LanguageName { get; set; }
    public int TotalWords { get; set; }
    public int LearnedWords { get; set; }
    public double LearnedPercent { get; set; }
    public int TotalTrainingDays { get; set; }
}