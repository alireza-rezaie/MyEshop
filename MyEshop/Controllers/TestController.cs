using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEshop.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        public string Test1()
        {
            return "test1";
        }
        //[AllowAnonymous]
        public string Test2()
        {
            return "test2";
        }
    }
}
