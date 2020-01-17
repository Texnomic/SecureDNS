using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PipelineNet.Middleware;
using Serilog;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Servers.Extensions;
using Texnomic.DNS.Servers.Options;
using Texnomic.FilterLists;
using Texnomic.FilterLists.Enums;
using Texnomic.FilterLists.Models;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class FilterMiddleware : IAsyncMiddleware<IMessage, IMessage>
    {
        private readonly ILogger Logger;
        private readonly WebClient WebClient;
        private readonly FastHashSet<string> Filter;

        public FilterMiddleware(IOptionsMonitor<FilterMiddlewareOptions> Options, ILogger Logger) : base()
        {
            this.Logger = Logger;

            WebClient = new WebClient();

            Filter = new FastHashSet<string>();

            _ = InitializeAsync(Options.CurrentValue.ListTags);
        }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            if (Filter.Contains(Message.Questions.First().Domain.Name))
            {
                Logger.Warning("Filtered Query {@ID}.", Message.ID);

                return new Message()
                {
                    ID = Message.ID,
                    MessageType = MessageType.Response,
                    ResponseCode = ResponseCode.Refused,
                    Questions = Message.Questions
                };
            }
            else
            {
                return await Next(Message);
            }
        }

        public async Task InitializeAsync(List<Tags> Tags)
        {
            try
            {
                Logger.Verbose("FilterLists Initialization Started with {@Tags} Selected.", Tags);

                var Lists = await GetFilterListsAsync(Tags);

                Logger.Information("FilterLists Initialization Started with {@Count} Selected Lists.", Lists.Count);

                string File = null;
                string[] Domains = null;

                foreach (var List in Lists)
                {
                    try
                    {
                        File = await WebClient.DownloadStringTaskAsync(List.ViewUrl);

                        Domains = Parse(File);

                        for (int i = 0; i < Domains.Length; i++)
                        {
                            if (!Filter.Contains(Domains[i]))
                            {
                                Filter.Add(Domains[i]);
                            }
                        }

                        Logger.Verbose("{@FilterList} Download Completed.", List);
                    }
                    catch (Exception Error)
                    {
                        Logger.Error("{@Error} While Downloading {@FilterList}.", Error, List);
                    }
                }

                File = null;
                Domains = null;
                Lists = null;

                Logger.Information("FilterLists Initialization Completed with {@Count} Domains.", Filter.Count);
            }
            catch (Exception Error)
            {
                Logger.Fatal("Fatal {@Error} Occurred While Initializing FilterLists.", Error);
            }
        }

        private static async Task<List<FilterList>> GetFilterListsAsync(List<Tags> Tags)
        {
            var Client = new FilterListsClient();

            var Lists = await Client.GetListsAsync();

            Lists = Lists.Where(List => List.Syntax == Syntax.Hosts)
                         .Where(List => List.Tags.Any(Tag => Tags.Contains(Tag)))
                         .ToList();

            return Lists;
        }

        private static string[] Parse(string File)
        {
            return File.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                       .Select(Line => Line.Replace("0.0.0.0", ""))
                       .Select(Line => Line.Replace("127.0.0.1", ""))
                       .Select(Line => Line.Trim())
                       .SkipWhile(Line => Line.StartsWith("#"))
                       .SkipWhile(Line => string.IsNullOrEmpty(Line))
                       .SkipWhile(Line => string.IsNullOrWhiteSpace(Line))
                       .ToArray();
        }
    }
}
