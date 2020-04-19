using System;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThematicMapCreator.Api.Contracts;
using ThematicMapCreator.Api.Models;

namespace ThematicMapCreator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : Controller
    {
        private readonly ThematicMapDbContext context;

        public AuthorizationController(ThematicMapDbContext context)
        {
            this.context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserOverview>> Login(AuthorizationRequest request)
        {
            var response = await context.Users
                .Where(user => user.Email == request.Email)
                .FirstOrDefaultAsync(user => user.Password == request.Password);

            if (response == null)
            {
                return BadRequest("Invalid email or password.");
            }

            return Json(response.Adapt<UserOverview>());
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            return Ok(true);
        }

        [HttpPost("signin")]
        public async Task<ActionResult<UserOverview>> Signin(RegistrationRequest request)
        {
            var nameExists = await context.Users.AnyAsync(user => user.Name == request.Name);
            if (nameExists)
            {
                return BadRequest("Name not unique.");
            }

            var emailExists = await context.Users.AnyAsync(user => user.Email == request.Email);
            if (emailExists)
            {
                return BadRequest("Email not unique.");
            }

            var newUser = request.Adapt<User>();
            newUser.Id = Guid.NewGuid();

            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();

            return Json(newUser.Adapt<UserOverview>());
        }
    }
}
