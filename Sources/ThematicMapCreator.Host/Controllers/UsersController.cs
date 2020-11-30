using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain.Services;
using ThematicMapCreator.Host.Extensions;

namespace ThematicMapCreator.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUsersService service;

        public UsersController(IUsersService service)
        {
            this.service = service;
        }

        [HttpGet("{id:guid}/maps")]
        public async Task<List<MapDto>> GetMapsAsync(Guid id)
        {
            return (await service.GetMapsAsync(id)).ConvertAll(map => map.ToDto());
        }
    }
}
