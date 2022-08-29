using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Provider
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
