using System.ComponentModel.DataAnnotations;

namespace BackendServer.ViewModel
{
    public class EventCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string Description { get; set; }
    }
}
