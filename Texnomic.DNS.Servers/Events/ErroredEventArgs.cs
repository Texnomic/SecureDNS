using System;
using Texnomic.DNS.Abstractions;

namespace Texnomic.DNS.Servers.Events
{
    public class ErroredEventArgs : EventArgs
    {
        public readonly Exception Error;

        public ErroredEventArgs(Exception Error)
        {
            this.Error = Error;
        }
    }
}