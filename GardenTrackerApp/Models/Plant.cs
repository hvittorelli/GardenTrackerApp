using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace GardenTrackerApp.Models
{
    public class Plant
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        [StringLength(100)]
        public string? Name {  get; set; }

        [StringLength(50)]
        public string? WaterFrequency { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
