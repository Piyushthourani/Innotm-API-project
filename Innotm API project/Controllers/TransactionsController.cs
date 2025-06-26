using Innotm_API_project.Data;
using Microsoft.AspNetCore.Mvc;

namespace Innotm_API_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _context;
        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }
    }
}
