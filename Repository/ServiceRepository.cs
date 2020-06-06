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
    public class ServiceRepository : IRepository<DataModels.Service>
    {
        private UserContext db;

        public ServiceRepository(UserContext userContext)
        {
            this.db = userContext;
        }

        public IEnumerable<DataModels.Service> GetItemList()
        {
            return db.Services;
        }

        public DataModels.Service GetItem(int id)
        {
            return db.Services.Find(id);
        }

        public void Create(DataModels.Service service)
        {
            db.Services.Add(service);
        }

        public void Update(DataModels.Service service)
        {
            db.Entry(service).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            DataModels.Service service = db.Services.Find(id);
            if (service != null)
                db.Services.Remove(service);
        }
    }
}
