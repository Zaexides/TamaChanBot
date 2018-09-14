using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using TamaChanBot.API;
using TamaChanBot.Utility;
using ZaeTools.HTML;
using ZaeTools.HTML.CSS;
using ZaeTools.HTML.CSS.Properties;

namespace TamaChanBot.Core
{
    public class HelpFileGenerator
    {
        public const string HTML_HELP_FILE_PATH = @"Data\Help.html";

        public List<Command> commands = new List<Command>();

        public void Generate()
        {
            TamaChan.Instance.Logger.LogInfo("Generating help files...");
            WriteHelpFile();
            TamaChan.Instance.Logger.LogInfo("Help files generated.");
        }

        private void WriteHelpFile()
        {
            using (HTMLBuilder builder = new HTMLBuilder(new HTMLDocument()))
            {
                builder.HeadBuilder
                    .SetTitle("Tama Chan Documentation")
                    .SetCSS(CreateCSS());

                builder.BodyBuilder.AddTag(CreateNavigationIndex());

                GenericContainerBuilder mainDiv = GenericContainerBuilder.CreateDiv().SetClass("content") as GenericContainerBuilder;
                TableBuilder[] tables = CreateTables();

                foreach (TableBuilder table in tables)
                    mainDiv.TagContentBuilder.AddTag(table);
                builder.BodyBuilder.AddTag(mainDiv);

                File.WriteAllText(HTML_HELP_FILE_PATH, builder.BuildHTML());
            }

            commands.Clear();
        }

        private TableBuilder[] CreateTables()
        {
            TableBuilder commandTable = new TableBuilder().SetCaption(new TableBuilder.Caption().TagContentBuilder.AddText("Normal Commands").Parent as TableBuilder.Caption).SetId("regular") as TableBuilder;
            TableBuilder nsfwCommandTable = new TableBuilder().SetCaption(new TableBuilder.Caption().TagContentBuilder.AddText("NSFW Commands").Parent as TableBuilder.Caption).SetId("nsfw") as TableBuilder;
            TableBuilder adminCommandTable = new TableBuilder().SetCaption(new TableBuilder.Caption().TagContentBuilder.AddText("Admin Commands").Parent as TableBuilder.Caption).SetId("admin") as TableBuilder;
            TableBuilder ownerCommandTable = new TableBuilder().SetCaption(new TableBuilder.Caption().TagContentBuilder.AddText("Bot Owner Commands").Parent as TableBuilder.Caption).SetId("owner") as TableBuilder;

            string[] tableHeaders = new string[] { "Command", "Parameters", "Description", "Required Permissions" };
            TableBuilder[] tables = new TableBuilder[] { commandTable, adminCommandTable, nsfwCommandTable, ownerCommandTable };
            foreach (TableBuilder t in tables)
            {
                TableBuilder.Row row = new TableBuilder.Row();
                foreach (string th in tableHeaders)
                    row.AddTableEntry(TableBuilder.Row.Cell.CreateTableHeader().TagContentBuilder.AddText(th).Parent as TableBuilder.Row.Cell);
                t.AddRow(row);
            }

            foreach (Command c in commands)
            {
                TableBuilder targetTable;
                if (c.botOwnerOnly)
                    targetTable = ownerCommandTable;
                else if (c.permissionFlag.HasFlag(Permission.Admin))
                    targetTable = adminCommandTable;
                else if (c.isNsfw)
                    targetTable = nsfwCommandTable;
                else
                    targetTable = commandTable;

                targetTable.AddRow(new TableBuilder.Row()
                    .AddTableEntries(
                    TableBuilder.Row.Cell.CreateTableData().TagContentBuilder.AddText($"{TamaChan.Instance.botSettings.commandPrefix}{c.name}").Parent as TableBuilder.Row.Cell,
                    TableBuilder.Row.Cell.CreateTableData().TagContentBuilder.AddSection(GetParametersDivision(c)).Parent as TableBuilder.Row.Cell,
                    TableBuilder.Row.Cell.CreateTableData().TagContentBuilder.AddText(c.Description).Parent as TableBuilder.Row.Cell,
                    TableBuilder.Row.Cell.CreateTableData().TagContentBuilder.AddSection(GetPermissionDivisions(c)).Parent as TableBuilder.Row.Cell
                    ));

                c.Description = null;
            }

            return tables;
        }

