using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.Library.Models;
using EpubWebLibraryServer.Areas.Library.Services;

namespace EpubWebLibraryServer.Areas.Library.Controllers
{
    [ApiController]
    [Authorize]
    [Area("Library")]
    public class MetadataController : Controller
    {
        private readonly EpubManager _epubManager;

        public MetadataController(EpubManager epubManager)
        {
            this._epubManager = epubManager;
        }

        [HttpGet]
        [Route("/api/epubs/metadata/{epubId}")]
        public async Task<IActionResult> GetEpubMetadata(int epubId)
        {
            string username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            EpubMetadata metadata = await _epubManager.GetEpubMetadataAsync(epubId);
            if (metadata is null)
            {
                return NotFound();
            }
            if (!String.Equals(metadata.Owner, username))
            {
                return Unauthorized();
            }
            return Ok(metadata);
        }
    }
}