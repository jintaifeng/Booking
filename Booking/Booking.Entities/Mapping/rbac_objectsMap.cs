using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration;

namespace Booking.Data.Models.Mapping
{
    public class rbac_objectsMap : EntityTypeConfiguration<rbac_objects>
    {
        public rbac_objectsMap()
        {
            // Primary Key
            this.HasKey(t => t.object_id);

            // Properties
            this.Property(t => t.object_id)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.object_name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.object_url)
                .HasMaxLength(100);

            this.Property(t => t.object_type)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.description)
                .HasMaxLength(200);

            this.Property(t => t.status)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.icon)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("rbac_objects");
            this.Property(t => t.object_id).HasColumnName("object_id");
            this.Property(t => t.object_name).HasColumnName("object_name");
            this.Property(t => t.object_url).HasColumnName("object_url");
            this.Property(t => t.object_type).HasColumnName("object_type");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.status).HasColumnName("status");
            this.Property(t => t.tstamp).HasColumnName("tstamp");
            this.Property(t => t.icon).HasColumnName("icon");
            this.Property(t => t.sort).HasColumnName("sort");
        }
    }
}
