using System;
using System.Net;

using Texnomic.SecureDNS.Abstractions;

namespace Texnomic.SecureDNS.Middlewares.Events
{
    public class AnsweredEventArgs : EventArgs
    {
        public readonly IMessage Response;
        public readonly IPEndPoint EndPoint;

        public AnsweredEventArgs(IMessage Response, IPEndPoint EndPoint)
        {
            this.Response = Response;
            this.EndPoint = EndPoint;
        }
    }
}