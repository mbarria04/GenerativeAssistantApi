
using System.Text.RegularExpressions;

namespace GenerativeAssistantApi.Services
{
    public class RagService
    {
        private readonly KnowledgeStore _store;
        public RagService(KnowledgeStore store) => _store = store;

        public string Retrieve(string query, int maxLines = 40)
        {
            // 1) Leer todo el contenido (Markdowns)
            var content = _store.ReadAll() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(content))
                return "(No hay documentos en Knowledge/)";

            // 2) Tokenizar la consulta con Regex para obtener palabras significativas
            var keywords = Regex.Matches(query ?? string.Empty, @"\w+")
                                .Select(m => m.Value.ToLowerInvariant())
                                .Distinct()
                                .ToArray();

            // 3) Dividir el contenido en líneas (soporta \r\n y \n)
            var lines = content.Replace("\r\n", "\n").Split('\n');

            // 4) Buscar líneas que contengan cualquiera de las keywords (case-insensitive)
            var matched = lines
                .Where(l => keywords.Length == 0
                            ? false
                            : keywords.Any(k => l.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0))
                .Take(maxLines)
                .ToArray();

            // 5) Si no hay coincidencias, devolver las primeras líneas como contexto básico
            if (matched.Length == 0)
                matched = lines.Take(Math.Min(maxLines, lines.Length)).ToArray();

            // 6) Unir líneas con salto de línea (no string.Empty)
            var ctx = string.Join("\n", matched);

            // 7) Garantizar que el resultado no sea vacío
            if (string.IsNullOrWhiteSpace(ctx))
                ctx = "(No se encontraron fragmentos relevantes)";

            return ctx;
        }
    }
}
