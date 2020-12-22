using EpubWebLibraryServer.Areas.User.Data;

namespace EpubWebLibraryServer.Areas.User.Services
{
    public interface ITokenGenerator
    {
        string GenerateToken(ApplicationUser user);
    }
}