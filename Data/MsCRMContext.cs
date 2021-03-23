﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using VityazReports.Models;

#nullable disable

namespace VityazReports.Data
{
    public partial class MsCRMContext : DbContext
    {
        public MsCRMContext()
        {
        }

        public MsCRMContext(DbContextOptions<MsCRMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<NewAlarmExtensionBase> NewAlarmExtensionBase { get; set; }
        public virtual DbSet<NewAndromedaExtensionBase> NewAndromedaExtensionBase { get; set; }
        public virtual DbSet<NewGuardObjectBase> NewGuardObjectBase { get; set; }
        public virtual DbSet<NewGuardObjectExtensionBase> NewGuardObjectExtensionBase { get; set; }
        public virtual DbSet<NewGuardObjectHistory> NewGuardObjectHistory { get; set; }
        public virtual DbSet<NewPlacesGbrbase> NewPlacesGbrbase { get; set; }
        public virtual DbSet<NewPlacesGbrextensionBase> NewPlacesGbrextensionBase { get; set; }
        public virtual DbSet<SystemUserBase> SystemUserBase { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=sql-service;Initial Catalog=vityaz_MSCRM;Persist Security Info=True;User ID=admin;Password=111111");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AI");

            modelBuilder.Entity<NewAlarmExtensionBase>(entity =>
            {
                entity.HasIndex(e => new { e.NewAlarmDt, e.NewAndromedaAlarm }, "AlarmDtAndromedaINDEX")
                    .IsClustered();

                entity.HasIndex(e => new { e.NewName, e.NewOwner, e.NewDeparture, e.NewArrival, e.NewCancel, e.NewAlarmDt, e.NewPs, e.NewAct, e.NewOnc, e.NewPolice, e.NewTpc, e.NewOrder, e.NewGroup }, "ndx_SystemManaged")
                    .HasFillFactor((byte)80);
            });

            modelBuilder.Entity<NewAndromedaExtensionBase>(entity =>
            {
                entity.HasIndex(e => e.NewNumber, "ndx_SystemManaged")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.NewContactAndromeda, "ndx_for_cascaderelationship_new_contact_new_andromeda")
                    .HasFillFactor((byte)80);

                entity.Property(e => e.NewAndromedaId).ValueGeneratedNever();
            });

