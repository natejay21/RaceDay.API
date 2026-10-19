namespace RaceDay.API.DTOs
{
    public class AuthResponseDto
    {
        public string Message { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
    }
}