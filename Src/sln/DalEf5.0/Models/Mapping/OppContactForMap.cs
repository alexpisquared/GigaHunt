using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DalEf5.Models.Mapping
{
    public class OppContactForMap : EntityTypeConfiguration<OppContactFor>
    {
        public OppContactForMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Expr1)
                .HasMaxLength(388);

            // Table & Column Mappings
            this.ToTable("OppContactFor");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Expr1).HasColumnName("Expr1");
        }
    }
}
