using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Tarefas.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string FullName { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string NickName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Observe que o authenticationType deve corresponder àquele definido em CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Adicionar declarações de usuário personalizado aqui

            userIdentity.AddClaim(new Claim("FullName", this.FullName));
            userIdentity.AddClaim(new Claim("NickName", this.NickName));
            userIdentity.AddClaim(new Claim(ClaimTypes.Email, this.Email));

            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>()
                .ToTable("Usuarios")
                .Property(p => p.Id)
                .HasColumnName("UsuarioId");

            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Usuarios")
                .Property(p => p.Id)
                .HasColumnName("UsuarioId");

            modelBuilder.Entity<IdentityUserRole>()
                .ToTable("UsuarioPapel");

            modelBuilder.Entity<IdentityUserLogin>()
                .ToTable("Logins");

            modelBuilder.Entity<IdentityUserClaim>()
                .ToTable("Claims");

            modelBuilder.Entity<IdentityRole>()
                .ToTable("Papeis");
        }
    }
}