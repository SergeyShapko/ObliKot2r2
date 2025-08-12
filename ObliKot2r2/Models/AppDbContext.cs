using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ObliKot2r2.Models;

namespace ObliKot2r2.Models
{

    public class AppDbContext : DbContext
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Philia> Philia { get; set; }
        public DbSet<Subdivision> Subdivisions { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<GasMeasuringObject> GasMeasuringObjects { get; set; }
        public DbSet<PointIO> PointIOs { get; set; }
        public DbSet<PointIOType> PointIOTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация для PointIOType
            modelBuilder.Entity<PointIOType>().HasData(
                new PointIOType { IdTypePointIO = 1, NameTypePointIO = "Точка входу" },
                new PointIOType { IdTypePointIO = 2, NameTypePointIO = "Точка виходу" },
                new PointIOType { IdTypePointIO = 3, NameTypePointIO = "Точка входу-виходу" },
                new PointIOType { IdTypePointIO = 4, NameTypePointIO = "Віртуальна точка входу" },
                new PointIOType { IdTypePointIO = 5, NameTypePointIO = "Віртуальна точка виходу" },
                new PointIOType { IdTypePointIO = 6, NameTypePointIO = "Віртуальна точка входу-виходу" }
           );
            // Другие конфигурации моделей...
    }
            public DbSet<MeasuringPipe> MeasuringPipes { get; set; }
    }

}
