using System.IO;
using System.Linq;
using CaptureSnippets;
using Xunit;

public class DocoUpdater
{
    [Fact]
    public void Run()
    {
        var root = GitRepoDirectoryFinder.FindForFilePath();

        var finder = new FileFinder();
        var snippetSourceFiles = finder.FindFiles(Path.Combine(root, "src/Docs/Snippets"));
        var snippets = FileSnippetExtractor.Read(snippetSourceFiles).ToList();
        var handling = new GitHubSnippetMarkdownHandling(root);
        var processor = new MarkdownProcessor(snippets, handling.AppendGroup);
        var sourceMdFiles = Directory.EnumerateFiles(Path.Combine(root, "pages/source"), "*.md");
        var pagesDir = Path.Combine(root, "pages");
        PurgeDirectory(pagesDir);
        foreach (var sourceFile in sourceMdFiles)
        {
            ProcessFile(sourceFile, processor, pagesDir);
        }
    }

    static void PurgeDirectory(string pagesDir)
    {
        foreach (var page in Directory.EnumerateFiles(pagesDir, "*.md"))
        {
            File.Delete(page);
        }
    }

    static void ProcessFile(string sourceFile, MarkdownProcessor markdownProcessor, string pagesDir)
    {
        var target = Path.Combine(pagesDir, Path.GetFileName(sourceFile));
        using (var reader = File.OpenText(sourceFile))
        using (var writer = File.CreateText(target))
        {
            var processResult = markdownProcessor.Apply(reader, writer);
            var missing = processResult.MissingSnippets;
            if (missing.Any())
            {
                throw new MissingSnippetsException(missing);
            }
        }
    }
}