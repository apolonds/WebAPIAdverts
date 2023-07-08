using System.ComponentModel.DataAnnotations;

namespace WebAPIAdverts.Models.DTO
{
    public class AnnouncementDTO
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }

    }
}
