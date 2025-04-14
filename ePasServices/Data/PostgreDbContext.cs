using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ePasServices.Models;

namespace ePasServices.Data;

public partial class PostgreDbContext : DbContext
{
    public PostgreDbContext()
    {
    }

    public PostgreDbContext(DbContextOptions<PostgreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppRole> AppRoles { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<AppUserRole> AppUserRoles { get; set; }

    public virtual DbSet<AuditFlowMaster> AuditFlowMasters { get; set; }

    public virtual DbSet<AuditTransaction> AuditTransactions { get; set; }

    public virtual DbSet<Spbu> Spbus { get; set; }

    public virtual DbSet<SpbuImage> SpbuImages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres2;Username=postgres;Password=admin");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AppRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("app_role_pkey");

            entity.ToTable("app_role");

            entity.HasIndex(e => new { e.App, e.Name }, "ux_app_role_name").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.App)
                .HasMaxLength(100)
                .HasColumnName("app");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.MenuFunction).HasColumnName("menu_function");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("app_user_pkey");

            entity.ToTable("app_user");

            entity.HasIndex(e => e.Username, "ux_app_user_username").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.LastChangePasswdDt)
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("last_change_passwd_dt");
            entity.Property(e => e.LastLoginDt)
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("last_login_dt");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.NotificationToken)
                .HasMaxLength(255)
                .HasColumnName("notification_token");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(60)
                .HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.SuffixRefreshToken)
                .HasMaxLength(25)
                .HasColumnName("suffix_refresh_token");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<AppUserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("app_user_role_pkey");

            entity.ToTable("app_user_role");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AppRoleId)
                .HasMaxLength(50)
                .HasColumnName("app_role_id");
            entity.Property(e => e.AppUserId)
                .HasMaxLength(50)
                .HasColumnName("app_user_id");
            entity.Property(e => e.SpbuId)
                .HasMaxLength(50)
                .HasColumnName("spbu_id");

            entity.HasOne(d => d.AppRole).WithMany(p => p.AppUserRoles)
                .HasForeignKey(d => d.AppRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("app_user_role_app_role_id_fkey");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AppUserRoles)
                .HasForeignKey(d => d.AppUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("app_user_role_app_user_id_fkey");

            entity.HasOne(d => d.Spbu).WithMany(p => p.AppUserRoles)
                .HasForeignKey(d => d.SpbuId)
                .HasConstraintName("app_user_role_spbu_id_fkey");
        });

        modelBuilder.Entity<AuditFlowMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_flow_master_pkey");

            entity.ToTable("audit_flow_master");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AuditLevel)
                .HasMaxLength(100)
                .HasColumnName("audit_level");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.FailedAuditLevel)
                .HasMaxLength(100)
                .HasColumnName("failed_audit_level");
            entity.Property(e => e.PassedAuditLevel)
                .HasMaxLength(100)
                .HasColumnName("passed_audit_level");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<AuditTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_transaction_pkey");

            entity.ToTable("audit_transaction");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AppUserId)
                .HasMaxLength(50)
                .HasColumnName("app_user_id");
            entity.Property(e => e.AuditLevel)
                .HasMaxLength(100)
                .HasColumnName("audit_level");
            entity.Property(e => e.AuditScheduleDate).HasColumnName("audit_schedule_date");
            entity.Property(e => e.AuditType)
                .HasMaxLength(100)
                .HasColumnName("audit_type");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.SpbuId)
                .HasMaxLength(50)
                .HasColumnName("spbu_id");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AuditTransactions)
                .HasForeignKey(d => d.AppUserId)
                .HasConstraintName("audit_transaction_app_user_id_fkey");

            entity.HasOne(d => d.Spbu).WithMany(p => p.AuditTransactions)
                .HasForeignKey(d => d.SpbuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_transaction_spbu_id_fkey");
        });

        modelBuilder.Entity<Spbu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("spbu_pkey");

            entity.ToTable("spbu");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<SpbuImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("spbu_image_pkey");

            entity.ToTable("spbu_image");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Filepath)
                .HasMaxLength(255)
                .HasColumnName("filepath");
            entity.Property(e => e.SpbuId)
                .HasMaxLength(50)
                .HasColumnName("spbu_id");

            entity.HasOne(d => d.Spbu).WithMany(p => p.SpbuImages)
                .HasForeignKey(d => d.SpbuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("spbu_image_spbu_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
