using System;
using System.Net;
using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.DNS.Servers.Events
{
    public class ResolvedEventArgs : EventArgs
    {
        public readonly IMessage Request;
        public readonly IMessage Response;
        public readonly IPEndPoint EndPoint;

        public ResolvedEventArgs(IMessage Request, IMessage Response, IPEndPoint EndPoint)
        {
            this.Request = Request;
            this.Response = Response;
            this.EndPoint = EndPoint;
        }
    }
}