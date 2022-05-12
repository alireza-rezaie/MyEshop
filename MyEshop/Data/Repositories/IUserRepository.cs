using MyEshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyEshop.Data.Repositories
{
    public interface IUserRepository
    {
        bool IsExitsUserByEmail(string email);
        void AddUser(Users user);
        Users GetUserForLogin(string email, string password);
    }
    public class UserRepository : IUserRepository
    {
        private MyEshopContext _context;
        public UserRepository(MyEshopContext context)
        {
            _context = context;
        }
        public void AddUser(Users user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            //mitavanim user ham nnavisim ef core motvage mishavad
            //_context.Add(user);
        }

        public Users GetUserForLogin(string email,string password)
        {
            return _context.Users
                .SingleOrDefault(u => u.Email == email && u.Password == password);
        }

        public bool IsExitsUserByEmail(string email)
        {
            return _context.Users.Any(p => p.Email == email);
        }
    }
}
