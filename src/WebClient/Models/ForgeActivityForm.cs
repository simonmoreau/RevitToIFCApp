using System.ComponentModel.DataAnnotations;

namespace WebClient.Models
{
    public class ForgeActivityForm
    {
        [Required]
        public string? Engine { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? AppbundleFile { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}
