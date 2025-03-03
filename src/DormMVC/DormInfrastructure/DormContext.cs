using System;
using System.Collections.Generic;
using DormDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace DormInfrastructure;

public partial class DormContext : DbContext
{
    public DormContext()
    {
    }

    public DormContext(DbContextOptions<DormContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<GuestVisit> GuestVisits { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentChange> StudentChanges { get; set; }

    public virtual DbSet<StudentPayment> StudentPayments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-IG300NS\\SQLEXPRESS; Database=dorm; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.ToTable("Faculty");

            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Dean).HasMaxLength(50);
            entity.Property(e => e.FacultyName).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<GuestVisit>(entity =>
        {
            entity.HasKey(e => e.VisitId);

            entity.ToTable("GuestVisit");

            //entity.Property(e => e.VisitId)
            //    .ValueGeneratedNever()
            //    .HasColumnName("VisitID");
            entity.Property(e => e.VisitId).ValueGeneratedOnAdd();
            entity.Property(e => e.GuestName).HasMaxLength(50);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.GuestVisits)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GuestVisit_Student");
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.ToTable("PaymentType");

            entity.Property(e => e.PaymentTypeId).HasColumnName("PaymentTypeID");
            entity.Property(e => e.PaymentName).HasMaxLength(50);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("Room");

            entity.Property(e => e.RoomId)
                .ValueGeneratedNever()
                .HasColumnName("RoomID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Student");

            //entity.Property(e => e.StudentId)
            //    .ValueGeneratedNever()
            //    .HasColumnName("StudentID");
            entity.Property(e => e.StudentId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Students)
                .HasForeignKey(d => d.FacultyId)
                .HasConstraintName("FK_Student_Faculty");

            entity.HasOne(d => d.Room).WithMany(p => p.Students)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Student_Room");
        });

        modelBuilder.Entity<StudentChange>(entity =>
        {
            entity.HasKey(e => e.ChangeId);
            entity.Property(e => e.ChangeId).ValueGeneratedOnAdd(); //manually added

            //entity.Property(e => e.ChangeId)
            //    .ValueGeneratedNever()
            //    .HasColumnName("ChangeID");
            entity.Property(e => e.ChangeId).ValueGeneratedOnAdd();
            entity.Property(e => e.ChangeField).HasMaxLength(50);
            entity.Property(e => e.NewValue).HasMaxLength(50);
            entity.Property(e => e.OldValue).HasMaxLength(50);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentChanges)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentChanges_Student");
        });

        modelBuilder.Entity<StudentPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);

            entity.ToTable("StudentPayment");

            //entity.Property(e => e.PaymentId)
            //    .ValueGeneratedNever()
            //    .HasColumnName("PaymentID");
            entity.Property(e => e.PaymentId).ValueGeneratedOnAdd();
            entity.Property(e => e.PaymentTypeId).HasColumnName("PaymentTypeID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.StudentPayments)
                .HasForeignKey(d => d.PaymentTypeId)
                .HasConstraintName("FK_StudentPayment_PaymentType");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentPayments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentPayment_Student");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
