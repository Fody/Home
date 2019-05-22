using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkdownSnippets;
using Xunit;
using Xunit.Abstractions;

public class DocoUpdater :
    XunitLoggingBase
{
    [Fact]
    public async Task Run()
    {
        var root = GitRepoDirectoryFinder.FindForFilePath();

        var finder = new FileFinder();
        var addinPath = Path.Combine(root, "BasicFodyAddin");
        var snippetSourceFiles = finder.FindFiles(
            Path.Combine(root, "src/Docs"),
            addinPath);
        var snippets = FileSnippetExtractor.Read(snippetSourceFiles).ToList();

        await snippets.AppendUrlsAsSnippets(
            "https://raw.githubusercontent.com/Fody/Fody/master/FodyPackaging/Weaver.props",
            "https://raw.githubusercontent.com/Fody/Fody/master/FodyPackaging/build/FodyPackaging.props",
            "https://raw.githubusercontent.com/Fody/Fody/master/FodyPackaging/build/FodyPackaging.targets");

        var pagesDir = Path.Combine(root, "pages");
        PurgeDirectory(pagesDir);

        var markdownProcessor = new DirectoryMarkdownProcessor(root);
        markdownProcessor.IncludeSnippets(snippets);
        markdownProcessor.Run();
    }

    static void PurgeDirectory(string pagesDir)
    {
        foreach (var page in Directory.EnumerateFiles(pagesDir, "*.md"))
        {
            File.Delete(page);
        }
    }

    public DocoUpdater(ITestOutputHelper output) :
        base(output)
    {
    }
}