using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThematicMapCreator.Api.Models
{
    public sealed class ThematicMapDbContext : DbContext
    {
        public ThematicMapDbContext(DbContextOptions options) : base(options)
        {
            Database.MigrateAsync().Wait();
        }

        public DbSet<Layer> Layers { get; set; }
        public DbSet<Map> Maps { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(ConfigureUserEntity);
            modelBuilder.Entity<Map>(ConfigureMapEntity);
            modelBuilder.Entity<Layer>(ConfigureLayerEntity);
        }

        private void ConfigureLayerEntity(EntityTypeBuilder<Layer> builder)
        {
            builder.ToTable("layer").HasKey(layer => layer.Id);

            builder.Property(layer => layer.Id).HasDefaultValueSql("newid()");

            builder.Property(layer => layer.Name)
                .HasColumnName("name")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(layer => layer.Settings)
                .HasColumnName("settings")
                .IsRequired();

            builder.Property(layer => layer.Data)
                .HasColumnName("data")
                .IsRequired();

            builder.HasOne(layer => layer.Map)
                .WithMany(map => map.Layers)
                .HasForeignKey(layer => layer.MapId);
        }

        private void ConfigureMapEntity(EntityTypeBuilder<Map> builder)
        {
            builder.ToTable("map").HasKey(map => map.Id);

            builder.Property(map => map.Id).HasDefaultValueSql("newid()");

            builder.Property(map => map.Name)
                .HasColumnName("name")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(map => map.Settings)
                .HasColumnName("settings")
                .IsRequired();

            builder.HasOne(map => map.User)
                .WithMany(user => user.Maps)
                .HasForeignKey(map => map.UserId);
        }

        private void ConfigureUserEntity(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user").HasKey(user => user.Id);

            builder.Property(user => user.Id).HasDefaultValueSql("newid()");

            builder.Property(user => user.Name)
                .HasColumnName("name")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(user => user.Email)
                .HasColumnName("email")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(user => user.Password)
                .HasColumnName("password")
                .HasMaxLength(64)
                .IsRequired();
        }
    }
}
