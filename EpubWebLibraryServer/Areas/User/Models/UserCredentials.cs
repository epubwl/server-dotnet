using System.ComponentModel.DataAnnotations;

namespace EpubWebLibraryServer.Areas.User.Models
{
    public class UserCredentials
    {
        [Required]
        public string Username { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
}