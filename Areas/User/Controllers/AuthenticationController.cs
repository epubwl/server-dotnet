using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.User.Data;
using EpubWebLibraryServer.Areas.User.Models;
using EpubWebLibraryServer.Areas.User.Services;

namespace EpubWebLibraryServer.Areas.User.Controllers
{
    [ApiController]
    [Area("User")]
    [Route("api/[area]/[controller]/[action]")]
    public class AuthenticationController : Controller
    {
        private readonly ITokenGenerator _tokenGenerator;
        
        private readonly UserManager<ApplicationUser> _userManager;
        
        public AuthenticationController(ITokenGenerator tokenGenerator, UserManager<ApplicationUser> userManager)
        {
            this._tokenGenerator = tokenGenerator;
            this._userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserCredentials userCredentials)
        {
            var user = new ApplicationUser()
            {
                UserName = userCredentials.Username
            };
            IdentityResult result = await _userManager.CreateAsync(user, userCredentials.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return Ok(new { token = _tokenGenerator.GenerateToken(user) });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserCredentials userCredentials)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userCredentials.Username);
            foreach (IPasswordValidator<ApplicationUser> passwordValidator in _userManager.PasswordValidators)
            {
                IdentityResult result = await passwordValidator.ValidateAsync(_userManager, user, userCredentials.Password);
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
            }
            return Ok(new { token = _tokenGenerator.GenerateToken(user) });
        }
    }
}