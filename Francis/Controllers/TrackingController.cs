using Francis.Database;
using Francis.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Francis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrackingController : ControllerBase
    {
        private readonly BotDbContext _context;


        public TrackingController(BotDbContext context)
        {
            _context = context;
        }


        [HttpGet("users")]
        public List<BotUser> GetUsers()
        {
            return _context.BotUsers
                .OrderByDescending(x => x.UserName)
                .ToList();
        }

        [HttpGet("requests")]
        public List<RequestProgression> GetRequests()
        {
            return _context.RequestProgressions
                .OrderByDescending(x => x.CreationDate)
                .ToList();
        }
    }
}