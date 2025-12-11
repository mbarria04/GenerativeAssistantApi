
namespace GenerativeAssistantApi.Services
{
    public class KnowledgeStore
    {
        private readonly string _path;
        public KnowledgeStore(IWebHostEnvironment env)
        {
            _path = Path.Combine(env.ContentRootPath, "Knowledge");
            Directory.CreateDirectory(_path);
        }

        public async Task SaveAsync(string title, string content)
        {
            var safe = string.Join("_", title.Split(Path.GetInvalidFileNameChars()));
            var file = Path.Combine(_path, safe + ".md");
            await File.WriteAllTextAsync(file, content);
        }

        public IEnumerable<string> ListTitles()
        {
            return Directory.GetFiles(_path, "*.md").Select(Path.GetFileNameWithoutExtension);
        }

        public string ReadAll()
        {
            var files = Directory.GetFiles(_path, "*.md");
            return string.Join("", files.Select(File.ReadAllText));
        }
    }
}
