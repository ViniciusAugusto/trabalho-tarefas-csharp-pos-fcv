using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Tarefas.Models
{
    public class ContextoDB : DbContext
    {
        public ContextoDB() : base("DefaultConnection")
        {

        }

        public DbSet<UsuariosTarefasModel> Usuarios { get; set; }

        public DbSet<TimeModel> Times { get; set; }

        public DbSet<TarefaModel> Tarefas { get; set; }
        public DbSet<NotasModel> Notas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<UsuariosTarefasModel>()
                        .HasMany(h => h.Times)
                        .WithMany(w => w.Usuarios)
                        .Map(m =>
                        {
                            m.MapLeftKey("UsuariosTarefasModelId");
                            m.MapRightKey("TimeModelId");
                            m.ToTable("UsuariosTimesModel");
                        });

            base.OnModelCreating(modelBuilder);
        }
    }
}