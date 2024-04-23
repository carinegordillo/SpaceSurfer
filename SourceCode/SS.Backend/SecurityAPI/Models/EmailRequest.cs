using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string TargetEmail { get; set; }
    }
}
