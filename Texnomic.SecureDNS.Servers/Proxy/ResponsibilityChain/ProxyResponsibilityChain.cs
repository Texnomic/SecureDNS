<<<<<<< HEAD
﻿using Microsoft.Extensions.Options;
using PipelineNet.ChainsOfResponsibility;
using PipelineNet.MiddlewareResolver;
using Texnomic.SecureDNS.Abstractions;
using Texnomic.SecureDNS.Abstractions.Enums;
using Texnomic.SecureDNS.Servers.Proxy.Options;
=======
﻿using PipelineNet.ChainsOfResponsibility;
>>>>>>> 80e159a06224c769a7805e12328fb284df6c8bc1

namespace Texnomic.SecureDNS.Servers.Proxy.ResponsibilityChain;

public class ProxyResponsibilityChain : AsyncResponsibilityChain<IMessage, IMessage>
{
    public ProxyResponsibilityChain(IOptionsMonitor<ProxyResponsibilityChainOptions> Options, IMiddlewareResolver MiddlewareResolver) : base(MiddlewareResolver)
    {
        Options.CurrentValue.GetMiddlewares().ForEach(Middleware => Chain(Middleware));

        Finally(FinalCheck);
    }

    private static async Task<IMessage> FinalCheck(IMessage Message)
    {
        if (Message.MessageType != MessageType.Response)
            throw new NotImplementedException("Responsibility Chain Fall-throw!");

        return await Task.FromResult(Message);
    }
}