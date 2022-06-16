﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyEshop.Data;
using MyEshop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ZarinpalSandbox;

namespace MyEshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyEshopContext _context;


        public HomeController(ILogger<HomeController> logger, MyEshopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Detail(int id)
        {
            var product = _context.Products
                  .Include(p => p.Item)
                  .SingleOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var categeries = _context.Products
                .Where(p => p.Id == id)
                .SelectMany(c => c.CategoryToProducts)
                .Select(ca => ca.Category)
                .ToList();

            var vm = new DetailsViewModel()
            {
                Product = product,
                Categories = categeries
            };
            return View(vm);
        }
        [Authorize]
        public IActionResult AddToCart(int itemId)
        {
            var product = _context.Products
                .Include(p => p.Item)
                .SingleOrDefault(p => p.ItemId == itemId);
            if (product != null)
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
                var order = _context.Order.FirstOrDefault(o => o.UserId == userId && !o.IsFinaly);
                if (order != null)
                {
                    var OrderaDetail = _context.OrderDetails
                        .FirstOrDefault(d => d.OrderId == order.OrderId && d.ProductId == product.Id);
                    if (OrderaDetail != null)
                    {
                        //product.Item.QuantityInStock
                        OrderaDetail.Count += 1;
                    }
                    else
                    {
                        _context.OrderDetails.Add(new OrderDetail()
                        {
                            OrderId = order.OrderId,
                            ProductId = product.Id,
                            Price = product.Item.Price,
                            Count = 1
                        });
                    }
                }
                else
                {
                    order = new Order()
                    {
                        IsFinaly = false,
                        CreatDate = DateTime.Now,
                        UserId = userId,
                    };
                    _context.Order.Add(order);
                    _context.SaveChanges();

                    _context.OrderDetails.Add(new OrderDetail()
                    {
                        OrderId = order.OrderId,
                        ProductId = product.Id,
                        Price = product.Item.Price,
                        Count = 1
                    });
                }
                _context.SaveChanges();
            }
            return RedirectToAction("ShowCart");
        }
        [Authorize]
        public IActionResult RemoveCart(int DetailId)
        {
            var orderdetails = _context.OrderDetails.Find(DetailId);
            //if (orderdetails.Count>1)
            //{
            //    orderdetails.Count -= 1;
            //}
            //else
            //{
            //    _context.Remove(orderdetails);
            //}
            _context.Remove(orderdetails);
            _context.SaveChanges();

            return RedirectToAction("ShowCart");
        }
        [Authorize]
        public IActionResult ShowCart()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
            var order = _context.Order.Where(o => o.UserId == userId && !o.IsFinaly)
                .Include(o => o.OrderDetails)
                .ThenInclude(c => c.Product)
                .SingleOrDefault();

            return View(order);
        }
        public IActionResult Index()
        {
            var products = _context.Products
                .ToList();
            return View(products);
        }
        [Authorize]
        public IActionResult Payment()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var order = _context.Order
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.UserId == userId && !o.IsFinaly);
            if (order == null)
            {

                return NotFound();

            }

            var payment = new Payment((int)order.OrderDetails.Sum(d => d.Price * d.Count));

            var res = payment.PaymentRequest($"پرداخت فاکتور شماره {order.OrderId}",
                "http://localhost:1635/Home/OnlinePayment/" + order.OrderId, "alireza3356864@gmail.com", "09383356867");
            if (res.Result.Status == 100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority);
            }
            else
            {
                return BadRequest();
            }


            return View();
        }
        public IActionResult OnlinePayment(int id)
        {
            if (HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"].ToString().ToLower() == "ok" &&
                HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"].ToString();
                var order = _context.Order.Include(o => o.OrderDetails)
                    .FirstOrDefault(o => o.OrderId == id);
                var payment = new Payment((int)order.OrderDetails.Sum(d => d.Price * d.Count));
                var res = payment.Verification(authority).Result;
                if (res.Status == 100)
                {
                    order.IsFinaly = true;
                    _context.Order.Update(order);
                    _context.SaveChanges();
                    ViewBag.code = res.RefId;
                    return View();
                }
            }

            return View();
        }




        [Route("ContactUs")]
        public IActionResult ContactUs()
        {
            return View();
        }


        public IActionResult PriCvacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
