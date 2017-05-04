
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Toucan.Data.Model;

namespace Toucan.Data
{
    public sealed class NpgSqlContext : DbContextBase
    {
        public NpgSqlContext() : base()
        {
        }

        public NpgSqlContext(DbContextOptions<NpgSqlContext> options) : base(options)
        {
        }

        protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Provider>(entity =>
            {
                entity.Property(e => e.ProviderId).HasColumnType("varchar(64)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Enabled)
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(128)");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .HasColumnType("varchar(16)");

                entity.Property(e => e.CreatedOn)
                    .IsRequired()
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("current_timestamp");

                entity.Property(e => e.Enabled)
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(64)");

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(o => o.CreatedBy)
                    .IsRequired();
            });

            modelBuilder.HasSequence<long>("user_seq")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .ForSqlServerHasDefaultValueSql("nextval('\"user_seq\"')");

                entity.Property(e => e.CreatedOn)
                    .IsRequired()
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("current_timestamp");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Enabled)
                    .HasDefaultValueSql("true");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Verified)
                    .HasDefaultValueSql("false");

            });

            modelBuilder.Entity<UserProvider>(entity =>
            {
                entity.HasKey(e => new { e.ProviderId, e.UserId })
                    .HasName("PK_UserProvider");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserProvider_UserId");

                entity.Property(e => e.ProviderId)
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.CreatedOn)
                    .IsRequired()
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("current_timestamp");

                entity.Property(e => e.ExternalId)
                    .HasColumnType("varchar(64)");

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ProviderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserProvider_Provider");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Providers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserProvider_User");

                entity.HasDiscriminator<string>("UserProviderType")
                   .HasValue<UserProvider>("External")
                   .HasValue<UserProviderLocal>("Local");
            });

            modelBuilder.Entity<Verification>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_Verification");

                entity.Property(e => e.Code)
                    .IsRequired(true)
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.IssuedAt)
                    .IsRequired()
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("current_timestamp");

                entity.Property(e => e.RedeemedAt)
                    .IsRequired(false)
                    .HasColumnType("timestamp");

                entity.HasOne(d => d.User)
                    .WithMany(d => d.Verifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Verification_User");
            });

            modelBuilder.Entity<UserProviderLocal>(entity =>
            {

                entity.Property(e => e.PasswordSalt)
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.PasswordHash)
                    .HasColumnType("varchar(256)");

            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.UserId })
                    .HasName("PK_UserRole");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserRole_UserId");

                entity.Property(e => e.RoleId).HasColumnType("varchar(16)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserRole_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserRole_User");
            });
        }
    }
}