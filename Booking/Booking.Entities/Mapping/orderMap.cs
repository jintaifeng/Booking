using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Booking.Data.Models.Mapping
{
    public class orderMap : EntityTypeConfiguration<order>
    {
        public orderMap()
        {
            // Primary Key
            this.HasKey(t => t.order_id);

            // Properties
            this.Property(t => t.booking_name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.phone)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.order_status)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.bookling_status)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.settle_status)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.description)
                .HasMaxLength(200);

            this.Property(t => t.agency)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("order");
            this.Property(t => t.order_id).HasColumnName("order_id");
            this.Property(t => t.user_id).HasColumnName("user_id");
            this.Property(t => t.course_id).HasColumnName("course_id");
            this.Property(t => t.booking_name).HasColumnName("booking_name");
            this.Property(t => t.phone).HasColumnName("phone");
            this.Property(t => t.order_status).HasColumnName("order_status");
            this.Property(t => t.bookling_status).HasColumnName("bookling_status");
            this.Property(t => t.settle_status).HasColumnName("settle_status");
            this.Property(t => t.member_number).HasColumnName("member_number");
            this.Property(t => t.appointment_time).HasColumnName("appointment_time");
            this.Property(t => t.deposit).HasColumnName("deposit");
            this.Property(t => t.pay_balance).HasColumnName("pay_balance");
            this.Property(t => t.total).HasColumnName("total");
            this.Property(t => t.tax).HasColumnName("tax");
            this.Property(t => t.tstamp).HasColumnName("tstamp");
            this.Property(t => t.description).HasColumnName("description");
            this.Property(t => t.agency).HasColumnName("agency");

            // Relationships
            this.HasRequired(t => t.golf_course)
                .WithMany(t => t.orders)
                .HasForeignKey(d => d.course_id);
            this.HasRequired(t => t.user)
                .WithMany(t => t.orders)
                .HasForeignKey(d => d.user_id);

        }
    }
}
