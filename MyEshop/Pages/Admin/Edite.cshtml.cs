using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyEshop.Data;
using MyEshop.Models;

namespace MyEshop.Pages.Admin
{
    public class EditeModel : PageModel
    {
        MyEshopContext _context;
        public EditeModel(MyEshopContext context)
        {
            _context = context;
        }
        [BindProperty]
        public AddEditeProductViewModel Product { get; set; }
        [BindProperty]
        public List<int> SelectedGroups { get; set; }
        public List<int> GroupProduct { get; set; }
        public void OnGet(int id)
        {
            var product = _context.Products
                .Include(p => p.Item)
                .Where(p => p.Id == id)
                .Select(s => new AddEditeProductViewModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    QuantityInStock = s.Item.QuantityInStock,
                    Price = s.Item.Price
                }).FirstOrDefault();
            Product = product;

            Product.Categories = _context.Categories.ToList();
            GroupProduct = _context.CategoryToProducts.Where(c => c.ProductId == id)
                .Select(s => s.CategoryId).ToList();
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var product = _context.Products.Find(Product.Id);
            var item = _context.Items.First(p => p.Id == product.ItemId);

            product.Name = Product.Name;
            product.Description = Product.Name;
            item.Price = Product.Price;
            item.QuantityInStock = Product.QuantityInStock;
            _context.SaveChanges();
            if (Product.Picture?.Length > 0)
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(),
                   "wwwroot",
                   "images",
                   product.Id + Path.GetExtension(Product.Picture.FileName));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Product.Picture.CopyTo(stream);
                }
            }
            //delete all CategoryToProducts
            _context.CategoryToProducts.Where(c => c.ProductId == product.Id).ToList()
                .ForEach(g => _context.CategoryToProducts.Remove(g));
            
            if (SelectedGroups.Any() && SelectedGroups.Count > 0)
            {
                foreach (int gr in SelectedGroups)
                {
                    _context.CategoryToProducts.Add(new CategoryToProduct()
                    {
                        CategoryId = gr,
                        ProductId = Product.Id
                    });
                }
                _context.SaveChanges();
            }

            return RedirectToPage("Index");
        }
    }
}
