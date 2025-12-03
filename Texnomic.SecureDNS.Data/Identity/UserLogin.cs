using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Texnomic.SecureDNS.Data.Identity;

[Table("UserLogins")]
public class UserLogin : IdentityUserLogin<Guid>
{

}