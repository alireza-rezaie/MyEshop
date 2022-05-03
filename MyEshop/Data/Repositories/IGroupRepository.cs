using MyEshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEshop.Data.Repositories
{
    public interface IGroupRepository
    {
        IEnumerable<Category> GetAllCategories();
        IEnumerable<ShowGroupViewModel> GetGroupForShow();


    }
    public class GroupRepository : IGroupRepository
    {
        private MyEshopContext _context;
        public GroupRepository(MyEshopContext context)
        {
            _context = context;
        }
        public IEnumerable<Category> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ShowGroupViewModel> GetGroupForShow()
        {
           return _context.Categories
           .Select(c => new ShowGroupViewModel()
           {
               GroupId = c.Id,
               Name = c.Name,
               ProductCount = _context.CategoryToProducts.Count(g => g.CategoryId == c.Id)
           }).ToList();
        }
    }
}
