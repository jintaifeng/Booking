using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Booking.Data.Models.Mapping
{
    public class golf_courseMap : EntityTypeConfiguration<golf_course>
    {
        public golf_courseMap()
        {
            // Primary Key
            this.HasKey(t => t.course_id);

            // Properties
            this.Property(t => t.course_name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.description)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("golf_course");
            this.Property(t => t.course_id).HasColumnName("course_id");
            this.Property(t => t.course_name).HasColumnName("course_name");
            this.Property(t => t.description).HasColumnName("description");
        }
    }
}
