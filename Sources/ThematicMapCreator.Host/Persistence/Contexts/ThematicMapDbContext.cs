using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThematicMapCreator.Domain.Models;

namespace ThematicMapCreator.Host.Persistence.Contexts
{
    public sealed class ThematicMapDbContext : DbContext
    {
        public ThematicMapDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(ConfigureUserEntity);
            modelBuilder.Entity<Map>(ConfigureMapEntity);
            modelBuilder.Entity<Layer>(ConfigureLayerEntity);
        }

        private void ConfigureLayerEntity(EntityTypeBuilder<Layer> builder)
        {
            builder.ToTable("layers").HasKey(layer => layer.Id);

            builder.Property(layer => layer.Id)
                .HasColumnName("id");

            builder.Property(layer => layer.Index)
                .HasColumnName("index")
                .IsRequired();

            builder.Property(layer => layer.Name)
                .HasColumnName("name")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(layer => layer.Description)
                .HasColumnName("description")
                .HasMaxLength(1024)
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

            builder.Property(layer => layer.IsVisible)
                .HasColumnName("is_visible")
                .IsRequired();

            builder.Property(layer => layer.MapId)
                .HasColumnName("map_id")
                .IsRequired();

            builder.HasOne(layer => layer.Map)
                .WithMany(map => map.Layers)
                .HasForeignKey(layer => layer.MapId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private void ConfigureMapEntity(EntityTypeBuilder<Map> builder)
        {
            builder.ToTable("maps").HasKey(map => map.Id);

            builder.Property(map => map.Id)
                .HasColumnName("id");

            builder.Property(map => map.Name)
                .HasColumnName("name")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(map => map.Description)
                .HasColumnName("description")
                .HasMaxLength(1024);

            builder.Property(layer => layer.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.HasOne(map => map.User)
                .WithMany(user => user.Maps)
                .HasForeignKey(map => map.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private void ConfigureUserEntity(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users").HasKey(user => user.Id);

            builder.Property(user => user.Id)
                .HasColumnName("id");

            builder.Property(user => user.Name)
                .HasColumnName("name")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(user => user.Email)
                .HasColumnName("email")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(user => user.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(64)
                .IsRequired();

            builder.HasIndex(user => user.Name).IsUnique();
            builder.HasIndex(user => user.Email).IsUnique();
        }
    }
}
