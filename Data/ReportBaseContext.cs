// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using VityazReports.Models.MainWindow;

#nullable disable

namespace VityazReports.Data
{
    public partial class ReportBaseContext : DbContext
    {
        public ReportBaseContext()
        {
        }

        public ReportBaseContext(DbContextOptions<ReportBaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CalculatedRoutes> CalculatedRoutes { get; set; }
        public virtual DbSet<Reports> Reports { get; set; }
        public virtual DbSet<UsersReports> UsersReports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=sql-service;Initial Catalog=ReportBase;Persist Security Info=True;User ID=admin;Password=111111");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<CalculatedRoutes>(entity =>
            {
                entity.Property(e => e.ClcRecordId)
                    .ValueGeneratedNever()
                    .HasComment("Задает уникальный идентификатор записи в базе");

                entity.Property(e => e.ClcCalcName).HasComment("Наименование расчета для отображения пользователю");

                entity.Property(e => e.ClcDuration).HasComment("Время прибытия экипажа до объекта в секундах");

                entity.Property(e => e.ClcGroup).HasComment("Номер маршрута(экипажа)");

                entity.Property(e => e.ClcGroupLatitude).HasComment("Широта (55...)");

                entity.Property(e => e.ClcGroupLongitude).HasComment("Долгота (61...)");

                entity.Property(e => e.ClcKeyCalcId).HasComment("Поле для группировки расчётов(объекты из одного расчета имеют один KeyCalc)");

                entity.Property(e => e.ClcObjectId).HasComment("Хранит идентификатор объекта базы A28");
            });

            modelBuilder.Entity<Reports>(entity =>
            {
                entity.Property(e => e.RptId).ValueGeneratedNever();
            });

            modelBuilder.Entity<UsersReports>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}