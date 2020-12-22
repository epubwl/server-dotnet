using System.ComponentModel.DataAnnotations;

namespace EpubWebLibraryServer.Areas.User.Models
{
    public class PasswordChange
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}