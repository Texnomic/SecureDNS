using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Texnomic.SecureDNS.Data.Identity;

[Table("RoleClaims")]
public class RoleClaim : IdentityRoleClaim<Guid>
{

}