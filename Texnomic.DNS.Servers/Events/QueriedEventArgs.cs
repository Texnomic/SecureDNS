using System;
using System.Net;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Servers.Events
{
    public class QueriedEventArgs : EventArgs
    {
        public readonly IMessage Request;
        public readonly IPEndPoint EndPoint;

        public QueriedEventArgs(IMessage Request, IPEndPoint EndPoint)
        {
            this.Request = Request;
            this.EndPoint = EndPoint;
        }
    }
}