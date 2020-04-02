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
    public class UsersController : ControllerBase
    {
        private readonly ThematicMapDbContext context;

        public UsersController(ThematicMapDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> Get() => await context.Users.AsNoTracking().ToListAsync();
    }
}
