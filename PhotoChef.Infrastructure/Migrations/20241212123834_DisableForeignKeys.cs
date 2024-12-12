using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoChef.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DisableForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = 0;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = 1;");
        }
    }
}
