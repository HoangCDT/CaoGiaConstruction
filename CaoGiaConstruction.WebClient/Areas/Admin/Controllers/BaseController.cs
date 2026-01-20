using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")]
    public class BaseController : Controller
    {
        public BaseController()
        {
        }
    }
}