using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Stock_Management_API.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [NotMapped] // Ignored in database
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
    }
}
