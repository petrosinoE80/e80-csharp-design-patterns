using System.Text;

namespace CSharpCourse.DesignPatterns.Assignments;

internal class FluentMarkdownBuilder
{
    private readonly StringBuilder _builder = new();

    private void BuildHeaders(string[] headers)
    {
        _builder.AppendLine("|" + string.Join("|", headers) + "|");
        _builder.AppendLine("|" + string.Join("|", Enumerable.Repeat("---", headers.Length)) + "|");
    }

    public FluentMarkdownBuilder AddBold(string text)
    {
        _builder.Append($"**{text}**");
        return this;
    }

    public FluentMarkdownBuilder AddHeader(int headerLevel, string text)
    {
        var prefix = new string(Enumerable.Repeat('#', headerLevel).ToArray());
        _builder.AppendLine($"{prefix} {text}");
        return this;
    }

    public FluentMarkdownBuilder AddItalic(string text)
    {
        _builder.Append($"*{text}*");
        return this;
    }

    public FluentMarkdownBuilder AddLink(string name, string url)
    {
        _builder.Append($"{name}");
        return this;
    }

    public FluentMarkdownBuilder AddTable(string[] headers, string[][] rows)
    {
        BuildHeaders(headers);
        foreach (var row in rows) _builder.AppendLine("|" + string.Join("|", row) + "|");
        return this;
    }

    public FluentMarkdownBuilder AddTable(string[] headers, Action<TableBuilder> tableAction)
    {
        BuildHeaders(headers);

        var tableBuilder = new TableBuilder();
        tableAction(tableBuilder);
        _builder.Append(tableBuilder.Build());

        return this;
    }

    public FluentMarkdownBuilder AddText(string text)
    {
        _builder.Append(text);
        return this;
    }

    public FluentMarkdownBuilder NewLine()
    {
        _builder.AppendLine();
        return this;
    }

    public override string ToString()
        => _builder.ToString();

    public class CellBuilder
    {
        private readonly StringBuilder _cellBuilder = new();

        public CellBuilder AddBold(string text)
        {
            _cellBuilder.Append($"**{text}**");
            return this;
        }

        public CellBuilder AddItalic(string text)
        {
            _cellBuilder.Append($"*{text}*");
            return this;
        }

        public CellBuilder AddLink(Action<LinkBuilder> linkAction, string url)
        {
            var linkBuilder = new LinkBuilder();
            linkAction(linkBuilder); // Configura il link
            _cellBuilder.Append($"[{linkBuilder.Build()}]({url})"); // Aggiunge il link formattato
            return this;
        }

        public string Build()
        {
            return _cellBuilder.ToString();
        }
    }

    public class LinkBuilder
    {
        private readonly StringBuilder _linkBuilder = new();

        public LinkBuilder AddBold(string text)
        {
            _linkBuilder.Append($"**{text}**");
            return this;
        }

        public string Build()
        {
            return _linkBuilder.ToString();
        }
    }

    public class RowBuilder
    {
        private readonly StringBuilder _rowBuilder = new();

        public RowBuilder AddCell(Action<CellBuilder> cellAction)
        {
            var cellBuilder = new CellBuilder();
            cellAction(cellBuilder);
            _rowBuilder.Append("|" + cellBuilder.Build() + "");

            return this;
        }

        public string Build()
        {
            return _rowBuilder.ToString() + "|";
        }
    }

    public class TableBuilder
    {
        private readonly StringBuilder _tableBuilder = new();

        public TableBuilder AddRow(Action<RowBuilder> rowAction)
        {
            var rowBuilder = new RowBuilder();
            rowAction(rowBuilder);
            _tableBuilder.AppendLine(rowBuilder.Build());

            return this;
        }

        public string Build()
        {
            return _tableBuilder.ToString();
        }
    }
}