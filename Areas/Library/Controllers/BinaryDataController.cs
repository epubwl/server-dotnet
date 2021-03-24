using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.Library.Models;
using EpubWebLibraryServer.Areas.Library.Services;

namespace EpubWebLibraryServer.Areas.Library.Controllers
{
    [ApiController]
    [Authorize]
    [Area("Library")]
    public class BinaryDataController : Controller
    {
        private readonly EpubManager _epubManager;

        private const string _epubMimetype = "application/epub+zip";

        public BinaryDataController(EpubManager epubManager)
        {
            _epubManager = epubManager;
        }

        [HttpPost]
        [Consumes(_epubMimetype)]
        [Route("/api/epubs")]
        public async Task<IActionResult> UploadNewEpub()
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
            return File(binaryStream, _epubMimetype);
        }

        [HttpPut]
        [Consumes(_epubMimetype)]
        [Route("/api/epubs/{epubId}")]
        public async Task<IActionResult> UploadAndReplaceEpub(int epubId)
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
            metadata = await _epubManager.ReplaceEpubAsync(epubId, username, Request.Body);
            return Ok(metadata);
        }

        [HttpDelete]
        [Route("/api/epubs/{epubId}")]
        public async Task<IActionResult> DeleteEpub(int epubId)
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
            metadata = await _epubManager.DeleteEpubAsync(epubId);
            return Ok(metadata);
        }
    }
}