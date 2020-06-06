using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoService.Context;
using AutoService.DataModels;

namespace AutoService.Repository
{
    public class UserRepository : IRepository<User>
    {
        private UserContext db;

        public UserRepository(UserContext userContext)
        {
            this.db =userContext;
        }

        public IEnumerable<User> GetItemList()
        {
            return db.Users;
        }

        public User GetItem(int id)
        {
            return db.Users.Find(id);
        }

        public void Create(User user)
        {
            db.Users.Add(user);
        }

        public void Update(User user)
        {
            db.Entry(user).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            User user = db.Users.Find(id);
            if (user != null)
                db.Users.Remove(user);
        }
    }
}
