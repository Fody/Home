using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CaptureSnippets;
using Xunit;

public class DocoUpdater
{
    [Fact]
    public async Task Run()
    {
        var root = GitRepoDirectoryFinder.FindForFilePath();

        var finder = new FileFinder();
        var addinPath = Path.Combine(root,"BasicFodyAddin");
        var snippetSourceFiles = finder.FindFiles(
            Path.Combine(root, "src/Docs"),
            addinPath);
        var snippets = FileSnippetExtractor.Read(snippetSourceFiles).ToList();
        snippets.AppendFilesAsSnippets(
            Path.Combine(addinPath,"Tests/Tests.csproj"),
            Path.Combine(addinPath,"BasicFodyAddin/BasicFodyAddin.csproj"),
            Path.Combine(addinPath,"BasicFodyAddin.Fody/BasicFodyAddin.Fody.xcf"),
            Path.Combine(addinPath,"BasicFodyAddin.Fody/BasicFodyAddin.Fody.csproj"),
            Path.Combine(addinPath,"SmokeTest/FodyWeavers.xsd"),
            Path.Combine(addinPath,"appveyor.yml"));

        await snippets.AppendUrlsAsSnippets(
            "https://raw.githubusercontent.com/Fody/Fody/master/FodyPackaging/Weaver.props",
            "https://raw.githubusercontent.com/Fody/Fody/master/FodyPackaging/build/FodyPackaging.props",
            "https://raw.githubusercontent.com/Fody/Fody/master/FodyPackaging/build/FodyPackaging.targets");


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