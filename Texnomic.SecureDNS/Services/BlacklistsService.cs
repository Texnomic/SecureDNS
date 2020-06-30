using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Texnomic.SecureDNS.Core.DataTypes;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Data.Models;

namespace Texnomic.SecureDNS.Services
{
    public class BlacklistsService
    {
        private readonly DatabaseContext DatabaseContext;
        private readonly IHttpClientFactory HttpClientFactory;

        public BlacklistsService(DatabaseContext DatabaseContext, IHttpClientFactory HttpClientFactory)
        {
            this.DatabaseContext = DatabaseContext;
            this.HttpClientFactory = HttpClientFactory;
        }

        public async Task InitializeAsync()
        {
            await FilterLists_EasyPrivacyAsync();
            await FilterLists_EasyListAsync();
            await FilterLists_280BlockerAsync();
            await FilterLists_1HostsAsync();
            await RansomwareTrackerAsync();
            await MalwareDomainsAsync();
            await MalwareDomainListAsync();
            await YoYoAsync();
        }

        private async Task FilterLists_EasyPrivacyAsync()
        {
            var File = await DownloadAsync("https://v.firebog.net/hosts/Easyprivacy.txt");

            Save(File);
        }

        private async Task FilterLists_EasyListAsync()
        {
            var File = await DownloadAsync("https://raw.githubusercontent.com/austinheap/sophos-xg-block-lists/master/easylist.txt");

            Save(File);
        }

        private async Task FilterLists_280BlockerAsync()
        {
            var File = await DownloadAsync("https://280blocker.net/files/280blocker_domain.txt");

            Save(File);
        }

        private async Task FilterLists_1HostsAsync()
        {
            var File = await DownloadAsync("https://1hos.cf/complete/list.txt");

            Save(File);
        }

        private async Task RansomwareTrackerAsync()
        {
            var File = await DownloadAsync("https://ransomwaretracker.abuse.ch/downloads/RW_DOMBL.txt");

            Save(File);
        }

        private async Task MalwareDomainsAsync()
        {
            var File = await DownloadAsync("http://mirror1.malwaredomains.com/files/justdomains");

            Save(File);
        }

        private async Task MalwareDomainListAsync()
        {
            var File = await DownloadAsync("http://www.malwaredomainlist.com/hostslist/hosts.txt");

            File = File.Select(Line => Line.Replace("127.0.0.1", "").Trim());

            Save(File);
        }

        private async Task YoYoAsync()
        {
            var File = await DownloadAsync("https://pgl.yoyo.org/as/serverlist.php?showintro=0;hostformat=hosts");

            File = File.Select(Line => Line.Replace("127.0.0.1", "").Trim());

            Save(File);
        }

        private void Save(IEnumerable<string> File)
        {
            Upsert(Convert(File));
        }
        private static IEnumerable<Blacklist> Convert(IEnumerable<string> File)
        {
            return File.ToList()
                       .ConvertAll(Line => new Blacklist() { Domain = Domain.FromString(Line) });
        }
        private void Upsert(IEnumerable<Blacklist> List)
        {
            List.Batch(100)
                .ForEach(Batch => DatabaseContext.Blacklists
                                                 .UpsertRange(Batch)
                                                 .On(Record => Record.Domain)
                                                 .Run());
        }
        private async Task<IEnumerable<string>> DownloadAsync(string URL)
        {
            using (var Client = HttpClientFactory.CreateClient())
            {
                var Data = await Client.GetStringAsync(URL);

                return Parse(Data);
            }
        }
        private static IEnumerable<string> Parse(string Data)
        {
            return Data.Split("\n")
                       .Select(Line => Line.Trim())
                       .SkipWhile(Line => Line.StartsWith("#"))
                       .SkipWhile(Line => string.IsNullOrEmpty(Line))
                       .SkipWhile(Line => string.IsNullOrWhiteSpace(Line));
        }
    }
}
