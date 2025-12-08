namespace SportSchedule.Services.Scraping
{
    public static class HTTPHelper
    {
        internal static async Task<HtmlDocument> LoadHtmlDocumentFromUrlAsync(HttpClient client, string url)
        {
            string html = await client.GetStringAsync(url);

            // Carico il contenuto in HtmlDocument
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        internal static HtmlNodeCollection FiltraNodiP(HtmlNode div)
        {
            return div.SelectNodes(".//p");
        }
    }
}
