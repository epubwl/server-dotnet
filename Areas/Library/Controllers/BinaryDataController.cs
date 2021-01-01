using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.Library.Data;
using EpubWebLibraryServer.Areas.Library.Services;

namespace EpubWebLibraryServer.Areas.Library.Controllers
{
    [ApiController]
    [Authorize]
    [Area("Library")]
    public class BinaryDataController : Controller
    {
        private readonly EpubManager _epubManager;

        public BinaryDataController(EpubManager epubManager)
        {
            this._epubManager = epubManager;
        }

        [HttpPost]
        [Route("/api/epubs")]
        public async Task<IActionResult> UploadEpub()
        {
            string username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            EpubMetadata metadata = await _epubManager.AddEpubAsync(username, Request.Body);
            return Ok(metadata);
        }

        [HttpGet]
        [Route("/api/epubs/{epubId}")]
        public async Task<IActionResult> DownloadEpub(int epubId)
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
            Stream binaryStream = await _epubManager.GetEpubAsync(epubId);
            return File(binaryStream, "application/epub+zip");
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