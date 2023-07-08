using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAdverts.Data;
using WebAPIAdverts.Models;
using WebAPIAdverts.Models.DTO;
using WebAPIAdverts.Service;

namespace WebAPIAdverts.Controllers
{
    [Route("api/Announcements")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public AnnouncementsController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Get all adverts.
        /// </summary>
        // GET: api/Announcements
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AnnouncementDTO>>> GetAllAnnouncements([FromQuery] int? minRating,
            string? sortBy = null, string? sortOrder = null)
        {
            var announcements = await _mediator.GetAllAnnouncements(minRating, sortBy, sortOrder);
            return Ok(announcements);
        }

        /// <summary>
        /// Get a specific announcement.
        /// </summary>
        // GET: api/Announcements/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAnnouncement(Guid id)
        {
            var announcementDto = await _mediator.GetAnnouncement(id);
            if (announcementDto == null)
                return NotFound();

            return Ok(announcementDto);
        }

        /// <summary>
        /// Post a specific announcement.
        /// </summary>
        // POST: api/Announcements
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAnnouncement([FromBody] AnnouncementDTO announcementDto)
        {
            var userId = announcementDto.UserId;
            var userAnnouncementCount = await _mediator.GetUserAnnouncementCount(userId);
            var userAnnouncementLimit = GetUserAnnouncementLimit();

            if (userAnnouncementCount >= userAnnouncementLimit)
            {
                return BadRequest($"User has reached the maximum number of announcements ({userAnnouncementLimit}).");
            }

            var createdAnnouncementDto = await _mediator.CreateAnnouncement(announcementDto);
            return Ok(createdAnnouncementDto);
        }

        /// <summary>
        /// Put a specific announcement.
        /// </summary>
        // PUT: api/Announcements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnnouncement(Guid id, [FromBody] AnnouncementDTO announcementDto)
        {
            var updatedAnnouncementDto = await _mediator.UpdateAnnouncement(id, announcementDto);
            if (updatedAnnouncementDto == null)
            {
                return NotFound();
            }

            return Ok(updatedAnnouncementDto);
        }

        /// <summary>
        /// Deletes a specific announcement.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        // DELETE: api/Announcements/5
        [HttpDelete("{id},{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAnnouncement(Guid id, Guid userId)
        {
            var result = await _mediator.DeleteAnnouncement(id, userId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        private int GetUserAnnouncementLimit()
        {
            var limit = _configuration.GetValue<int>("MaxAnnouncementsPerUser");
            return limit;
        }

    }
}
