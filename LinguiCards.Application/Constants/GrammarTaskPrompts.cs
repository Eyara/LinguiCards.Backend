namespace LinguiCards.Application.Constants;

public static class GrammarTaskPrompts
{
    private const string GrammarTaskPrompt = @"You are a Latin language teacher.
Generate one grammar exercise for a student.
Language: {0}
Level: {1}
Topic: {2}
Type: {3}
Respond only with the exercise text that the student will see, written in Latin (instructions in Russian if needed).
Use a unique internal pattern ID: {4}. This pattern *SHOULD* affect on variety of vocabular in text.";

    private const string EvaluationGrammarTaskPrompt = @"
Evaluate the grammar exercise. Always reply in Russian. Respond strictly in the following format:
Точность: N %. *that means accuracy of student's answer*
Твой ответ: *repeat student's answer*
Правильный ответ: *provide the correct answer*. 
Объяснение: *explain the grammar rule used in this exercise*. 
If something is not applicable, write ""нет"". No additional text.

Language: {0}. Level: {1}. Topic: {2}. Type: {3}.
Task text: {4}
Student's answer: {5}";

    public static string GetGrammarTaskPrompt(string language, string level, string? topic, string? type)
    {
        var topicText = string.IsNullOrWhiteSpace(topic) ? "any" : topic;
        var typeText = string.IsNullOrWhiteSpace(type) ? "any" : type;
        var patternId = Random.Shared.Next(1_000_000, int.MaxValue);
        return string.Format(GrammarTaskPrompt, language, level, topicText, typeText, patternId);
    }

    public static string GetEvaluationGrammarTaskPrompt(string language, string level, string taskText, string userAnswer, string? topic, string? type)
    {
        var topicText = string.IsNullOrWhiteSpace(topic) ? "any" : topic;
        var typeText = string.IsNullOrWhiteSpace(type) ? "any" : type;
        return string.Format(EvaluationGrammarTaskPrompt, language, level, topicText, typeText, taskText, userAnswer);
    }
}

