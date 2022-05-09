using System.ComponentModel.DataAnnotations;

namespace APIMarketplaceApp.Models
{
    public class AuthenticateRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string MotDePasse { get; set; }
    }
}