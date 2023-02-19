using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Texnomic.SecureDNS.Data.Identity;

[Table("UserClaims")]
public class UserClaim : IdentityUserClaim<Guid>
{

}