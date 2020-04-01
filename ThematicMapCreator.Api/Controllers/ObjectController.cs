using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ThematicMapCreator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObjectController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<object> Get()
        {
            return Enumerable.Empty<object>();
        }
    }
}
