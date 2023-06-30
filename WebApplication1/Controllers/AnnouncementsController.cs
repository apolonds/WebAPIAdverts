using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly DataContext _context;

        public AnnouncementsController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all adverts.
        /// </summary>
        // GET: api/Announcements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncements()
        {
          if (_context.Announcements == null)
          {
              return NotFound();
          }
            return await _context.Announcements.ToListAsync();
        }

        /// <summary>
        /// Get a specific announcement.
        /// </summary>
        // GET: api/Announcements/5
        [HttpGet("{id}")]
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
        public async Task<IActionResult> PutAnnouncement(Guid id, Announcement announcement)
        {
            if (id != announcement.Id)
            {
                return BadRequest();
            }

            _context.Entry(announcement).State = EntityState.Modified;

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
        public async Task<ActionResult<Announcement>> PostAnnouncement(Announcement announcement)
        {
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
        /// <returns></returns>
        // DELETE: api/Announcements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(Guid id)
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
