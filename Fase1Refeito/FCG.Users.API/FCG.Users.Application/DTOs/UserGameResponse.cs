namespace FCG.Users.Application.DTOs
{
    public class UserGameResponse
    {
        public Guid GameId { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public DateTime AcquiredAt { get; set; }
    }
}
