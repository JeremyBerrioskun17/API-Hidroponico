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


            //modelBuilder.Entity<Led>().HasData(new Led { Id = 1, Nombre = "Luces", On = false });
        }
    }
}
