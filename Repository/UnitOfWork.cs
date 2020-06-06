using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoService.Context;

namespace AutoService.Repository
{
    public class UnitOfWork : IDisposable
    {
        private UserContext db = new UserContext();
        private UserRepository userRepository;
        private UsersServicesRepository usersServicesRepository;
        private ServiceRepository serviceRepository;

        public UserRepository Users
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(db);
                return userRepository;
            }
        }

        public ServiceRepository Services
        {
            get
            {
                if (serviceRepository == null)
                    serviceRepository = new ServiceRepository(db);
                return serviceRepository;
            }
        }

        public UsersServicesRepository Orders
        {
            get
            {
                if (usersServicesRepository == null)
                    usersServicesRepository = new UsersServicesRepository(db);
                return usersServicesRepository;
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
