using Microsoft.Extensions.Options;
using PipelineNet.Middleware;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Texnomic.DNS.Models;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;
using Texnomic.DNS.Servers.Options;
using Texnomic.DNS.Records;
using System.Linq;

namespace Texnomic.DNS.Servers.Middlewares
{
    public class HostTableMiddleware : IAsyncMiddleware<IMessage, IMessage>
    {
        private readonly ILogger Logger;
        private Dictionary<string, IPAddress> HostTable;

        public HostTableMiddleware(IOptionsMonitor<HostTableMiddlewareOptions> Options, ILogger Logger)
        {
            this.Logger = Logger;

            Options.OnChange(OptionsOnChange);

            HostTable = Options.CurrentValue.HostTable.ToDictionary(HT => HT.Key, HT => IPAddress.Parse(HT.Value));
        }

        private void OptionsOnChange(HostTableMiddlewareOptions Options)
        {
            HostTable = Options.HostTable.ToDictionary(HT => HT.Key, HT => IPAddress.Parse(HT.Value));

            Logger.Information("Host Table {@HostTable} Updates Applied.", HostTable);
        }

        public async Task<IMessage> Run(IMessage Message, Func<IMessage, Task<IMessage>> Next)
        {
            if (Message.Questions[0].Type == RecordType.A)
            {
                if (HostTable.ContainsKey(Message.Questions[0].Domain.Name))
                {
                    Logger.Information("Resolved Query {@ID} For {@Domain} From Host Table.", Message.ID, Message.Questions[0].Domain.Name);

                    return new Message()
                    {
                        ID = Message.ID,
                        MessageType = MessageType.Response,
                        ResponseCode = ResponseCode.NoError,
                        Questions = new List<IQuestion>()
                        {
                            new Question()
                            {
                                Type = RecordType.A,
                                Class = RecordClass.Internet,
                                Domain = new Domain(Message.Questions[0].Domain.Name)
                            }
                        },
                        Answers = new List<IAnswer>()
                        {
                          new Answer()
                          {
                              TimeToLive = new TimeToLive()
                              {
                                  Value = TimeSpan.FromSeconds(30)
                              },

                              Domain = new Domain(Message.Questions[0].Domain.Name),

                              Length = 32,

                              Record = new A()
                              {
                                  Address = new IPv4Address()
                                  {
                                      Value = HostTable.GetValueOrDefault(Message.Questions[0].Domain.Name)
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
            else
            {
                return await Next(Message);
            }
        }
    }
}
