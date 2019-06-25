using System.Threading.Tasks;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Resolvers
{
    public interface IResolver
    {
        Task<byte[]> ResolveAsync(byte[] Request);
        Task<Message> ResolveAsync(Message Request);
    }
}
