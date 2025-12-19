using System.ComponentModel.DataAnnotations.Schema;

namespace Bibliotheque.Models
{
    [Table("user_role")]
    public class UserRole
    {
        public int UserId { get; set; }
        public LibraryUser? User { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
