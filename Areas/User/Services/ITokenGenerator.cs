using EpubWebLibraryServer.Areas.User.Models;

namespace EpubWebLibraryServer.Areas.User.Services
{
    public interface ITokenGenerator
    {
        string GenerateToken(ApplicationUser user);
    }
}