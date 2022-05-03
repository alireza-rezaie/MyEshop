using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEshop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEshop.Controllers
{
    public class ProductController : Controller
    {
        private MyEshopContext _context;
        public ProductController(MyEshopContext context)
        {
            _context = context;
        }
        [Route("Group/{id}/{name}")]
        public IActionResult ShowProductByGroupId(int id,string name)
        {
            ViewData["GroupName"] = name;
            var Poducts = _context.CategoryToProducts
                .Where(p => p.CategoryId == id)
                .Include(c=>c.Product)
                .Select(c => c.Product)
                .ToList();
            return View(Poducts);
        }
    }
}
