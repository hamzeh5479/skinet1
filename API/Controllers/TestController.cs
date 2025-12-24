using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
    [HttpGet]
    public string Get()
    {
        return "API is working";
    }
    }
}