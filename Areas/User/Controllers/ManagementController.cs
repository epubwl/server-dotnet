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
            this._userManager = userManager;
        }

        [Authorize]
        [HttpPatch]
        [Route("/api/users/password")]
        public async Task<IActionResult> ChangePassword(PasswordChange passwordChange)
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
            return Ok();
        }
    }   
}