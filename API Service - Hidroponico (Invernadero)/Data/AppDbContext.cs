using Microsoft.EntityFrameworkCore;
using API_Service___Hidroponico__Invernadero_.Models;

namespace API_Service___Hidroponico__Invernadero_.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();

        // Catálogos
        public DbSet<Plaga> Plagas => Set<Plaga>();
        public DbSet<Enfermedad> Enfermedades => Set<Enfermedad>();

        // Diagnósticos
        public DbSet<Diagnostico> Diagnosticos => Set<Diagnostico>();


        //public DbSet<Led> Leds => Set<Led>();


        // ===== Hidroponía | Cosechas =====
        public DbSet<Hidroponico> Hidroponicos => Set<Hidroponico>();
        public DbSet<EtapaHidroponico> EtapasHidroponico => Set<EtapaHidroponico>();
        public DbSet<CosechaHidroponico> CosechasHidroponico => Set<CosechaHidroponico>();
        public DbSet<CosechaEtapa> CosechaEtapas => Set<CosechaEtapa>();
        public DbSet<RiegoHistorico> RiegosHistorico => Set<RiegoHistorico>();
        public DbSet<Bandeja> Bandejas => Set<Bandeja>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Usuarios =====
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios", schema: "dbo");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Username).HasMaxLength(150).IsRequired();
                entity.Property(x => x.Correo).HasMaxLength(150).IsRequired();
                entity.Property(x => x.Contrasena).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Foto).HasColumnType("NVARCHAR(MAX)");
                entity.Property(x => x.Activo).IsRequired();
                entity.Property(x => x.CreadoEn).IsRequired();

                entity.HasIndex(x => x.Username).IsUnique();
                entity.HasIndex(x => x.Correo).IsUnique();

                entity.HasOne(x => x.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(x => x.RolId)
                      .IsRequired();
            });

            // ===== Roles =====
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles", schema: "dbo");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Nombre).HasMaxLength(50).IsRequired();
                entity.HasIndex(x => x.Nombre).IsUnique();
            });

            // ===== Plagas =====
            modelBuilder.Entity<Plaga>(e =>
            {
                e.ToTable("Plagas", "dbo");
                e.HasKey(x => x.Id);

                e.Property(x => x.NombreComun).HasMaxLength(150).IsRequired();
                e.HasIndex(x => x.NombreComun).IsUnique().HasDatabaseName("UQ_Plagas_NombreComun");

                e.Property(x => x.NombreCientifico).HasMaxLength(150);
                e.Property(x => x.CicloVida).HasMaxLength(100);
                e.Property(x => x.FotoUrl).HasMaxLength(500);
                e.Property(x => x.PartesAfectadas).HasMaxLength(200);
                e.Property(x => x.Temporada).HasMaxLength(100);

                e.Property(x => x.NivelRiesgo).HasColumnType("tinyint");
                e.HasCheckConstraint("CK_Plagas_NivelRiesgo", "[NivelRiesgo] BETWEEN 0 AND 5");

                e.Property(x => x.CreadoEn).HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("SYSUTCDATETIME()");
            });

            // ===== Enfermedades =====
            modelBuilder.Entity<Enfermedad>(e =>
            {
                e.ToTable("Enfermedades", "dbo");
                e.HasKey(x => x.Id);

                e.Property(x => x.NombreComun).HasMaxLength(150).IsRequired();
                e.HasIndex(x => x.NombreComun).IsUnique().HasDatabaseName("UQ_Enfermedades_NombreComun");

                e.Property(x => x.AgenteCausal).HasMaxLength(150);
                e.Property(x => x.TipoPatogeno).HasMaxLength(20);
                e.Property(x => x.FotoUrl).HasMaxLength(500);
                e.Property(x => x.PartesAfectadas).HasMaxLength(200);
                e.Property(x => x.Temporada).HasMaxLength(100);

                e.Property(x => x.NivelRiesgo).HasColumnType("tinyint");
                e.HasCheckConstraint("CK_Enfer_NivelRiesgo", "[NivelRiesgo] BETWEEN 0 AND 5");

                e.Property(x => x.CreadoEn).HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("SYSUTCDATETIME()");
            });

            // ===== Diagnósticos =====
            modelBuilder.Entity<Diagnostico>(entity =>
            {
                entity.ToTable("Diagnosticos", schema: "dbo");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Codigo)
                      .HasDefaultValueSql("NEWSEQUENTIALID()")
                      .ValueGeneratedOnAdd();

                entity.Property(x => x.FotoDiagnostico).HasColumnType("NVARCHAR(MAX)");
                entity.Property(x => x.Notas).HasColumnType("NVARCHAR(MAX)");

                entity.Property(x => x.FechaMuestreo)
                      .HasColumnType("datetime2(0)")
                      .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.Property(x => x.EtapaFenologica).HasMaxLength(50);

                entity.Property(x => x.Tipo)
                      .HasConversion<string>()
                      .HasMaxLength(12)
                      .IsRequired();

                entity.Property(x => x.Severidad).HasColumnType("tinyint");
                entity.HasCheckConstraint("CK_Diag_Severidad", "[Severidad] IS NULL OR ([Severidad] BETWEEN 0 AND 5)");

                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(x => x.InspectorId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_Diag_Inspector");

                entity.HasIndex(x => x.Codigo).IsUnique();
                entity.HasIndex(x => x.InspectorId);
                entity.HasIndex(x => x.FechaMuestreo);
                entity.HasIndex(x => new { x.Tipo, x.PlagaId, x.EnfermedadId });
            });


            // ===== Hidroponicos =====
            modelBuilder.Entity<Hidroponico>(e =>
            {
                e.ToTable("Hidroponicos", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
                e.Property(x => x.NumeroHidroponico).IsRequired();
                e.Property(x => x.Observaciones).HasColumnType("NVARCHAR(MAX)");
                e.Property(x => x.CantidadBandejas);
                e.Property(x => x.CreadoEn).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");

                e.HasIndex(x => x.NumeroHidroponico).HasDatabaseName("IX_Hidroponicos_Numero");
                e.HasMany(x => x.Cosechas).WithOne(c => c.Hidroponico).HasForeignKey(c => c.HidroponicoId).OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.Bandejas).WithOne(b => b.Hidroponico).HasForeignKey(b => b.HidroponicoId).OnDelete(DeleteBehavior.Cascade);
            });

            // ===== EtapasHidroponico =====
            modelBuilder.Entity<EtapaHidroponico>(e =>
            {
                e.ToTable("EtapasHidroponico", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
                e.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
                e.Property(x => x.OrdenEtapa).IsRequired();
                e.Property(x => x.DuracionHoras).IsRequired();
                e.Property(x => x.RequiereLavado).IsRequired();
                e.Property(x => x.Observaciones).HasColumnType("NVARCHAR(MAX)");

                e.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("UX_EtapasHidroponico_Codigo");
            });

            // ===== CosechasHidroponico =====
            modelBuilder.Entity<CosechaHidroponico>(e =>
            {
                e.ToTable("CosechasHidroponico", "dbo");
                e.HasKey(x => x.Id);

                e.Property(x => x.HidroponicoId).IsRequired();
                e.Property(x => x.NombreZafra).HasMaxLength(200);
                e.Property(x => x.FechaInicio).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.FechaEstimulada).HasColumnType("datetime2(0)");
                e.Property(x => x.FechaFin).HasColumnType("datetime2(0)");
                e.Property(x => x.Observaciones).HasColumnType("NVARCHAR(MAX)");
                e.Property(x => x.Estado).HasMaxLength(50).IsRequired().HasDefaultValue("ACTIVA");
                e.Property(x => x.CreadoEn).HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()");

                e.HasIndex(x => x.HidroponicoId).HasDatabaseName("IX_Cosechas_HidroponicoId");
                e.HasMany(x => x.Etapas).WithOne(ce => ce.Cosecha).HasForeignKey(ce => ce.CosechaId).OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.Riegos).WithOne(r => r.Cosecha).HasForeignKey(r => r.CosechaId).OnDelete(DeleteBehavior.Cascade);
            });

            // ===== CosechaEtapas =====
            modelBuilder.Entity<CosechaEtapa>(e =>
            {
                e.ToTable("CosechaEtapas", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.CosechaId).IsRequired();
                e.Property(x => x.EtapaId).IsRequired();
                e.Property(x => x.FechaInicioReal).HasColumnType("datetime2(0)").IsRequired();
                e.Property(x => x.FechaFinReal).HasColumnType("datetime2(0)").IsRequired(false);
                e.Property(x => x.DuracionHorasPlan).IsRequired();
                e.Property(x => x.RiegoProgramadoIntervaloSegundos);
                e.Property(x => x.Notas).HasColumnType("NVARCHAR(MAX)");

                e.HasIndex(x => new { x.CosechaId, x.EtapaId }).IsUnique().HasDatabaseName("UX_CosechaEtapa_Cosecha_Etapa");

                // Relación a plantilla de etapa (si quieres navegación inversa)
                e.HasOne(x => x.Etapa).WithMany(et => et.CosechaEtapas).HasForeignKey(x => x.EtapaId).OnDelete(DeleteBehavior.Restrict);
            });

            // ===== RiegosHistorico =====
            modelBuilder.Entity<RiegoHistorico>(e =>
            {
                e.ToTable("RiegosHistorico", "dbo");
                e.HasKey(x => x.Id);

                e.Property(x => x.CosechaId).IsRequired();
                e.Property(x => x.EtapaId).IsRequired(false);
                e.Property(x => x.HidroponicoId).IsRequired(false);
                e.Property(x => x.InicioRiego).HasColumnType("datetime2(0)").IsRequired();
                e.Property(x => x.DuracionSeg).IsRequired(false);
                e.Property(x => x.Fuente).HasMaxLength(100);
                e.Property(x => x.Observaciones).HasColumnType("NVARCHAR(MAX)");

                e.HasIndex(x => x.CosechaId).HasDatabaseName("IX_Riegos_CosechaId");
                e.HasIndex(x => x.InicioRiego).HasDatabaseName("IX_Riegos_InicioRiego");
            });

            // ===== Bandejas =====
            modelBuilder.Entity<Bandeja>(e =>
            {
                e.ToTable("Bandejas", "dbo");
                e.HasKey(x => x.Id);
                e.Property(x => x.HidroponicoId).IsRequired();
                e.Property(x => x.Numero).IsRequired();
                e.Property(x => x.CantidadHoyos);
                e.Property(x => x.Observaciones).HasColumnType("NVARCHAR(MAX)");

                e.HasIndex(x => new { x.HidroponicoId, x.Numero }).IsUnique().HasDatabaseName("UX_Bandejas_Hidroponico_Numero");
            });

            //modelBuilder.Entity<Led>().HasData(new Led { Id = 1, Nombre = "Luces", On = false });
        }
    }
}
