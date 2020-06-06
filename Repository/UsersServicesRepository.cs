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
    public class UsersServicesRepository : IRepository<UsersServices>
    {
        private UserContext db;

        public UsersServicesRepository(UserContext userContext)
        {
            this.db =userContext;
        }

        public IEnumerable<UsersServices> GetItemList()
        {
            return db.UsersServices;
        }

        public UsersServices GetItem(int id)
        {
            return db.UsersServices.Find(id);
        }

        public void Create(UsersServices usersServices)
        {
            db.UsersServices.Add(usersServices);
        }

        public void Update(UsersServices userServices)
        {
            db.Entry(userServices).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            UsersServices usersServices = db.UsersServices.Find(id);
            if (usersServices != null)
                db.UsersServices.Remove(usersServices);
        }
    }
}
