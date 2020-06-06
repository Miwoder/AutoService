using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using AutoService.DataModels;

namespace AutoService.Context
{
    public class UserContext : DbContext
    {
        public UserContext() :
            base("ServiceDB")
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<DataModels.Service> Services { get; set; }
        public DbSet<UsersServices> UsersServices { get; set; }


    }
}
