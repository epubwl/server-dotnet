using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.Library.Services;
using EpubWebLibraryServer.Areas.User.Models;

namespace EpubWebLibraryServer.Areas.User.Controllers
{
    [ApiController]
    [Area("User")]
    public class ManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly EpubManager _epubManager;

        public ManagementController(UserManager<ApplicationUser> userManager, EpubManager epubManager)
        {
            _userManager = userManager;
            _epubManager = epubManager;
        }

        [Authorize]
        [HttpPost]
        [Route("/api/users/passwordchange")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChange passwordChange)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user is null)
            {
                return Unauthorized();
            }
            IdentityResult result = await _userManager.ChangePasswordAsync(user, passwordChange.CurrentPassword, passwordChange.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [Authorize]
        [HttpDelete]
        [Route("/api/users/{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user is null)
            {
                return Unauthorized();
            }
            if (!String.Equals(user.UserName, username))
            {
                return Unauthorized();
            }
            await _epubManager.DeleteAllEpubsFromOwner(username);
            await _userManager.DeleteAsync(user);
            return NoContent();
        }
    }   
}