using Microsoft.AspNetCore.Mvc;
using MyEshop.Data;
using MyEshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEshop.Components
{
    public class ProductGroupsComponent : ViewComponent
    {
        MyEshopContext _context;
        public ProductGroupsComponent(MyEshopContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            return View("/Views/Components/ProductGroupsComponents.cshtml", categories);
        }
    }
}
