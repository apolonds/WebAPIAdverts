using AutoMapper;
using WebAPIAdverts.Models;
using WebAPIAdverts.Models.DTO;

namespace WebAPIAdverts.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
            {
                CreateMap<Announcement, AnnouncementDTO>().ReverseMap();
    }
    }
}
