using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Texnomic.DNS.Models;
using Texnomic.SecureDNS.Data;
using Texnomic.SecureDNS.Models;

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

        public async Task Initalize()
        {
            await FilterLists_EasyPrivacy();
            await FilterLists_EasyList();
            await FilterLists_280Blocker();
            await FilterLists_1Hosts();
            await RansomwareTracker();
            await MalwareDomains();
            await MalwareDomainList();
            await YoYo();
        }

        private async Task FilterLists_EasyPrivacy()
        {
            var File = await Download("https://v.firebog.net/hosts/Easyprivacy.txt");

            Save(File);
        }

        private async Task FilterLists_EasyList()
        {
            var File = await Download("https://raw.githubusercontent.com/austinheap/sophos-xg-block-lists/master/easylist.txt");

            Save(File);
        }

        private async Task FilterLists_280Blocker()
        {
            var File = await Download("https://280blocker.net/files/280blocker_domain.txt");

            Save(File);
        }

        private async Task FilterLists_1Hosts()
        {
            var File = await Download("https://1hos.cf/complete/list.txt");

            Save(File);
        }

        private async Task RansomwareTracker()
        {
            var File = await Download("https://ransomwaretracker.abuse.ch/downloads/RW_DOMBL.txt");

            Save(File);
        }

        private async Task MalwareDomains()
        {
            var File = await Download("http://mirror1.malwaredomains.com/files/justdomains");

            Save(File);
        }

        private async Task MalwareDomainList()
        {
            var File = await Download("http://www.malwaredomainlist.com/hostslist/hosts.txt");

            File = File.Select(Line => Line.Replace("127.0.0.1", "").Trim());

            Save(File);
        }

        private async Task YoYo()
        {
            var File = await Download("https://pgl.yoyo.org/as/serverlist.php?showintro=0;hostformat=hosts");

            File = File.Select(Line => Line.Replace("127.0.0.1", "").Trim());

            Save(File);
        }

        private void Save(IEnumerable<string> File)
        {
            Upsert(Convert(File));
        }
        private List<Blacklist> Convert(IEnumerable<string> File)
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
        private async Task<IEnumerable<string>> Download(string URL)
        {
            using (var Client = HttpClientFactory.CreateClient())
            {
                var Data = await Client.GetStringAsync(URL);

                return Parse(Data);
            }
        }
        private IEnumerable<string> Parse(string Data)
        {
            return Data.Split("\n")
                       .Select(Line => Line.Trim())
                       .SkipWhile(Line => Line.StartsWith("#"))
                       .SkipWhile(Line => string.IsNullOrEmpty(Line))
                       .SkipWhile(Line => string.IsNullOrWhiteSpace(Line));
        }
    }
}
