namespace Tarefas.Migrations.ContextoDB
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inclusãodasoutrastabelas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TarefaModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Titulo = c.String(nullable: false),
                        Descricao = c.String(),
                        Criador = c.String(nullable: false),
                        Responsavel = c.String(),
                        DataCriacao = c.DateTime(nullable: false),
                        DataConclusao = c.DateTime(),
                        DataAgendamento = c.DateTime(),
                        TimeModelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TimeModel", t => t.TimeModelId, cascadeDelete: true)
                .Index(t => t.TimeModelId);
            
            CreateTable(
                "dbo.TimeModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Dono = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UsuariosTarefasModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Apelido = c.String(nullable: false),
                        NomeCompleto = c.String(nullable: false),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UsuariosTimesModel",
                c => new
                    {
                        UsuariosTarefasModelId = c.Int(nullable: false),
                        TimeModelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UsuariosTarefasModelId, t.TimeModelId })
                .ForeignKey("dbo.UsuariosTarefasModel", t => t.UsuariosTarefasModelId, cascadeDelete: true)
                .ForeignKey("dbo.TimeModel", t => t.TimeModelId, cascadeDelete: true)
                .Index(t => t.UsuariosTarefasModelId)
                .Index(t => t.TimeModelId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TarefaModel", "TimeModelId", "dbo.TimeModel");
            DropForeignKey("dbo.UsuariosTimesModel", "TimeModelId", "dbo.TimeModel");
            DropForeignKey("dbo.UsuariosTimesModel", "UsuariosTarefasModelId", "dbo.UsuariosTarefasModel");
            DropIndex("dbo.UsuariosTimesModel", new[] { "TimeModelId" });
            DropIndex("dbo.UsuariosTimesModel", new[] { "UsuariosTarefasModelId" });
            DropIndex("dbo.TarefaModel", new[] { "TimeModelId" });
            DropTable("dbo.UsuariosTimesModel");
            DropTable("dbo.UsuariosTarefasModel");
            DropTable("dbo.TimeModel");
            DropTable("dbo.TarefaModel");
        }
    }
}
