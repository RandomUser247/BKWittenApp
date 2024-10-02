using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BackendServer.Pages
{
    [Authorize(Policy = "RequireAdminRole")]
    public class CalendarModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
