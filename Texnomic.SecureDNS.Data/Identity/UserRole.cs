using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Texnomic.SecureDNS.Data.Identity;

[Table("UserRoles")]
public class UserRole : IdentityUserRole<Guid>
{

}