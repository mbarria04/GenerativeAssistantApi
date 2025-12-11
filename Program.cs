
using GenerativeAssistantApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<KnowledgeStore>();
builder.Services.AddSingleton<RagService>();
builder.Services.AddSingleton<LlmService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

// Ingesta de texto plano
app.MapPost("/api/assistant/ingest", async (IngestRequest req, KnowledgeStore store) =>
{
    if (string.IsNullOrWhiteSpace(req?.Title) || string.IsNullOrWhiteSpace(req?.Content))
        return Results.BadRequest(new { error = "Debe enviar Title y Content" });
    await store.SaveAsync(req.Title, req.Content);
    return Results.Ok(new { ok = true });
});


// Listado de documentos
app.MapGet("/api/assistant/documents", (KnowledgeStore store) => Results.Ok(store.ListTitles()));

// Chat (usa RAG + LLM opcional)
app.MapPost("/api/assistant/chat", async (ChatMessageRequest req, RagService rag, LlmService llm) =>
{
    if (string.IsNullOrWhiteSpace(req?.Message))
        return Results.BadRequest(new { error = "Mensaje vacío" });

    var context = rag.Retrieve(req.Message);
    var reply = await llm.ReplyAsync(req.Message, context);
    return Results.Ok(new { reply, context });
});

app.Run();

public record IngestRequest(string Title, string Content);
public record ChatMessageRequest(string Message);
