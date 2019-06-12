using System.Collections.Generic;
using System.IO;
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

        List<Snippet> snippets = new List<Snippet>();

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