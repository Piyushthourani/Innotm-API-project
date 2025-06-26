using Innotm_API_project.Data;
using Microsoft.AspNetCore.Mvc;

namespace Innotm_API_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : Controller
    {
        private readonly AppDbContext _context;
        public WalletController(AppDbContext context)
        {
            _context = context;
        }
    }
}
