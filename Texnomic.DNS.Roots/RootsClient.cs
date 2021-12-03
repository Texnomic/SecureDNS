using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RestSharp;
using Texnomic.DNS.Roots.Models;
using YamlDotNet.Serialization;

namespace Texnomic.DNS.Roots
{
    public static class RootsClient
    {
        private static readonly RestClient RestClient = new RestClient($"https://root-servers.org/archives/");

        public static async Task<Root[]> GetList()
        {
            var Date = await GetDate();
            var Links = await GetLinks(Date);
            return await GetYAML(Links);
        }

        private static async Task<Root[]> GetYAML(List<string> Links)
        {
            return await Task.WhenAll(Links.ConvertAll(GetYAML));
        }

        private static async Task<Root> GetYAML(string Link)
        {
            var RestRequest = new RestRequest(Link);

            var Response = await RestClient.ExecuteGetAsync(RestRequest);

            var Deserializer = new Deserializer();

            return Deserializer.Deserialize<Root>(Response.Content);
        }

        private static async Task<List<string>> GetLinks(string Date)
        {
            var RestRequest = new RestRequest(Date);

            var Response = await RestClient.ExecuteGetAsync(RestRequest);

            var HtmlDocument = new HtmlDocument();

            HtmlDocument.LoadHtml(Response.Content);

            return HtmlDocument.DocumentNode
                                .SelectNodes("/html/body/pre/a[position()>1]")
                                .Select(Node => Node.GetAttributeValue("href", null))
                                .Select(YAML => $"{Date}{YAML}")
                                .ToList();
        }

        private static async Task<string> GetDate()
        {
            var RestRequest = new RestRequest();

            var Response = await RestClient.ExecuteGetAsync(RestRequest);

            var HtmlDocument = new HtmlDocument();

            HtmlDocument.LoadHtml(Response.Content);

            return HtmlDocument.DocumentNode.SelectSingleNode("/html/body/pre/a[last()]").InnerText;
        }
    }
}
