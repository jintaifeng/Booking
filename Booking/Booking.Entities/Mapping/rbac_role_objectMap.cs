using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Booking.Data.Models.Mapping
{
    public class rbac_role_objectMap : EntityTypeConfiguration<rbac_role_object>
    {
        public rbac_role_objectMap()
        {
            // Primary Key
            this.HasKey(t => t.rro_id);

            // Properties
            this.Property(t => t.object_id)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.description)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("rbac_role_object");
            this.Property(t => t.rro_id).HasColumnName("rro_id");
            this.Property(t => t.roles_id).HasColumnName("roles_id");
            this.Property(t => t.object_id).HasColumnName("object_id");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.tstamp).HasColumnName("tstamp");

            // Relationships
            this.HasRequired(t => t.rbac_objects)
                .WithMany(t => t.rbac_role_object)
                .HasForeignKey(d => d.object_id);
            this.HasRequired(t => t.rbac_roles)
                .WithMany(t => t.rbac_role_object)
                .HasForeignKey(d => d.roles_id);

        }
    }
}
