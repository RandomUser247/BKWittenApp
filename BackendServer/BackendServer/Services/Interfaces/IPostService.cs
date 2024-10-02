using ContentDB.Migrations;

namespace BackendServer.Services.Interfaces
{
    public interface IPostService
    {
        Task<List<Post>> GetRecentPostsAsync(int currentPage, int pageSize);
        Task<List<Post>> GetPendingPostsAsync();
        Task<Post> GetPostByIdAsync(int postId);
        Task CreatePostAsync(Post post, List<IFormFile> images, IFormFile video, string altText);
        Task EditPostAsync(Post post, List<IFormFile> images, IFormFile video, string altText);
        Task DeletePostAsync(int postId);
        Task ConfirmPostAsync(int postId);
        Task<double> GetTotalPostCountAsync();
    }

}
