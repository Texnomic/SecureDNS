using Serilog;
using System.Net;
using Microsoft.Extensions.Options;
using PipelineNet.Middleware;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Core;
using Texnomic.SecureDNS.Core.DataTypes;
using Texnomic.SecureDNS.Core.Records;
using Texnomic.SecureDNS.Middlewares.Options;

namespace Texnomic.SecureDNS.Middlewares;

public class HostTableMiddleware : IAsyncMiddleware<IMessage, IMessage>
{
    private readonly ILogger Logger;
    private readonly IOptionsMonitor<HostTableMiddlewareOptions> Options;
    private Dictionary<string, IPAddress> HostTable;

    public HostTableMiddleware(IOptionsMonitor<HostTableMiddlewareOptions> Options, ILogger Logger)
    {
        this.Logger = Logger;

        this.Options = Options;

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
        if (Message.Questions.First().Type == RecordType.A)
        {
            if (HostTable.ContainsKey(Message.Questions.First().Domain.Name))
            {
                Logger.Information("Resolved Query {@ID} For {@Domain} From Host Table.", Message.ID, Message.Questions.First().Domain.Name);

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
                            Domain = Domain.FromString(Message.Questions.First().Domain.Name)
                        }
                    },
                    Answers = new List<IAnswer>()
                    {
                        new Answer()
                        {
                            Type = RecordType.A,

                            Class = RecordClass.Internet,

                            TimeToLive = TimeSpan.FromSeconds(Options.CurrentValue.TimeToLive),

                            Domain = Domain.FromString(Message.Questions.First().Domain.Name),

                            Length = 4,

                            Record = new A()
                            {
                                Address = HostTable.GetValueOrDefault(Message.Questions.First().Domain.Name)
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