using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Texnomic.SecureDNS.Data.Abstractions.Identity;

namespace Texnomic.SecureDNS.Data.Identity;

[Table("Users")]
public class User : IdentityUser<Guid>, IUser
{
    public User()
    {
        Id = Guid.NewGuid();
    }
}