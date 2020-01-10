using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using Polly;
using Polly.Retry;
using RestSharp;
using Serilog;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Models;
using Texnomic.DNS.Records;
using Texnomic.DNS.Servers.Extensions;
using Texnomic.FilterLists;
using Texnomic.FilterLists.Enums;
using Texnomic.FilterLists.Models;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class FilterMiddleware : IAsyncMiddleware<IMessage, IMessage>
    {
        private readonly ILogger Logger;
        private readonly RestClient RestClient;
        private readonly AsyncRetryPolicy<IRestResponse<string>> RetryPolicy;
        private readonly FastHashSet<string> Filter;

        public FilterMiddleware(Tags[] Tags, ILogger Logger) : base()
        {
            this.Logger = Logger;

            RestClient = new RestClient();

            Filter = new FastHashSet<string>();

            RetryPolicy = Policy.HandleResult<IRestResponse<string>>(ResultPredicate)
                                .RetryAsync(3);

            _ = InitializeAsync(Tags);
        }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            if (Filter.Contains(Message.Questions.First().Domain.Name))
            {
                Logger.Warning("Filtered {@Query}.", Message);

                return new Message()
                {
                    ID = Message.ID,
                    MessageType = MessageType.Response,
                    ResponseCode = ResponseCode.NoError,
                    Questions = Message.Questions,
                    Answers = new List<IAnswer>()
                    {
                        new Answer()
                        {
                            Type = RecordType.A,

                            Class = RecordClass.Internet,

                            Domain = Message.Questions.First().Domain,

                            Length = 32,

                            TimeToLive = new TimeToLive()
                            {
                               Value = TimeSpan.FromDays(1)
                            },

                            Record = new A()
                            {
                                 Address = new IPv4Address()
                                 {
                                     Value = IPAddress.Parse("192.168.254.230")
                                 }
                            }
                        }
                    }
                };
            }
            else
            {
                return await Next(Message);
            }
        }

        public async Task InitializeAsync(Tags[] Tags)
        {
            try
            {
                Logger.Information("Initializing FilterLists.");

                var Lists = await GetFilterListsAsync(Tags);

                Logger.Information("Found {@Count} FilterLists.", Lists.Count);

                string File = null;
                string[] Domains = null;

                foreach (var List in Lists)
                {
                    try
                    {
                        File = await DownloadAsync(List.ViewUrl);

                        Domains = Parse(File);

                        for (int i = 0; i < Domains.Length; i++)
                        {
                            if (!Filter.Contains(Domains[i]))
                            {
                                Filter.Add(Domains[i]);
                            }
                        }

                        Logger.Information("Downloaded FilterList {@FilterList}.", List.ViewUrl);
                    }
                    catch (WebException Error)
                    {
                        Logger.Error("{@Error} While Downloading FilterList {@FilterList}.", Error, List.ViewUrl);
                    }
                }

                File = null;
                Domains = null;
                Lists = null;

                Logger.Information("FilterLists Initialized.");
            }
            catch (Exception Error)
            {
                Logger.Fatal("Fatal {@Error} Occurred While Initializing FilterLists.", Error);
            }
        }

        private async Task<List<FilterList>> GetFilterListsAsync(Tags[] Tags)
        {
            var Client = new FilterListsClient();

            var Lists = await Client.GetListsAsync();

            Lists = Lists.Where(List => List.Syntax == Syntax.Hosts)
                         .Where(List => List.Tags.Any(Tag => Tags.Contains(Tag)))
                         .Take(5)
                         .ToList();

            return Lists;
        }

        private static bool ResultPredicate(IRestResponse Response)
        {
            return Response.ErrorException != null;
        }
        private async Task<string> DownloadAsync(string URL)
        {
            var Request = new RestRequest(URL);

            var Response = await RetryPolicy.ExecuteAsync(() => RestClient.ExecuteGetTaskAsync<string>(Request));

            if (Response.ErrorException != null) throw Response.ErrorException;

            return Response.Data;
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
