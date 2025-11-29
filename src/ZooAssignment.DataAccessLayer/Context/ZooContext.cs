using Microsoft.EntityFrameworkCore;
using ZooAssignment.DataAccessLayer.Models;

namespace ZooAssignment.DataAccessLayer.Context
{
    public class ZooContext : DbContext
    {
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<FoodPrice> FoodPrices { get; set; }

        public ZooContext(DbContextOptions<ZooContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Animal entity
            modelBuilder.Entity<Animal>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Animal>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Animal>()
                .Property(a => a.Name)
                .IsRequired();

            // Configure FoodPrice entity
            modelBuilder.Entity<FoodPrice>()
                .HasKey(fp => fp.Id);

            modelBuilder.Entity<FoodPrice>()
                .Property(fp => fp.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<FoodPrice>()
                .Property(fp => fp.FoodType)
                .IsRequired();

            modelBuilder.Entity<AnimalType>()
                .HasKey(at => at.Id);

            modelBuilder.Entity<AnimalType>()
                .Property(at => at.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AnimalType>()
                .Property(at => at.TypeName)
                .IsRequired();

            // Configure relationship between Animal and AnimalType
            modelBuilder.Entity<Animal>()
                .HasOne(a => a.Type)
                .WithMany(at => at.Animals)
                .HasForeignKey(a => a.AnimalTypeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
