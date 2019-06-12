using System.Threading.Tasks;
using Texnomic.DNS.Models;

namespace Texnomic.DNS.Resolvers
{
    public interface IResolver
    {
        Task<Message> ResolveAsync(Message Request);
    }
}
