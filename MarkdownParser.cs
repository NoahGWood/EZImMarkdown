using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ImGuiNET;

namespace EZImMarkdown
{
    public static class MarkdownParser
    {
        public static void RenderMarkdown(string mdContent)
        {
            string[] lines = mdContent.Split('\n');
            foreach (string line in lines)
            {
                ParseMarkdownLine(line);
                ImGui.NewLine();
            }
        }
        public static async Task RenderMarkdownFromUrl(string url)
        {
            try
            {
                string markdownContent = await DownloadMarkdownContent(url);
                if (!string.IsNullOrEmpty(markdownContent))
                {
                    ImGui.Begin("Markdown Viewer");

                    // Render Markdown using ImGui draw calls
                    MarkdownParser.RenderMarkdown(markdownContent);

                    ImGui.End();
                }
                else
                {
                    Console.WriteLine("Failed to download Markdown content from the URL.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occured: {ex.Message}");
            }
        }
        private static async Task<string> DownloadMarkdownContent(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download Markdown content from {url}: {ex.Message}");
                    return string.Empty;
                }
            }
        }
        public static void ParseMarkdownLine(string markdownLine)
        {
            if (Regex.IsMatch(markdownLine, @"^#{1,6}\s(.+)"))
            {
                // Header
                Match match = Regex.Match(markdownLine, @"^#{1,6}\s(.+)");
                string headerText = match.Groups[1].Value;
                RenderHeader(headerText, match.Groups[0].Length);
            }
            else if (Regex.IsMatch(markdownLine, @"^\*\s(.+)"))
            {
                // Unordered List
                Match match = Regex.Match(markdownLine, @"^\*\s(.+)");
                string listItemText = match.Groups[1].Value;
                RenderListItem(listItemText, ImGui.GetFontSize() * 0.6f, '\u2022'); // Bullet point character
            }
            else if (Regex.IsMatch(markdownLine, @"^\d+\.\s(.+)"))
            {
                // Ordered List
                Match match = Regex.Match(markdownLine, @"^\d+\.\s(.+)");
                string listItemText = match.Groups[1].Value;
                RenderListItem(listItemText, ImGui.GetFontSize() * 0.6f, '\u2022'); // Numbered list uses the same rendering as unordered
            }
            else if (Regex.IsMatch(markdownLine, @"^\[([^\]]+)\]\(([^)]+)\)"))
            {
                // Link
                Match match = Regex.Match(markdownLine, @"^\[([^\]]+)\]\(([^)]+)\)");
                string linkText = match.Groups[1].Value;
                string linkUrl = match.Groups[2].Value;
                RenderLink(linkText, linkUrl);
            }
            else if (Regex.IsMatch(markdownLine, @"^!\[([^\]]+)\]\(([^)]+)\)"))
            {
                // Image
                Match match = Regex.Match(markdownLine, @"^!\[([^\]]+)\]\(([^)]+)\)");
                string altText = match.Groups[1].Value;
                string imageUrl = match.Groups[2].Value;
                RenderImage(altText, imageUrl);
            }
            else if (Regex.IsMatch(markdownLine, @"^\s*-{3,}\s*$"))
            {
                // Horizontal Rule
                RenderHorizontalRule();
            }
            else
            {
                // Paragraph
                RenderParagraph(markdownLine);
            }
        }
        private static void RenderHeader(string text, int header)
        {
            // Render header based on the header level
            // You can customize this based on your ImGui implementation
            ImGui.TextColored(new System.Numerics.Vector4(1, 0, 0, 1), text); // Red color for headers
        }
        private static void RenderListItem(string text, float indent, char bulletChar)
        {
            ImGui.Indent(indent);
            ImGui.Text($"{bulletChar} {text}");
            ImGui.Unindent(indent);
        }

        private static void RenderLink(string text, string url)
        {
            ImGui.TextColored(new System.Numerics.Vector4(0, 0, 1, 1), $"[Link]({url}): {text}");
        }

        private static void RenderImage(string altText, string imageUrl)
        {
            ImGui.TextColored(new System.Numerics.Vector4(0, 1, 0, 1), $"[Image]({imageUrl}): {altText}");
            // You can add ImGui.Image rendering here using the image URL
        }

        private static void RenderHorizontalRule()
        {
            ImGui.Separator();
        }

        private static void RenderParagraph(string text)
        {
            // Render paragraphs as plain text
            ImGui.Text(text);
        }
    }
}
