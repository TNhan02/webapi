using System;
using System.Collections.Generic;

namespace webapi.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string UserRole { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? PasswordHash { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
