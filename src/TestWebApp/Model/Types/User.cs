using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWebApp.Model.Types
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_name")]
        public string UserName { get; set; } = null!;

        [Required]
        [Column("pass_hash")]
        public string PassHash { get; set; } = null!;

        [Column("session")]
        public string Session { get; set; } = string.Empty;
    }
}
