using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Booking.Data.Models.Mapping;

namespace Booking.Data.Models
{
    public partial class GolfBookingContext : DbContext
    {
        static GolfBookingContext()
        {
            Database.SetInitializer<GolfBookingContext>(null);
        }

        public GolfBookingContext()
            : base("Name=GolfBookingContext")
        {
        }

        public DbSet<golf_course> golf_course { get; set; }
        public DbSet<order> orders { get; set; }
        public DbSet<rbac_objects> rbac_objects { get; set; }
        public DbSet<rbac_role_object> rbac_role_object { get; set; }
        public DbSet<rbac_roles> rbac_roles { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<user> users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new golf_courseMap());
            modelBuilder.Configurations.Add(new orderMap());
            modelBuilder.Configurations.Add(new rbac_objectsMap());
            modelBuilder.Configurations.Add(new rbac_role_objectMap());
            modelBuilder.Configurations.Add(new rbac_rolesMap());
            modelBuilder.Configurations.Add(new sysdiagramMap());
            modelBuilder.Configurations.Add(new userMap());
        }
    }
}
