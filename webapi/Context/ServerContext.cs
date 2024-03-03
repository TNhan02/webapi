using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using webapi.Models;

namespace webapi.Context
{
    public partial class ServerContext : DbContext
    {
        public ServerContext()
        {
        }

        public ServerContext(DbContextOptions<ServerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:webapiContext");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.HasIndex(e => e.Name, "UQ__product__72E12F1B6063C4AB")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdditionalInfo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("additional_info");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Quantity).HasColumnName("quantity");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Username, "UQ__user__F3DBC57209826C0C")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdditionalInfo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("additional_info");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.PasswordHash)
                    .IsUnicode(false)
                    .HasColumnName("password_hash");

                entity.Property(e => e.UserRole)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("user_role");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
