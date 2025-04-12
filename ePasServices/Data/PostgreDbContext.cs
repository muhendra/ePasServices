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

    public virtual DbSet<Audit> Audits { get; set; }

    public virtual DbSet<AuditPreparation> AuditPreparations { get; set; }

    public virtual DbSet<AuditQuestionnaire> AuditQuestionnaires { get; set; }

    public virtual DbSet<AuditQuestionnaireMedium> AuditQuestionnaireMedia { get; set; }

    public virtual DbSet<AuditSurvey> AuditSurveys { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Master> Masters { get; set; }

    public virtual DbSet<MasterGroup> MasterGroups { get; set; }

    public virtual DbSet<MasterPreparation> MasterPreparations { get; set; }

    public virtual DbSet<MasterQuestionnaire> MasterQuestionnaires { get; set; }

    public virtual DbSet<MasterQuestionnaireCategory> MasterQuestionnaireCategories { get; set; }

    public virtual DbSet<MasterQuestionnaireMultipleChoice> MasterQuestionnaireMultipleChoices { get; set; }

    public virtual DbSet<MasterSurvey> MasterSurveys { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<SchemaMigration> SchemaMigrations { get; set; }

    public virtual DbSet<Spbu> Spbus { get; set; }

    public virtual DbSet<SpbuEmail> SpbuEmails { get; set; }

    public virtual DbSet<SpbuImage> SpbuImages { get; set; }

    public virtual DbSet<SysParameter> SysParameters { get; set; }

    public virtual DbSet<Upm> Upms { get; set; }

    public virtual DbSet<VwAppUser> VwAppUsers { get; set; }

    public virtual DbSet<VwCity> VwCities { get; set; }

    public virtual DbSet<VwMasterQuestionnaireCategory> VwMasterQuestionnaireCategories { get; set; }

    public virtual DbSet<VwProvince> VwProvinces { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=e-pas_management;Username=postgres;Password=admin");

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

            entity.HasOne(d => d.AppRole).WithMany(p => p.AppUserRoles)
                .HasForeignKey(d => d.AppRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("app_user_role_app_role_id_fkey");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AppUserRoles)
                .HasForeignKey(d => d.AppUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("app_user_role_app_user_id_fkey");
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_pkey");

            entity.ToTable("audit");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AuditorId)
                .HasMaxLength(50)
                .HasColumnName("auditor_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.ExecuteTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("execute_time");
            entity.Property(e => e.MasterQuestionnaireCategoryId)
                .HasMaxLength(50)
                .HasColumnName("master_questionnaire_category_id");
            entity.Property(e => e.MasterQuestionnaireCategoryName)
                .HasMaxLength(100)
                .HasColumnName("master_questionnaire_category_name");
            entity.Property(e => e.PlanDate).HasColumnName("plan_date");
            entity.Property(e => e.ResultFilepath)
                .HasMaxLength(255)
                .HasColumnName("result_filepath");
            entity.Property(e => e.ResultPoint).HasColumnName("result_point");
            entity.Property(e => e.SendTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("send_time");
            entity.Property(e => e.SpbuId)
                .HasMaxLength(50)
                .HasColumnName("spbu_id");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.SurveyTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("survey_time");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Auditor).WithMany(p => p.Audits)
                .HasForeignKey(d => d.AuditorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_auditor_fkey");

            entity.HasOne(d => d.MasterQuestionnaireCategory).WithMany(p => p.Audits)
                .HasForeignKey(d => d.MasterQuestionnaireCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("master_questionnaire_category_fkey");

            entity.HasOne(d => d.Spbu).WithMany(p => p.Audits)
                .HasForeignKey(d => d.SpbuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_spbu_fkey");
        });

        modelBuilder.Entity<AuditPreparation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_preparation_pkey");

            entity.ToTable("audit_preparation");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Answer)
                .HasMaxLength(100)
                .HasColumnName("answer");
            entity.Property(e => e.AuditId)
                .HasMaxLength(50)
                .HasColumnName("audit_id");
            entity.Property(e => e.OrderNo)
                .HasDefaultValue(0)
                .HasColumnName("order_no");
            entity.Property(e => e.Preparation).HasColumnName("preparation");

            entity.HasOne(d => d.Audit).WithMany(p => p.AuditPreparations)
                .HasForeignKey(d => d.AuditId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_preparation_audit_fkey");
        });

        modelBuilder.Entity<AuditQuestionnaire>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_questionnaire_pkey");

            entity.ToTable("audit_questionnaire");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Answer).HasColumnName("answer");
            entity.Property(e => e.AuditId)
                .HasMaxLength(50)
                .HasColumnName("audit_id");
            entity.Property(e => e.OrderNo)
                .HasDefaultValue(0)
                .HasColumnName("order_no");
            entity.Property(e => e.Point)
                .HasDefaultValue(0)
                .HasColumnName("point");
            entity.Property(e => e.Questionnaire).HasColumnName("questionnaire");
            entity.Property(e => e.QuestionnaireType)
                .HasMaxLength(50)
                .HasColumnName("questionnaire_type");

            entity.HasOne(d => d.Audit).WithMany(p => p.AuditQuestionnaires)
                .HasForeignKey(d => d.AuditId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_questionnaire_audit_fkey");
        });

        modelBuilder.Entity<AuditQuestionnaireMedium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_questionnaire_media_pkey");

            entity.ToTable("audit_questionnaire_media");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AuditQuestionnaireId)
                .HasMaxLength(50)
                .HasColumnName("audit_questionnaire_id");
            entity.Property(e => e.MediaFilepath)
                .HasMaxLength(255)
                .HasColumnName("media_filepath");
            entity.Property(e => e.MediaType)
                .HasMaxLength(50)
                .HasColumnName("media_type");

            entity.HasOne(d => d.AuditQuestionnaire).WithMany(p => p.AuditQuestionnaireMedia)
                .HasForeignKey(d => d.AuditQuestionnaireId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_questionnaire_media_audit_questionnaire_fkey");
        });

        modelBuilder.Entity<AuditSurvey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_survey_pkey");

            entity.ToTable("audit_survey");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Answer).HasColumnName("answer");
            entity.Property(e => e.AuditId)
                .HasMaxLength(50)
                .HasColumnName("audit_id");
            entity.Property(e => e.OrderNo)
                .HasDefaultValue(0)
                .HasColumnName("order_no");
            entity.Property(e => e.Survey).HasColumnName("survey");

            entity.HasOne(d => d.Audit).WithMany(p => p.AuditSurveys)
                .HasForeignKey(d => d.AuditId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_survey_audit_fkey");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("city_pkey");

            entity.ToTable("city");

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
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ProvinceId)
                .HasMaxLength(50)
                .HasColumnName("province_id");
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

            entity.HasOne(d => d.Province).WithMany(p => p.Cities)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("city_province_fkey");
        });

        modelBuilder.Entity<Master>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("master_pkey");

            entity.ToTable("master");

            entity.HasIndex(e => new { e.MasterGroupCode, e.Code }, "ux_master_code")
                .IsUnique()
                .HasFilter("((status)::text = 'ACTIVE'::text)");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
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
            entity.Property(e => e.MasterGroupCode)
                .HasMaxLength(50)
                .HasColumnName("master_group_code");
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

            entity.HasOne(d => d.MasterGroupCodeNavigation).WithMany(p => p.Masters)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.MasterGroupCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("master_master_group_code_fkey");
        });

        modelBuilder.Entity<MasterGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("master_group_pkey");

            entity.ToTable("master_group");

            entity.HasIndex(e => e.Code, "ux_master_group_code").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
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
            entity.Property(e => e.Descp).HasColumnName("descp");
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

        modelBuilder.Entity<MasterPreparation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("master_preparation_pkey");

            entity.ToTable("master_preparation");

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
            entity.Property(e => e.OrderNo)
                .HasDefaultValue(0)
                .HasColumnName("order_no");
            entity.Property(e => e.Preparation).HasColumnName("preparation");
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

        modelBuilder.Entity<MasterQuestionnaire>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("master_questionnaire_pkey");

            entity.ToTable("master_questionnaire");

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
            entity.Property(e => e.MasterQuestionnaireCategoryId)
                .HasMaxLength(50)
                .HasColumnName("master_questionnaire_category_id");
            entity.Property(e => e.OrderNo)
                .HasDefaultValue(0)
                .HasColumnName("order_no");
            entity.Property(e => e.Questionnaire).HasColumnName("questionnaire");
            entity.Property(e => e.QuestionnaireType)
                .HasMaxLength(50)
                .HasColumnName("questionnaire_type");
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
            entity.Property(e => e.Weight)
                .HasDefaultValue(1)
                .HasColumnName("weight");

            entity.HasOne(d => d.MasterQuestionnaireCategory).WithMany(p => p.MasterQuestionnaires)
                .HasForeignKey(d => d.MasterQuestionnaireCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("master_questionnaire_master_questionnaire_category_fkey");
        });

        modelBuilder.Entity<MasterQuestionnaireCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("master_questionnaire_category_pkey");

            entity.ToTable("master_questionnaire_category");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.MasterQuestionnaireCategoryId)
                .HasMaxLength(50)
                .HasColumnName("master_questionnaire_category_id");
            entity.Property(e => e.OrderNo)
                .HasDefaultValue(0)
                .HasColumnName("order_no");
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

        modelBuilder.Entity<MasterQuestionnaireMultipleChoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("master_questionnaire_multiple_choice_pkey");

            entity.ToTable("master_questionnaire_multiple_choice");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Answer).HasColumnName("answer");
            entity.Property(e => e.MasterQuestionnaireId)
                .HasMaxLength(50)
                .HasColumnName("master_questionnaire_id");
            entity.Property(e => e.OrderNo)
                .HasDefaultValue(0)
                .HasColumnName("order_no");
            entity.Property(e => e.Weight)
                .HasDefaultValue(1)
                .HasColumnName("weight");

            entity.HasOne(d => d.MasterQuestionnaire).WithMany(p => p.MasterQuestionnaireMultipleChoices)
                .HasForeignKey(d => d.MasterQuestionnaireId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("master_questionnaire_multiple_choice_master_questionnaire_fkey");
        });

        modelBuilder.Entity<MasterSurvey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("master_survey_pkey");

            entity.ToTable("master_survey");

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
            entity.Property(e => e.OrderNo)
                .HasDefaultValue(0)
                .HasColumnName("order_no");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.Survey).HasColumnName("survey");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_pkey");

            entity.ToTable("notification");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AppUserId)
                .HasMaxLength(50)
                .HasColumnName("app_user_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .HasColumnName("data");
            entity.Property(e => e.Message)
                .HasMaxLength(255)
                .HasColumnName("message");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasColumnName("token");
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

            entity.HasOne(d => d.AppUser).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.AppUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notification_app_user_fkey");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("province_pkey");

            entity.ToTable("province");

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
            entity.Property(e => e.UpmsId)
                .HasMaxLength(50)
                .HasColumnName("upms_id");

            entity.HasOne(d => d.Upms).WithMany(p => p.Provinces)
                .HasForeignKey(d => d.UpmsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("province_upms_fkey");
        });

        modelBuilder.Entity<SchemaMigration>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("schema_migrations_pkey");

            entity.ToTable("schema_migrations");

            entity.Property(e => e.Version)
                .ValueGeneratedNever()
                .HasColumnName("version");
            entity.Property(e => e.Dirty).HasColumnName("dirty");
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
            entity.Property(e => e.AreaCode)
                .HasMaxLength(50)
                .HasColumnName("area_code");
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

        modelBuilder.Entity<SpbuEmail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("spbu_email_pkey");

            entity.ToTable("spbu_email");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AppUserId)
                .HasMaxLength(50)
                .HasColumnName("app_user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.SpbuId)
                .HasMaxLength(50)
                .HasColumnName("spbu_id");

            entity.HasOne(d => d.AppUser).WithMany(p => p.SpbuEmails)
                .HasForeignKey(d => d.AppUserId)
                .HasConstraintName("spbu_email_app_user_id_fkey");

            entity.HasOne(d => d.Spbu).WithMany(p => p.SpbuEmails)
                .HasForeignKey(d => d.SpbuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("spbu_email_spbu_fkey");
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

        modelBuilder.Entity<SysParameter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sys_parameter_pkey");

            entity.ToTable("sys_parameter");

            entity.HasIndex(e => e.Code, "ux_sys_parameter_code").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
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
            entity.Property(e => e.Descp).HasColumnName("descp");
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

        modelBuilder.Entity<Upm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("upms_pkey");

            entity.ToTable("upms");

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

        modelBuilder.Entity<VwAppUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_app_user");

            entity.Property(e => e.App)
                .HasMaxLength(100)
                .HasColumnName("app");
            entity.Property(e => e.AppRole)
                .HasMaxLength(100)
                .HasColumnName("app_role");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasColumnType("character varying")
                .HasColumnName("phone_number");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<VwCity>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_city");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ProvinceName)
                .HasMaxLength(100)
                .HasColumnName("province_name");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<VwMasterQuestionnaireCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_master_questionnaire_category");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.OrderNo).HasColumnName("order_no");
            entity.Property(e => e.ParentName)
                .HasMaxLength(100)
                .HasColumnName("parent_name");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<VwProvince>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_province");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_date");
            entity.Property(e => e.UpmsName)
                .HasMaxLength(100)
                .HasColumnName("upms_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