        private GenericContainerBuilder GetParametersDivision(Command command)
        {
            GenericContainerBuilder division = GenericContainerBuilder.CreateDiv();

            ParameterInfo[] parameters = command.method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parameters[i];

                if (!parameterInfo.ParameterType.IsAssignableFrom(typeof(MessageContext)))
                {
                    ParameterInfo nextParameter = (i < parameters.Length - 1) ? parameters[i + 1] : null;
                    if (parameterInfo.IsOptionalParameter(nextParameter))
                        division.TagContentBuilder.AddSection(GetParameterDiv(parameterInfo, true));
                    else
                        division.TagContentBuilder.AddSection(GetParameterDiv(parameterInfo, false));
                }
            }

            return division;
        }

        private GenericContainerBuilder GetParameterDiv(ParameterInfo parameterInfo, bool isOptional)
        {
            GenericContainerBuilder paragraph = GenericContainerBuilder.CreateDiv();

            string className = "parameter";
            if (isOptional)
                className += " optional-parameter";
            paragraph.SetClass(className);

            string paragraphText = parameterInfo.Name;
            if (isOptional)
            {
                string defaultValueText;
                if (parameterInfo.DefaultValue == null || string.IsNullOrWhiteSpace(parameterInfo.DefaultValue.ToString()))
                    defaultValueText = "none";
                else
                    defaultValueText = parameterInfo.DefaultValue.ToString();

                paragraphText += $" = <span class=\"default-value\">{defaultValueText}</span>";
            }

            paragraph.TagContentBuilder.AddText(
                isOptional ? $"({paragraphText})" : $"[{paragraphText}]"
                );

            if (isOptional)
                paragraph.SetTitle("Optional");

            return paragraph;
        }

        private GenericContainerBuilder GetPermissionDivisions(Command command)
        {
            GenericContainerBuilder division = GenericContainerBuilder.CreateDiv();
            division.SetClass("permissions");

            foreach(Permission permission in Enum.GetValues(typeof(Permission)))
            {
                if(command.permissionFlag.HasFlag(permission))
                {
                    GenericContainerBuilder subDivision = GenericContainerBuilder.CreateDiv();
                    subDivision.TagContentBuilder.AddText(permission.ToString());
                    subDivision.SetClass("permission");
                    division.TagContentBuilder.AddSection(subDivision);
                }
            }

            return division;
        }

        private GenericContainerBuilder CreateNavigationIndex()
        {
            GenericContainerBuilder nav = GenericContainerBuilder.CreateNav();

            nav.TagContentBuilder.AddTag(
                GenericContainerBuilder.CreateHeading(GenericContainerBuilder.HeadingSize.MediumLarge).TagContentBuilder.AddText("Index").Parent
                ).AddTag(
                new AnchorBuilder("#regular").TagContentBuilder.AddText("&#9679;Normal").Parent
                ).AddBreak().AddTag(
                new AnchorBuilder("#admin").TagContentBuilder.AddText("&#9679;Admin").Parent
                ).AddBreak().AddTag(
                new AnchorBuilder("#nsfw").TagContentBuilder.AddText("&#9679;NSFW").Parent
                ).AddBreak().AddTag(
                new AnchorBuilder("#owner").TagContentBuilder.AddText("&#9679;Owner").Parent
                );

            return nav;
        }

