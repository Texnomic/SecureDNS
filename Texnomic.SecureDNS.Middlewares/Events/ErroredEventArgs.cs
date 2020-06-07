using System;

namespace Texnomic.SecureDNS.Middlewares.Events
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