using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Hayat.DAL.Entities;

namespace Hayat.DAL.Data.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(b => b.BranchId);
            
            builder.Property(b => b.BranchId).HasValueGenerator<UuidV7Generator>();
            builder.Property(b => b.BranchName).IsRequired().HasMaxLength(150);
            builder.Property(b => b.Phone).HasMaxLength(20);
            builder.Property(b => b.City).HasMaxLength(100);

            builder.HasMany(b => b.Clinics)
                   .WithOne(c => c.Branch)
                   .HasForeignKey(c => c.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
