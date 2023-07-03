using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/Announcements")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        public AnnouncementsController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Get all adverts.
        /// </summary>
        // GET: api/Announcements
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncements([FromQuery] int? minRating,
            string? sortBy = null, string? sortOrder = null)
        {
   
            IQueryable<Announcement> query = _context.Announcements;

            if (minRating.HasValue)
            {
                query = query.Where(a => a.Rating >= minRating.Value);
            }

            // Применение сортировки
            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                    case "number":
                        query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(a => a.Number) : query.OrderBy(a => a.Number);
                        break;
                    case "rating":
                        query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(a => a.Rating) : query.OrderBy(a => a.Rating);
                        break;
                    case "createdat":
                        query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt);
                        break;
                    case "expirationdate":
                        query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(a => a.ExpirationDate) : query.OrderBy(a => a.ExpirationDate);
                        break;
                    default:
                        // По умолчанию сортировка по ID
                        query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Id);
                        break;
                }
            }

            var announcements = await query.ToListAsync();

            if (announcements == null)
            {
                return NotFound();
            }

            return announcements;
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
        public async Task<ActionResult<Announcement>> GetAnnouncement(Guid id)
        {
          if (_context.Announcements == null)
          {
              return NotFound();
          }
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement == null)
            {
                return NotFound();
            }

            return announcement;
        }

        /// <summary>
        /// Put a specific announcement.
        /// </summary>
        // PUT: api/Announcements/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
         [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Announcement updatedAnnouncement)
        {
            if (id != updatedAnnouncement.Id)
            {
                return BadRequest();
            }
            var announcement = _context.Announcements.FirstOrDefault(a => a.Id == id);
            if (announcement == null)
            {
                return NotFound();
            }

            // Проверка, принадлежит ли объявление текущему пользователю
            if (announcement.UserId != updatedAnnouncement.UserId)
            {
                return Forbid("Нельзя редактировать объявления других пользователей.");
            }

            // Применение обновлений
            announcement.Number = updatedAnnouncement.Number;
            announcement.Text = updatedAnnouncement.Text;
            announcement.Image = updatedAnnouncement.Image;
            announcement.Rating = updatedAnnouncement.Rating;
            announcement.CreatedAt = updatedAnnouncement.CreatedAt;
            announcement.ExpirationDate = updatedAnnouncement.ExpirationDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnouncementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        /// <summary>
        /// Post a specific announcement.
        /// </summary>
        // POST: api/Announcements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Announcement>> PostAnnouncement(Announcement announcement)
        {
            // Получение настроек ограничения из конфигурации
            var maxAnnouncementsPerUser = _configuration.GetValue<int>("MaxAnnouncementsPerUser");

            // Проверка количества объявлений пользователя
            var userAnnouncementCount = _context.Announcements.Count(a => a.UserId == announcement.UserId);
            if (userAnnouncementCount >= maxAnnouncementsPerUser)
            {
                return BadRequest("Превышено максимальное количество объявлений для пользователя.");
            }

            if (_context.Announcements == null)
            {
                return Problem("Entity set 'DataContext.Announcements'  is null.");
            }
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnnouncement", new { id = announcement.Id }, announcement);
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
            if (_context.Announcements == null)
            {
                return NotFound();
            }
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }

            // Проверка, что пользователь является владельцем объявления
            if (announcement.UserId != userId)
            {
                return BadRequest("Нельзя удалять объявления других пользователей.");
            }

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        private bool AnnouncementExists(Guid id)
        {
            return (_context.Announcements?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
