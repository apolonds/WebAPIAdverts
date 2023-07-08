using WebAPIAdverts.Models;

namespace WebAPIAdverts.Data.Repositories.IRepositories
{
    public interface IAnnouncementRepository
    {
        Task<Announcement> GetById(Guid id);
        Task<IEnumerable<Announcement>> GetByUserId(Guid userId);
        Task Add(Announcement announcement);
        Task Update(Announcement announcement);
        Task Delete(Announcement announcement);
        Task<IEnumerable<Announcement>> GetFilteredAnnouncements(int? minRating);

    }
}
