using CSharpCourse.DesignPatterns.Assignments;

namespace CSharpCourse.DesignPatterns.Tests.AssignmentTests;

public class FluentMarkdownBuilderTests
{
    [Fact]
    public void AddTable()
    {
        string[] headers = ["Tables", "Are", "Cool"];
        string[][] rows = [
            ["Hello", "World", "!!!"],
            ["One", "Two", "Three"]
        ];

        var output = new FluentMarkdownBuilder()
            .AddTable(headers, rows)
            .ToString();

        var expected = """
                       |Tables|Are|Cool|
                       |---|---|---|
                       |Hello|World|!!!|
                       |One|Two|Three|

                       """;

        Assert.Equal(expected, output);
    }

    [Fact]
    public void NestedMarkdown()
    {
        string[] headers = ["My", "Table"];

        var output = new FluentMarkdownBuilder()
            .AddTable(headers, table =>
            {
                table.AddRow(row =>
                    {
                        row.AddCell(cell =>
                        {
                            cell.AddLink(link =>
                            {
                                link.AddBold("Bold");
                            }, "https://example.com");
                        });
                        row.AddCell(cell =>
                        {
                            cell.AddItalic("Italic");
                        });
                    });
            })
            .ToString();

        var expected = """
                       |My|Table|
                       |---|---|
                       |[**Bold**](https://example.com)|*Italic*|

                       """;

        Assert.Equal(expected, output);
    }
}
