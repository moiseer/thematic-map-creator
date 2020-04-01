using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThematicMapCreator.Api.Models;

namespace ThematicMapCreator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ThematicMapDbContext context;

        public UserController(ThematicMapDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> Get() => await context.Users.ToListAsync();
    }
}
