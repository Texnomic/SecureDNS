using System.ComponentModel.DataAnnotations;

namespace Texnomic.SecureDNS.Data.Models
{
    public class Configuration
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
