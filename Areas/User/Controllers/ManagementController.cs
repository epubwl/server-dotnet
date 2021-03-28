using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.User.Data;
using EpubWebLibraryServer.Areas.User.Models;

namespace EpubWebLibraryServer.Areas.User.Controllers
{
    [ApiController]
    [Area("User")]
    public class ManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagementController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        [Route("/api/users/passwordchange")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChange passwordChange)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user is null)
            {
                return BadRequest();
            }
            IdentityResult result = await _userManager.ChangePasswordAsync(user, passwordChange.CurrentPassword, passwordChange.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }   
}