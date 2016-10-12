using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Booking.Data.Models.Mapping
{
    public class userMap : EntityTypeConfiguration<user>
    {
        public userMap()
        {
            // Primary Key
            this.HasKey(t => t.user_id);

            // Properties
            this.Property(t => t.login_name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.email)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.password)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.phone)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.status)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("user");
            this.Property(t => t.user_id).HasColumnName("user_id");
            this.Property(t => t.login_name).HasColumnName("login_name");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.password).HasColumnName("password");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.role_id).HasColumnName("role_id");
            this.Property(t => t.phone).HasColumnName("phone");
            this.Property(t => t.status).HasColumnName("status");
            this.Property(t => t.update_time).HasColumnName("update_time");
            this.Property(t => t.tstamp).HasColumnName("tstamp");

            // Relationships
            this.HasRequired(t => t.rbac_roles)
                .WithMany(t => t.users)
                .HasForeignKey(d => d.role_id);

        }
    }
}
