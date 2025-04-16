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

    public virtual DbSet<MasterAuditFlow> MasterAuditFlows { get; set; }

    public virtual DbSet<MasterQuestioner> MasterQuestioners { get; set; }

    public virtual DbSet<MasterQuestionerDetail> MasterQuestionerDetails { get; set; }

    public virtual DbSet<Spbu> Spbus { get; set; }

    public virtual DbSet<SpbuImage> SpbuImages { get; set; }

    public virtual DbSet<TrxAudit> TrxAudits { get; set; }

    public virtual DbSet<TrxAuditChecklist> TrxAuditChecklists { get; set; }

    public virtual DbSet<TrxAuditMedium> TrxAuditMedia { get; set; }

    public virtual DbSet<TrxAuditQq> TrxAuditQqs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=34.101.139.4;Port=5432;Database=epas;Username=epas_pertamina;Password=Ep4sU$2025ApR@_Ps$w0rd_P3rtM1N4@0k2025Apr");

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

        modelBuilder.Entity<MasterAuditFlow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("master_audit_flow_pkey");

            entity.ToTable("master_audit_flow");

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

        modelBuilder.Entity<MasterQuestioner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("master_questioner_pkey");

            entity.ToTable("master_questioner");

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
            entity.Property(e => e.EffectiveEndDate).HasColumnName("effective_end_date");
            entity.Property(e => e.EffectiveStartDate).HasColumnName("effective_start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
            entity.Property(e => e.Version)
                .HasDefaultValue(0)
                .HasColumnName("version");
        });

        modelBuilder.Entity<MasterQuestionerDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("master_questioner_detail_pkey");

            entity.ToTable("master_questioner_detail");

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
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MasterQuestionerId)
                .HasMaxLength(50)
                .HasColumnName("master_questioner_id");
            entity.Property(e => e.OrderNo).HasColumnName("order_no");
            entity.Property(e => e.ParentId)
                .HasMaxLength(50)
                .HasColumnName("parent_id");
            entity.Property(e => e.ScoreOption).HasColumnName("score_option");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
            entity.Property(e => e.Weight)
                .HasPrecision(5, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.MasterQuestioner).WithMany(p => p.MasterQuestionerDetails)
                .HasForeignKey(d => d.MasterQuestionerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("master_questioner_detail_master_questioner_id_fkey");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("master_questioner_detail_parent_id_fkey");
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
            entity.Property(e => e.CityName)
                .HasMaxLength(255)
                .HasColumnName("city_name");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Level)
                .HasMaxLength(255)
                .HasColumnName("level");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.ManagerName)
                .HasMaxLength(255)
                .HasColumnName("manager_name");
            entity.Property(e => e.Mor)
                .HasMaxLength(100)
                .HasColumnName("mor");
            entity.Property(e => e.OwnerName)
                .HasMaxLength(255)
                .HasColumnName("owner_name");
            entity.Property(e => e.OwnerType)
                .HasMaxLength(100)
                .HasColumnName("owner_type");
            entity.Property(e => e.PhoneNumber1)
                .HasMaxLength(255)
                .HasColumnName("phone_number_1");
            entity.Property(e => e.PhoneNumber2)
                .HasMaxLength(255)
                .HasColumnName("phone_number_2");
            entity.Property(e => e.ProvinceName)
                .HasMaxLength(255)
                .HasColumnName("province_name");
            entity.Property(e => e.Quater).HasColumnName("quater");
            entity.Property(e => e.SalesArea)
                .HasMaxLength(255)
                .HasColumnName("sales_area");
            entity.Property(e => e.Sbm)
                .HasMaxLength(255)
                .HasColumnName("sbm");
            entity.Property(e => e.SpbuNo)
                .HasMaxLength(100)
                .HasColumnName("spbu_no");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
            entity.Property(e => e.Year).HasColumnName("year");
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

        modelBuilder.Entity<TrxAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trx_audit_pkey");

            entity.ToTable("trx_audit");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AppUserId)
                .HasMaxLength(50)
                .HasColumnName("app_user_id");
            entity.Property(e => e.AuditExecutionDate).HasColumnName("audit_execution_date");
            entity.Property(e => e.AuditLevel)
                .HasMaxLength(100)
                .HasColumnName("audit_level");
            entity.Property(e => e.AuditMediaTotal).HasColumnName("audit_media_total");
            entity.Property(e => e.AuditMediaUpload).HasColumnName("audit_media_upload");
            entity.Property(e => e.AuditMom).HasColumnName("audit_mom");
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
            entity.Property(e => e.MasterQuestionerId)
                .HasMaxLength(50)
                .HasColumnName("master_questioner_id");
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

            entity.HasOne(d => d.AppUser).WithMany(p => p.TrxAudits)
                .HasForeignKey(d => d.AppUserId)
                .HasConstraintName("trx_audit_app_user_id_fkey");

            entity.HasOne(d => d.MasterQuestioner).WithMany(p => p.TrxAudits)
                .HasForeignKey(d => d.MasterQuestionerId)
                .HasConstraintName("trx_audit_master_questioner_id_fkey");

            entity.HasOne(d => d.Spbu).WithMany(p => p.TrxAudits)
                .HasForeignKey(d => d.SpbuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("trx_audit_spbu_id_fkey");
        });

        modelBuilder.Entity<TrxAuditChecklist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trx_audit_checklist_pkey");

            entity.ToTable("trx_audit_checklist");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.MasterQuestionerDetailId)
                .HasMaxLength(50)
                .HasColumnName("master_questioner_detail_id");
            entity.Property(e => e.Point)
                .HasPrecision(5, 2)
                .HasColumnName("point");
            entity.Property(e => e.ScoreInput)
                .HasMaxLength(50)
                .HasColumnName("score_input");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.TrxAuditId)
                .HasMaxLength(50)
                .HasColumnName("trx_audit_id");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.MasterQuestionerDetail).WithMany(p => p.TrxAuditChecklists)
                .HasForeignKey(d => d.MasterQuestionerDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("trx_audit_checklist_master_questioner_detail_id_fkey");

            entity.HasOne(d => d.TrxAudit).WithMany(p => p.TrxAuditChecklists)
                .HasForeignKey(d => d.TrxAuditId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("trx_audit_checklist_trx_audit_id_fkey");
        });

        modelBuilder.Entity<TrxAuditMedium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trx_audit_media_pkey");

            entity.ToTable("trx_audit_media");

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
            entity.Property(e => e.MasterQuestionerDetailId)
                .HasMaxLength(50)
                .HasColumnName("master_questioner_detail_id");
            entity.Property(e => e.MediaPath).HasColumnName("media_path");
            entity.Property(e => e.MediaType)
                .HasMaxLength(100)
                .HasColumnName("media_type");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.TrxAuditId)
                .HasMaxLength(50)
                .HasColumnName("trx_audit_id");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.MasterQuestionerDetail).WithMany(p => p.TrxAuditMedia)
                .HasForeignKey(d => d.MasterQuestionerDetailId)
                .HasConstraintName("trx_audit_media_master_questioner_detail_id_fkey");

            entity.HasOne(d => d.TrxAudit).WithMany(p => p.TrxAuditMedia)
                .HasForeignKey(d => d.TrxAuditId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("trx_audit_media_trx_audit_id_fkey");
        });

        modelBuilder.Entity<TrxAuditQq>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("trx_audit_qq_pkey");

            entity.ToTable("trx_audit_qq");

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
            entity.Property(e => e.DensityVariation).HasColumnName("density_variation");
            entity.Property(e => e.DuMake)
                .HasMaxLength(255)
                .HasColumnName("du_make");
            entity.Property(e => e.DuSerialNo)
                .HasMaxLength(255)
                .HasColumnName("du_serial_no");
            entity.Property(e => e.Mode)
                .HasMaxLength(100)
                .HasColumnName("mode");
            entity.Property(e => e.NozzleNumber)
                .HasMaxLength(50)
                .HasColumnName("nozzle_number");
            entity.Property(e => e.ObservedDensity).HasColumnName("observed_density");
            entity.Property(e => e.ObservedDensity15Degree).HasColumnName("observed_density_15_degree");
            entity.Property(e => e.ObservedTemp).HasColumnName("observed_temp");
            entity.Property(e => e.Product)
                .HasMaxLength(255)
                .HasColumnName("product");
            entity.Property(e => e.QuantityVariationInPercentage).HasColumnName("quantity_variation_in_percentage");
            entity.Property(e => e.QuantityVariationWithMeasure).HasColumnName("quantity_variation_with_measure");
            entity.Property(e => e.ReferenceDensity15Degree).HasColumnName("reference_density_15_degree");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.TankNumber).HasColumnName("tank_number");
            entity.Property(e => e.TrxAuditId)
                .HasMaxLength(50)
                .HasColumnName("trx_audit_id");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.TrxAudit).WithMany(p => p.TrxAuditQqs)
                .HasForeignKey(d => d.TrxAuditId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("trx_audit_qq_trx_audit_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
