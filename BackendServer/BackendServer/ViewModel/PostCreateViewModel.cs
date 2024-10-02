using System.ComponentModel.DataAnnotations;

namespace BackendServer.ViewModel
{
    public class PostCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public List<IFormFile> Images { get; set; } = new();

        public IFormFile Video { get; set; }

        public string ImageAltText { get; set; }
    }
}