        private CSSDocument CreateCSS()
        {
            CSSDocument css = new CSSDocument();

            css.AddElements(
                new CSSHTMLElement("body")
                    .AddProperty(new CSSProperty("color", "#e3e3e3"))
                    .AddProperty(new CSSProperty("background-color", "#383838"))
                    .AddProperty(new CSSProperty("font-family", "Whitney, Helvetica, Neue, Arial, sans-serif")),
                new CSSHTMLElement("nav")
                    .AddProperty(new CSSProperty("width", "12%"))
                    .AddProperty(new CSSProperty("height", "100%"))
                    .AddProperty(new CSSProperty("position", "fixed"))
                    .AddProperty(new CSSProperty("z-index", "1"))
                    .AddProperty(new CSSProperty("top", "0"))
                    .AddProperty(new CSSProperty("left", "0"))
                    .AddProperty(new CSSProperty("background-color", "#262626"))
                    .AddProperty(new CSSProperty("padding-left", "5px"))
                    .AddProperty(new CSSProperty("overflow-x", "hidden")),
                new CSSHTMLElement("nav a")
                    .AddProperty(new CSSProperty("padding", "5px"))
                    .AddProperty(new CSSProperty("line-height", "1.5"))
                    .AddProperty(new CSSProperty("color", "#d8d8d8"))
                    .AddProperty(new CSSProperty("text-decoration-line", "none"))
                    .AddProperty(new CSSProperty("font-weight", "bold")),
                new CSSClassElement("content")
                    .AddProperty(new CSSProperty("margin-left", "12.5%")),
                new CSSHTMLElement("table")
                    .AddProperty(new CSSProperty("border-style", "solid solid solid solid"))
                    .AddProperty(new CSSProperty("border-width", "1px 1px 1px 5px"))
                    .AddProperty(new CSSProperty("border-color", "#282828 #282828 #282828 #cecefe"))
                    .AddProperty(new CSSProperty("border-radius", "5px"))
                    .AddProperty(new CSSProperty("border-spacing", "0px"))
                    .AddProperty(new CSSProperty("background-color", "#343434"))
                    .AddProperty(new CSSProperty("width", "100%"))
                    .AddProperty(new CSSProperty("margin-bottom", "10px"))
                    .AddProperty(new CSSProperty("table-layout", "fixed"))
                    .AddProperty(new CSSProperty("word-wrap", "break-word")),
                new CSSHTMLElement("caption")
                    .AddProperty(new CSSProperty("font-weight", "bold"))
                    .AddProperty(new CSSProperty("font-size", "1.2em")),
                new CSSHTMLElement("th")
                    .AddProperty(new CSSProperty("border-style", "none none solid none"))
                    .AddProperty(new CSSProperty("background-color", "#323232")),
                new CSSHTMLElement("th, td")
                    .AddProperty(new CSSProperty("border-color", "#282828"))
                    .AddProperty(new CSSProperty("padding", "3px")),
                new CSSHTMLElement("th:nth-child(even), td:nth-child(even)")
                    .AddProperty(new CSSProperty("background-color", "rgba(0,0,0,0.1)")),
                new CSSHTMLElement("th:first-child, td:first-child")
                    .AddProperty(new CSSProperty("width", "15%")),
                new CSSHTMLElement("tr:not(:first-child) td")
                    .AddProperty(new CSSProperty("border-style", "solid none none none"))
                    .AddProperty(new CSSProperty("border-width", "1px")),
                new CSSHTMLElement("th:nth-child(2), td:nth-child(2)")
                    .AddProperty(new CSSProperty("width", "15%")),
                new CSSHTMLElement("th:nth-child(4), td:nth-child(4)")
                    .AddProperty(new CSSProperty("width", "12.5%")),

                new CSSClassElement("parameter")
                    .AddProperty(new CSSProperty("font-size", "0.75em"))
                    .AddProperty(new CSSProperty("font-family", "Lucida Console, Georgia")),
                new CSSClassElement("optional-parameter")
                    .AddProperty(new CSSProperty("text-decoration", "underline #a4a4b2 dotted")),
                new CSSClassElement("default-value")
                    .AddProperty(new CSSProperty("font-style", "italic"))
                );

            return css;
        }
    }
}
