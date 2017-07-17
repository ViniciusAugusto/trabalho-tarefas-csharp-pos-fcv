namespace Tarefas.Migrations.ApplicationDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IniciodoProjeto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Papeis",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UsuarioPapel",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Papeis", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Usuarios", t => t.IdentityUser_Id)
                .Index(t => t.RoleId)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        UsuarioId = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        FullName = c.String(),
                        NickName = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.UsuarioId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.Claims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuarios", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.Logins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.Usuarios", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsuarioPapel", "IdentityUser_Id", "dbo.Usuarios");
            DropForeignKey("dbo.Logins", "IdentityUser_Id", "dbo.Usuarios");
            DropForeignKey("dbo.Claims", "IdentityUser_Id", "dbo.Usuarios");
            DropForeignKey("dbo.UsuarioPapel", "RoleId", "dbo.Papeis");
            DropIndex("dbo.Logins", new[] { "IdentityUser_Id" });
            DropIndex("dbo.Claims", new[] { "IdentityUser_Id" });
            DropIndex("dbo.Usuarios", "UserNameIndex");
            DropIndex("dbo.UsuarioPapel", new[] { "IdentityUser_Id" });
            DropIndex("dbo.UsuarioPapel", new[] { "RoleId" });
            DropIndex("dbo.Papeis", "RoleNameIndex");
            DropTable("dbo.Logins");
            DropTable("dbo.Claims");
            DropTable("dbo.Usuarios");
            DropTable("dbo.UsuarioPapel");
            DropTable("dbo.Papeis");
        }
    }
}
