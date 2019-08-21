using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Texnomic.SecureDNS.Data.Identity
{
    [Table("UserTokens")]
    public class UserToken : IdentityUserToken<Guid>
    {

    }
}
