
# GenerativeAssistantApi (ASP.NET Core 8 Web API)

API centrada solo en **IA generativa** con:
- `/api/assistant/ingest` → ingesta de documentos (texto)
- `/api/assistant/documents` → listar títulos
- `/api/assistant/chat` → chat que usa RAG simplificado + Azure OpenAI opcional

## Ejecutar
```bash
cd GenerativeAssistantApi
dotnet restore
dotnet run
```

## Configurar Azure OpenAI (opcional)
Editar `appsettings.json`:
```json
{
  "AzureOpenAI": {
    "Endpoint": "https://<endpoint>.openai.azure.com/",
    "Key": "<api-key>",
    "Deployment": "<deployment>"
  }
}
```

## Evitar duplicados de Content
Este proyecto **no** incluye `<Content Include="wwwroot\**\*">` en el `.csproj`. Para copiar `Knowledge/**` al output se usa `<None Update=... CopyToOutputDirectory=Always/>`, que **no** genera duplicados.
