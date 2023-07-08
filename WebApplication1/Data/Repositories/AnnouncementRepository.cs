using Microsoft.EntityFrameworkCore;
using System;
using WebAPIAdverts.Data.Repositories.IRepositories;
using WebAPIAdverts.Models;

namespace WebAPIAdverts.Data.Repositories
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly DataContext _dbContext;

        public AnnouncementRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Announcement> GetById(Guid id)
        {
            return await _dbContext.Announcements.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Announcement>> GetByUserId(Guid userId)
        {
            return await _dbContext.Announcements.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task Add(Announcement announcement)
        {
            await _dbContext.Announcements.AddAsync(announcement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Announcement announcement)
        {
            _dbContext.Entry(announcement).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Announcement announcement)
        {
            _dbContext.Announcements.Remove(announcement);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Announcement>> GetFilteredAnnouncements(int? minRating)
        {
            var query = _dbContext.Announcements.AsQueryable();

            if (minRating.HasValue)
            {
                query = query.Where(a => a.Rating >= minRating.Value);
            }

            return await query.ToListAsync();
        }


    }
}
