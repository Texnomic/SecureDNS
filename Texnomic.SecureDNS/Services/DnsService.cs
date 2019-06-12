using Hangfire;
using Texnomic.DNS;
using Texnomic.SecureDNS.Resolvers;

namespace Texnomic.SecureDNS.Services
{
    public class DnsService
    {
        private readonly IBackgroundJobClient JobClient;

        private static string JobID;

        public DnsService(IBackgroundJobClient BackgroundJobs)
        {
            JobClient = BackgroundJobs;
        }

        public void Start()
        {
            JobID = JobClient.Enqueue<DnsServer<DnsOverTls>>(Server => Server.StartAsync());
        }

        public void Stop()
        {
            JobClient.Delete(JobID);
        }
    }
}
