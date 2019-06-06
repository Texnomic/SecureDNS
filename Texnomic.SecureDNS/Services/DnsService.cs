using Hangfire;
using Texnomic.DNS.Server;

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
            JobID = JobClient.Enqueue<DnsServer>(Server => Server.Listen());
        }

        public void Stop()
        {
            JobClient.Delete(JobID);
        }
    }
}
