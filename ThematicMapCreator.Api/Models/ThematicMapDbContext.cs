using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThematicMapCreator.Api.Models
{
    public sealed class ThematicMapDbContext : DbContext
    {
        public ThematicMapDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Layer> Layers { get; set; }
        public DbSet<Map> Maps { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(ConfigureUserEntity);
            modelBuilder.Entity<Map>(ConfigureMapEntity);
            modelBuilder.Entity<Layer>(ConfigureLayerEntity);
            modelBuilder.Entity<LayerOptions>(ConfigureLayerOptionsEntity);
        }

        private void ConfigureLayerEntity(EntityTypeBuilder<Layer> builder)
        {
            builder.ToTable("layer").HasKey(layer => layer.Id);

            builder.Property(layer => layer.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newid()");

            builder.Property(layer => layer.Index)
                .HasColumnName("index")
                .IsRequired();

            builder.Property(layer => layer.Name)
                .HasColumnName("name")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(layer => layer.Data)
                .HasColumnName("data")
                .IsRequired();

            builder.Property(layer => layer.Visible)
                .HasColumnName("visible")
                .IsRequired();

            builder.Property(layer => layer.MapId)
                .HasColumnName("map_id");

            builder.HasOne(layer => layer.Map)
                .WithMany(map => map.Layers)
                .HasForeignKey(layer => layer.MapId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(layer => layer.Options)
                .WithOne(options => options.Layer)
                .HasForeignKey<LayerOptions>(options => options.LayerId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureLayerOptionsEntity(EntityTypeBuilder<LayerOptions> builder)
        {
            builder.ToTable("layer_options").HasKey(options => options.LayerId);

            builder.Property(layer => layer.LayerId)
                .HasColumnName("layer_id");

            builder.Property(layer => layer.Type)
                .HasColumnName("type")
                .HasDefaultValue(LayerType.Default)
                .IsRequired();
        }

        private void ConfigureMapEntity(EntityTypeBuilder<Map> builder)
        {
            builder.ToTable("map").HasKey(map => map.Id);

            builder.Property(map => map.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newid()");

            builder.Property(map => map.Name)
                .HasColumnName("name")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(map => map.Settings)
                .HasColumnName("settings");

            builder.Property(map => map.Description)
                .HasColumnName("description")
                .HasMaxLength(1024);

            builder.Property(layer => layer.UserId)
                .HasColumnName("user_id");

            builder.HasOne(map => map.User)
                .WithMany(user => user.Maps)
                .HasForeignKey(map => map.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private void ConfigureUserEntity(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user").HasKey(user => user.Id);

            builder.Property(user => user.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newid()");

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

            builder.HasIndex(user => user.Name).IsUnique();
            builder.HasIndex(user => user.Email).IsUnique();
        }
    }
}
