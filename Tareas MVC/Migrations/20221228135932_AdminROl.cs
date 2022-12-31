using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

//Esta migracion fue hecha utilizando el comando Add Migration AdminRol en el PM Console

namespace TareasMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminROl : Migration
    {
        /// <inheritdoc />
  
        ///Creando rol ADMIN una sola vez.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS(SELECT Id FROM AspNetRoles 
WHERE Id = 'bcfe5f34-6a3e-43ba-8698-7ff98a8f508b')
BEGIN 
	INSERT AspNetRoles (Id, [Name], [NormalizedName]) values
	---El valor del ID es un codigo GUID (Global Unique Identifier) generado en la pagina
	---guidgenerator.com
	('bcfe5f34-6a3e-43ba-8698-7ff98a8f508b', 'admin', 'ADMIN')
END"); }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE AspNetRoles WHERE Id = 'bcfe5f34-6a3e-43ba-8698-7ff98a8f508b");
        }
    }
}
