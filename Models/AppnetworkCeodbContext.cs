using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppnetworkCEO2023.Models;

public partial class AppnetworkCeodbContext : DbContext
{
    public AppnetworkCeodbContext()
    {
    }

    public AppnetworkCeodbContext(DbContextOptions<AppnetworkCeodbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CeoMentor> CeoMentors { get; set; }

    public virtual DbSet<CeoPayment> CeoPayments { get; set; }

    public virtual DbSet<SharedHolderNok> SharedHolderNoks { get; set; }

    public virtual DbSet<ShareholdersPayment> ShareholdersPayments { get; set; }

    public virtual DbSet<SubscriberAccount> SubscriberAccounts { get; set; }

    public virtual DbSet<SubscriberReferalBonuse> SubscriberReferalBonuses { get; set; }

    public virtual DbSet<SubscriberRegister> SubscriberRegisters { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=.;Database=AppnetworkCEOdb;Trusted_connection=true;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CeoMentor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CeoMentor_Id");

            entity.ToTable("CeoMentor");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("DOB");
            entity.Property(e => e.Othernames)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.CeoMentor)
                .HasForeignKey<CeoMentor>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CeoPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SubcriberPayment_Id");

            entity.ToTable("CeoPayment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Isconfirmed).HasDefaultValueSql("((0))");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentJson)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TransacRefId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.CeoPayment)
                .HasForeignKey<CeoPayment>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SharedHolderNok>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SharedHolderNOK_Id");

            entity.ToTable("SharedHolderNOK");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("DOB");
            entity.Property(e => e.Othernames)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SharedHolderNok)
                .HasForeignKey<SharedHolderNok>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ShareholdersPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ShareholdersPayment_Id");

            entity.ToTable("ShareholdersPayment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Isconfirmed).HasDefaultValueSql("((0))");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentJson)
                .HasMaxLength(2000)
                .IsUnicode(false);
            entity.Property(e => e.TransacRefId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ShareholdersPayment)
                .HasForeignKey<ShareholdersPayment>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SubscriberAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SubscriberAccount_Id");

            entity.ToTable("SubscriberAccount");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccountName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AccoutNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BankName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SubscriberAccount)
                .HasForeignKey<SubscriberAccount>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SubscriberReferalBonuse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ReferalBonuses_id");

            entity.Property(e => e.BonusDate).HasColumnType("datetime");

            entity.HasOne(d => d.Owner).WithMany(p => p.SubscriberReferalBonuses)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubscriberReferalBonuses_SubscriberRegister_Id");
        });

        modelBuilder.Entity<SubscriberRegister>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SubscriberRegister_Id");

            entity.ToTable("SubscriberRegister");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OtherName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PictureName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReferralCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RegisteredDate).HasColumnType("datetime");
            entity.Property(e => e.Religion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
