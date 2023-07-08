using WebAPIAdverts.Models.DTO;

namespace WebAPIAdverts.Service
{
    public interface IMediator
    {
        Task<AnnouncementDTO> GetAnnouncement(Guid id);
        Task<IEnumerable<AnnouncementDTO>> GetAllAnnouncements(int? minRating, string? sortBy, string? sortOrder);
        Task<AnnouncementDTO> CreateAnnouncement(AnnouncementDTO announcementDto);
        Task<AnnouncementDTO> UpdateAnnouncement(Guid id, AnnouncementDTO announcementDto);
        Task<bool> DeleteAnnouncement(Guid id, Guid userId);
        Task<int> GetUserAnnouncementCount(Guid userId);

    }
}
