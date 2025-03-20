using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NEXT.GEN.Migrations
{
    /// <inheritdoc />
    public partial class newgroupcols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupImage",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupImage",
                table: "Groups");
        }
    }
}
