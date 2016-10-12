using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Booking.Data.Models.Mapping
{
    public class rbac_rolesMap : EntityTypeConfiguration<rbac_roles>
    {
        public rbac_rolesMap()
        {
            // Primary Key
            this.HasKey(t => t.role_id);

            // Properties
            this.Property(t => t.role_name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.description)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("rbac_roles");
            this.Property(t => t.role_id).HasColumnName("role_id");
            this.Property(t => t.role_name).HasColumnName("role_name");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.tstamp).HasColumnName("tstamp");
        }
    }
}
