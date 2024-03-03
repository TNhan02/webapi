namespace webapi.Models.DTO
{
    public class DTOUser
    {
        public int? Id { get; set; }
        public string? UserRole { get; set; } = null!;
        public string? Username { get; set; } = null!;
        public string? Password { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
