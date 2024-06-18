using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ATBapi.Models.Entities;

public partial class atbContext : DbContext
{
    public atbContext()
    {
    }

    public atbContext(DbContextOptions<atbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Caja> Caja { get; set; }

    public virtual DbSet<Colaespera> Colaespera { get; set; }

    public virtual DbSet<Configuracion> Configuracion { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<Turno> Turno { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Caja>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("caja");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Nombre).HasMaxLength(255);
        });

        modelBuilder.Entity<Colaespera>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("colaespera");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DateTurnoCreado).HasColumnType("datetime");
            entity.Property(e => e.NumeroTurno).HasMaxLength(8);
        });

        modelBuilder.Entity<Configuracion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("configuracion");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Estado)
                .HasMaxLength(45)
                .HasDefaultValueSql("'Cerrado'");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Nombre).HasMaxLength(255);
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("turno");

            entity.HasIndex(e => e.IdUsuario, "IdUsuario");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Estado)
                .HasMaxLength(45)
                .HasDefaultValueSql("'Atendiendo'");
            entity.Property(e => e.HoraFinal).HasColumnType("datetime");
            entity.Property(e => e.HoraInicial).HasColumnType("datetime");
            entity.Property(e => e.IdUsuario).HasColumnType("int(11)");
            entity.Property(e => e.NumeroTurno).HasMaxLength(8);
            entity.Property(e => e.TiempoInicio).HasColumnType("time");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Turno)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("turno_ibfk_1");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.IdRole, "IdRole");

            entity.HasIndex(e => e.IdCaja, "users_caja_idx");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Contraseña).HasMaxLength(255);
            entity.Property(e => e.Correo).HasMaxLength(255);
            entity.Property(e => e.Eliminado).HasColumnType("int(11)");
            entity.Property(e => e.Estado)
                .HasMaxLength(45)
                .HasDefaultValueSql("'Desconectado'");
            entity.Property(e => e.IdCaja).HasColumnType("int(11)");
            entity.Property(e => e.IdRole).HasColumnType("int(11)");
            entity.Property(e => e.Nombre).HasMaxLength(255);

            entity.HasOne(d => d.IdCajaNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdCaja)
                .HasConstraintName("users_caja");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
