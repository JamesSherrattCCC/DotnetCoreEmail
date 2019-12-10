using EmailDaemon.Config;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmailDaemon.Auth
{
    /// <summary>
    /// Class containing the Azure authorisation configuration params.
    /// </summary>
    public class AuthConfig : AConfig<AuthConfig>
    {
        [Required(AllowEmptyStrings = false)]
        public string Instance { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Tenant { get; set; } = "common";

        [Required(AllowEmptyStrings = false)]
        public string ClientId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public IEnumerable<string> Scopes { get; set; }

        public string ClientSecret { get; set; }

        public string CertificateName { get; set; }

        public static AuthConfig Config { get; }  = ReadFromJsonFile("authsettings.json");
    }
}