            modelBuilder.Entity<NewGuardObjectBase>(entity =>
            {
                entity.HasIndex(e => new { e.CreatedBy, e.CreatedOn, e.ModifiedBy, e.ModifiedOn }, "ndx_Auditing")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => new { e.DeletionStateCode, e.Statecode, e.Statuscode }, "ndx_Core")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => new { e.OwningUser, e.OwningBusinessUnit }, "ndx_Security")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.VersionNumber, "ndx_Sync")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.NewGuardObjectId, "ndx_SystemManaged")
                    .HasFillFactor((byte)80);

                entity.Property(e => e.NewGuardObjectId).ValueGeneratedNever();

                entity.Property(e => e.VersionNumber)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.OwningUserNavigation)
                    .WithMany(p => p.NewGuardObjectBase)
                    .HasForeignKey(d => d.OwningUser)
                    .HasConstraintName("user_new_guard_object");
            });

            modelBuilder.Entity<NewGuardObjectExtensionBase>(entity =>
            {
                entity.HasIndex(e => e.NewGuardObjectId, "GuardObjectIndex")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => new { e.NewObjectNumber, e.NewAddress }, "NewNumber")
                    .IsClustered();

                entity.HasIndex(e => e.NewResult, "ndx_SystemManaged")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.NewAccountAgent, "ndx_for_cascaderelationship_new_account_agent_new_guard_object")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.NewAccount, "ndx_for_cascaderelationship_new_account_new_guard_object")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.NewReactionAccount, "ndx_for_cascaderelationship_new_account_new_guard_object_reaction")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.NewTechserviceAccount, "ndx_for_cascaderelationship_new_account_new_guard_object_techservice")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.NewUvoUnit, "ndx_for_cascaderelationship_new_account_new_guard_object_uvo_unit")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.NewContact, "ndx_for_cascaderelationship_new_contact_new_guard_object")
                    .HasFillFactor((byte)80);

                entity.HasOne(d => d.NewCuratorNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.NewCurator)
                    .HasConstraintName("new_systemuser_new_guard_object");

                entity.HasOne(d => d.NewCuratorMountNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.NewCuratorMount)
                    .HasConstraintName("new_new_guard_object_new_guard_object");

                entity.HasOne(d => d.NewCuratorUserMountNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.NewCuratorUserMount)
                    .HasConstraintName("new_systemuser_new_guard_object_mount");

                entity.HasOne(d => d.NewGuardObject)
                    .WithMany()
                    .HasForeignKey(d => d.NewGuardObjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_New_guard_objectExtensionBase_New_guard_objectBase");

                entity.HasOne(d => d.NewInspectorNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.NewInspector)
                    .HasConstraintName("new_systemuser_guard_object");

                entity.HasOne(d => d.NewRetentionNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.NewRetention)
                    .HasConstraintName("new_systemuser_retention_new_guard_object");
            });

            modelBuilder.Entity<NewGuardObjectHistory>(entity =>
            {
                entity.HasKey(e => e.ModifiedOn)
                    .HasName("PK_ModifiedOn");
            });

            modelBuilder.Entity<NewPlacesGbrbase>(entity =>
            {
                entity.HasIndex(e => new { e.CreatedBy, e.CreatedOn, e.ModifiedBy, e.ModifiedOn }, "ndx_Auditing")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => new { e.DeletionStateCode, e.Statecode, e.Statuscode }, "ndx_Core")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => new { e.OwningUser, e.OwningBusinessUnit }, "ndx_Security")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.VersionNumber, "ndx_Sync")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.NewPlacesGbrid, "ndx_SystemManaged")
                    .HasFillFactor((byte)80);

                entity.Property(e => e.NewPlacesGbrid).ValueGeneratedNever();

                entity.Property(e => e.VersionNumber)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.OwningUserNavigation)
                    .WithMany(p => p.NewPlacesGbrbase)
                    .HasForeignKey(d => d.OwningUser)
                    .HasConstraintName("user_new_placesgbr");
            });

            modelBuilder.Entity<NewPlacesGbrextensionBase>(entity =>
            {
                entity.HasIndex(e => e.NewName, "ndx_SystemManaged")
                    .HasFillFactor((byte)80);

                entity.Property(e => e.NewPlacesGbrid).ValueGeneratedNever();

                entity.HasOne(d => d.NewPlacesGbr)
                    .WithOne(p => p.NewPlacesGbrextensionBase)
                    .HasForeignKey<NewPlacesGbrextensionBase>(d => d.NewPlacesGbrid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_New_PlacesGBRExtensionBase_New_PlacesGBRBase");
            });

            modelBuilder.Entity<SystemUserBase>(entity =>
            {
                entity.HasKey(e => e.SystemUserId)
                    .HasName("cndx_PrimaryKey_SystemUser");

                entity.HasIndex(e => new { e.CreatedBy, e.CreatedOn, e.ModifiedBy, e.ModifiedOn }, "ndx_Auditing")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.DeletionStateCode, "ndx_Core")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => new { e.FullName, e.YomiFullName }, "ndx_Cover")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.InternalEmailAddress, "ndx_Email_1")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.PersonalEmailAddress, "ndx_Email_2")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.MobileAlertEmail, "ndx_Email_3")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.BusinessUnitId, "ndx_Security")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.VersionNumber, "ndx_Sync_VersionNumber")
                    .IsUnique()
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => new { e.Title, e.AccessMode, e.IsDisabled, e.DomainName }, "ndx_SystemManaged")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.CalendarId, "ndx_for_cascaderelationship_calendar_system_users")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.SiteId, "ndx_for_cascaderelationship_site_system_users")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.TerritoryId, "ndx_for_cascaderelationship_territory_system_users")
                    .HasFillFactor((byte)80);

                entity.HasIndex(e => e.ParentSystemUserId, "ndx_for_cascaderelationship_user_parent_user")
                    .HasFillFactor((byte)80);

                entity.Property(e => e.SystemUserId).ValueGeneratedNever();

                entity.Property(e => e.IncomingEmailDeliveryMethod).HasDefaultValueSql("((1))");

                entity.Property(e => e.InviteStatusCode).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsActiveDirectoryUser).HasDefaultValueSql("((1))");

                entity.Property(e => e.OutgoingEmailDeliveryMethod).HasDefaultValueSql("((1))");

                entity.Property(e => e.VersionNumber)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.ParentSystemUser)
                    .WithMany(p => p.InverseParentSystemUser)
                    .HasForeignKey(d => d.ParentSystemUserId)
                    .HasConstraintName("user_parent_user");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}