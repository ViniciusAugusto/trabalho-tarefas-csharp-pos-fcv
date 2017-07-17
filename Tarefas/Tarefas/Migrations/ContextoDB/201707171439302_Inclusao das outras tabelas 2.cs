namespace Tarefas.Migrations.ContextoDB
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inclusaodasoutrastabelas2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotasModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Texto = c.String(nullable: false),
                        dataHora = c.DateTime(nullable: false),
                        Usuario = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NotasModel");
        }
    }
}
