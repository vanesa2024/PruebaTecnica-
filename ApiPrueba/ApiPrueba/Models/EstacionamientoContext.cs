using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ApiPrueba.Models;

public partial class EstacionamientoContext : DbContext
{
    public EstacionamientoContext()
    {
    }

    public EstacionamientoContext(DbContextOptions<EstacionamientoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Estancia> Estancia { get; set; }

    public virtual DbSet<TipoVehiculo> TipoVehiculos { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estancia>(entity =>
        {
            entity.HasKey(e => e.IdEstancia).HasName("PK__Estancia__BB974FFD68224C71");

            entity.Property(e => e.HoraEntrada).HasColumnType("datetime");
            entity.Property(e => e.HoraSalida).HasColumnType("datetime");
            entity.Property(e => e.IdVehiculo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TotalPago).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Estancia)
                .HasForeignKey(d => d.IdVehiculo)
                .HasConstraintName("FK__Estancia__IdVehi__4E88ABD4");
        });

        modelBuilder.Entity<TipoVehiculo>(entity =>
        {
            entity.HasKey(e => e.IdTipoVehiculo).HasName("PK__TipoVehi__DC20741E29195D42");

            entity.ToTable("TipoVehiculo");

            entity.Property(e => e.Tarifa).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TipoVehiculo1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TipoVehiculo");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.NumeroPlaca).HasName("PK__Vehiculo__5462AC5BF0B9B151");

            entity.ToTable("Vehiculo");

            entity.Property(e => e.NumeroPlaca)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTipoVehiculoNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.IdTipoVehiculo)
                .HasConstraintName("FK__Vehiculo__IdTipo__4BAC3F29");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
