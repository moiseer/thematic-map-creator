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

            builder.Property(layer => layer.Style)
                .HasColumnName("style")
                .HasDefaultValue(LayerStyle.None)
                .IsRequired();

            builder.Property(layer => layer.StyleOptions)
                .HasColumnName("style_options");

            builder.Property(layer => layer.Type)
                .HasColumnName("type")
                .HasDefaultValue(LayerType.None)
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
