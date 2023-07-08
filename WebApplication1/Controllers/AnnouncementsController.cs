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

        //[HttpGet]
        //public async Task<IActionResult> GetAllAnnouncements()
        //{
        //    var announcementDtos = await _mediator.GetAllAnnouncements();
        //    return Ok(announcementDtos);
        //}
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


        ///// <summary>
        ///// Get all adverts.
        ///// </summary>
        //// GET: api/Announcements
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncements([FromQuery] int? minRating,
        //    string? sortBy = null, string? sortOrder = null)
        //{

        //    IQueryable<Announcement> query = _context.Announcements;

        //    if (minRating.HasValue)
        //    {
        //        query = query.Where(a => a.Rating >= minRating.Value);
        //    }

        //    // Применение сортировки
        //    if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
        //    {
        //        switch (sortBy.ToLower())
        //        {
        //            case "number":
        //                query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(a => a.Number) : query.OrderBy(a => a.Number);
        //                break;
        //            case "rating":
        //                query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(a => a.Rating) : query.OrderBy(a => a.Rating);
        //                break;
        //            case "createdat":
        //                query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt);
        //                break;
        //            case "expirationdate":
        //                query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(a => a.ExpirationDate) : query.OrderBy(a => a.ExpirationDate);
        //                break;
        //            default:
        //                // По умолчанию сортировка по ID
        //                query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Id);
        //                break;
        //        }
        //    }

        //    var announcements = await query.ToListAsync();

        //    if (announcements == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(_mapper.Map<List<AnnouncementDTO>>(announcements));
        //}

        ///// <summary>
        ///// Get a specific announcement.
        ///// </summary>
        //// GET: api/Announcements/5
        //[HttpGet("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<Announcement>> GetAnnouncement(Guid id)
        //{
        //    if (_context.Announcements == null)
        //    {
        //        return NotFound();
        //    }
        //    var announcement = await _context.Announcements.FindAsync(id);

        //    if (announcement == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(_mapper.Map<List<AnnouncementDTO>>(announcement));
        //}

        ///// <summary>
        ///// Put a specific announcement.
        ///// </summary>
        //// PUT: api/Announcements/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(Guid id, AnnouncementDTO updatedAnnouncementDto)
        //{
        //    if (id != updatedAnnouncementDto.Id)
        //    {
        //        return BadRequest();
        //    }
        //    var announcement = await _context.Announcements.FindAsync(id);
        //    if (announcement == null)
        //    {
        //        return NotFound();
        //    }

        //    // Проверка, принадлежит ли объявление текущему пользователю
        //    if (announcement.UserId != updatedAnnouncementDto.UserId)
        //    {
        //        return Forbid("Нельзя редактировать объявления других пользователей.");
        //    }

        //    _mapper.Map(updatedAnnouncementDto, announcement);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!AnnouncementExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        ///// <summary>
        ///// Post a specific announcement.
        ///// </summary>
        //// POST: api/Announcements
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<AnnouncementDTO>> PostAnnouncement(AnnouncementDTO announcementDto)
        //{
        //    // Получение настроек ограничения из конфигурации
        //    var maxAnnouncementsPerUser = _configuration.GetValue<int>("MaxAnnouncementsPerUser");

        //    // Проверка количества объявлений пользователя
        //    var userAnnouncementCount = await _context.Announcements.CountAsync(a => a.UserId == announcementDto.UserId);
        //    if (userAnnouncementCount >= maxAnnouncementsPerUser)
        //    {
        //        return BadRequest("Превышено максимальное количество объявлений для пользователя.");
        //    }

        //    var announcement = _mapper.Map<Announcement>(announcementDto);
        //    _context.Announcements.Add(announcement);
        //    await _context.SaveChangesAsync();

        //    var createdAnnouncementDto = _mapper.Map<AnnouncementDTO>(announcement);

        //    return CreatedAtAction("GetAnnouncement", new { id = announcement.Id }, createdAnnouncementDto);
        //}

        ///// <summary>
        ///// Deletes a specific announcement.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //// DELETE: api/Announcements/5
        //[HttpDelete("{id},{userId}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> DeleteAnnouncement(Guid id, Guid userId)
        //{

        //    if (id == Guid.Empty)
        //    {
        //        return BadRequest();
        //    }

        //    var announcement = await _context.Announcements.FindAsync(id);
        //    if (announcement == null)
        //    {
        //        return NotFound();
        //    }

        //    // Проверка, что пользователь является владельцем объявления
        //    if (announcement.UserId != userId)
        //    {
        //        return BadRequest("Нельзя удалять объявления других пользователей.");
        //    }

        //    _context.Announcements.Remove(announcement);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool AnnouncementExists(Guid id)
        //{
        //    return (_context.Announcements?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
    }
        }
