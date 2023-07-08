using AutoMapper;
using WebAPIAdverts.Data.Repositories.IRepositories;
using WebAPIAdverts.Models.DTO;
using WebAPIAdverts.Models;
using System.Configuration;

namespace WebAPIAdverts.Service
{
    public class Mediator : IMediator
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IMapper _mapper;

        public Mediator(IAnnouncementRepository announcementRepository, IMapper mapper)
        {
            _announcementRepository = announcementRepository;
            _mapper = mapper;
        }

        public async Task<int> GetUserAnnouncementCount(Guid userId)
        {
            var userAnnouncements = await _announcementRepository.GetByUserId(userId);
            return userAnnouncements.Count();
        }

        public async Task<AnnouncementDTO> GetAnnouncement(Guid id)
        {
            var announcement = await _announcementRepository.GetById(id);
            return _mapper.Map<AnnouncementDTO>(announcement);
        }

        public async Task<IEnumerable<AnnouncementDTO>> GetAllAnnouncements(int? minRating, string? sortBy, string? sortOrder)
        {
            var announcements = await _announcementRepository.GetFilteredAnnouncements(minRating);
            announcements = SortAnnouncements(announcements, sortBy, sortOrder);
            return _mapper.Map<IEnumerable<AnnouncementDTO>>(announcements);
        }

        private IEnumerable<Announcement> SortAnnouncements(IEnumerable<Announcement> announcements, string? sortBy, string? sortOrder)
        {
            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                    case "number":
                        announcements = sortOrder.ToLower() == "desc" ? announcements.OrderByDescending(a => a.Number) : announcements.OrderBy(a => a.Number);
                        break;
                    case "rating":
                        announcements = sortOrder.ToLower() == "desc" ? announcements.OrderByDescending(a => a.Rating) : announcements.OrderBy(a => a.Rating);
                        break;
                    case "createdat":
                        announcements = sortOrder.ToLower() == "desc" ? announcements.OrderByDescending(a => a.CreatedAt) : announcements.OrderBy(a => a.CreatedAt);
                        break;
                    case "expirationdate":
                        announcements = sortOrder.ToLower() == "desc" ? announcements.OrderByDescending(a => a.ExpirationDate) : announcements.OrderBy(a => a.ExpirationDate);
                        break;
                    default:
                        // По умолчанию сортировка по ID
                        announcements = sortOrder.ToLower() == "desc" ? announcements.OrderByDescending(a => a.Id) : announcements.OrderBy(a => a.Id);
                        break;
                }
            }

            return announcements;
        }

        public async Task<AnnouncementDTO> CreateAnnouncement(AnnouncementDTO announcementDto)
        {
            var announcement = _mapper.Map<Announcement>(announcementDto);
            await _announcementRepository.Add(announcement);
            return _mapper.Map<AnnouncementDTO>(announcement);
        }

        public async Task<AnnouncementDTO> UpdateAnnouncement(Guid id, AnnouncementDTO announcementDto)
        {
            var announcement = await _announcementRepository.GetById(id);
            if (announcement == null)
                return null;

            _mapper.Map(announcementDto, announcement);
            await _announcementRepository.Update(announcement);

            return _mapper.Map<AnnouncementDTO>(announcement);
        }

        public async Task<bool> DeleteAnnouncement(Guid id, Guid userId)
        {
            var announcement = await _announcementRepository.GetById(id);
            if (announcement == null)
                return false;

            // Проверка, что пользователь является владельцем объявления
            if (announcement.UserId != userId)
                return false;

            await _announcementRepository.Delete(announcement);
            return true;
        }
    }
}
