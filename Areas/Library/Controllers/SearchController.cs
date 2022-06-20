using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.Library.Models;
using EpubWebLibraryServer.Areas.Library.Services;

namespace EpubWebLibraryServer.Areas.Library.Controllers
{
    [ApiController]
    [Authorize]
    [Area("Library")]
    public class SearchController : Controller
    {
        private readonly EpubManager _epubManager;

        public SearchController(EpubManager epubManager)
        {
            _epubManager = epubManager;
        }

        [HttpGet]
        [Route("/api/epubs/search")]
        public async Task<IActionResult> Search()
        {
            string username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IList<EpubMetadata> metadatas = await _epubManager.Search(username);
            return Ok(metadatas);
        }
    }
}