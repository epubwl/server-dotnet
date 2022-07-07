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

        public BinaryDataController(EpubManager epubManager)
        {
            _epubManager = epubManager;
        }

        [HttpPost]
        [Consumes(EpubMimeTypes.Application.EpubZip)]
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
            EpubMetadata? metadata = await _epubManager.GetEpubMetadataAsync(epubId);
            if (metadata is null)
            {
                return NotFound();
            }
            if (!String.Equals(metadata.Owner, username))
            {
                return Unauthorized();
            }
            Stream binaryStream = await _epubManager.GetEpubAsync(epubId);
            return File(binaryStream, EpubMimeTypes.Application.EpubZip);
        }

        [HttpPut]
        [Consumes(EpubMimeTypes.Application.EpubZip)]
        [Route("/api/epubs/{epubId}")]
        public async Task<IActionResult> UploadAndReplaceEpub(int epubId)
        {
            string username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            EpubMetadata? metadata = await _epubManager.GetEpubMetadataAsync(epubId);
            if (metadata is null)
            {
                return NotFound();
            }
            if (!String.Equals(metadata.Owner, username))
            {
                return Unauthorized();
            }
            metadata = await _epubManager.ReplaceEpubAsync(epubId, Request.Body);
            return Ok(metadata);
        }

        [HttpDelete]
        [Route("/api/epubs/{epubId}")]
        public async Task<IActionResult> DeleteEpub(int epubId)
        {
            string username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            EpubMetadata? metadata = await _epubManager.GetEpubMetadataAsync(epubId);
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

        [HttpGet]
        [Route("/api/epubs/cover/{epubId}")]
        public async Task<IActionResult> GetCover(int epubId)
        {
            string username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            EpubMetadata? metadata = await _epubManager.GetEpubMetadataAsync(epubId);
            if (metadata is null)
            {
                return NotFound();
            }
            if (!String.Equals(metadata.Owner, username))
            {
                return Unauthorized();
            }
            string mimetype = await _epubManager.GetEpubCoverMimetypeAsync(epubId);
            Stream binaryStream = await _epubManager.GetEpubCoverAsync(epubId);
            return File(binaryStream, mimetype);
        }

        [HttpPut]
        [Consumes(EpubMimeTypes.Image.Gif, new string[] { EpubMimeTypes.Image.Jpeg, EpubMimeTypes.Image.Png, EpubMimeTypes.Image.SvgXml })]
        [Route("/api/epubs/cover/{epubId}")]
        public async Task<IActionResult> UploadAndReplaceEpubCover(int epubId)
        {
            string username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            EpubMetadata? metadata = await _epubManager.GetEpubMetadataAsync(epubId);
            if (metadata is null)
            {
                return NotFound();
            }
            if (!String.Equals(metadata.Owner, username))
            {
                return Unauthorized();
            }
            await _epubManager.ReplaceEpubCoverAsync(epubId, Request.Body, HttpContext.Request.ContentType ?? EpubMimeTypes.Application.OctetStream);
            return NoContent();
        }
    }
}