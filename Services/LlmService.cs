
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;

public class LlmService
{
    private readonly IConfiguration _cfg;
    public LlmService(IConfiguration cfg) { _cfg = cfg; }

    public async Task<string> ReplyAsync(string userMessage, string context)
    {
        var endpoint = _cfg["AzureOpenAI:Endpoint"];
        var key = _cfg["AzureOpenAI:Key"];
        var deployment = _cfg["AzureOpenAI:Deployment"]; // nombre del deployment (ej: gpt-4o-mini)

        // Fallback si no configuras Azure OpenAI
        if (string.IsNullOrWhiteSpace(endpoint) ||
            string.IsNullOrWhiteSpace(key) ||
            string.IsNullOrWhiteSpace(deployment))
        {
            return @$"(Fallback) Basado en la documentación:

{context}

Respuesta: Para tu pregunta '{userMessage}', sigue los pasos indicados arriba.";
        }

        // Cliente v2 con ApiKeyCredential
        var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(key));
        var chatClient = client.GetChatClient(deployment);

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("Eres un asistente de conocimiento interno. Usa solo el contexto proporcionado y no inventes datos."),
            new AssistantChatMessage($"Contexto:\n{context}"),
            new UserChatMessage(userMessage)
        };

        var options = new ChatCompletionOptions
        {
            Temperature = 0.1f
            /*MaxOutputTokens = 800*/   // <-- propiedad correcta en v2
        };

        var response = await chatClient.CompleteChatAsync(messages, options);

        // En v2, el contenido viene como partes; tomamos la última parte de texto
        var text = response.Value.Content.LastOrDefault()?.Text;
        return string.IsNullOrWhiteSpace(text) ? "(Sin respuesta)" : text!;
    }
}
