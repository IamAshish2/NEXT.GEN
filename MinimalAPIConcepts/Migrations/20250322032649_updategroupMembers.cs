using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NEXT.GEN.Migrations
{
    /// <inheritdoc />
    public partial class updategroupMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasJoined",
                table: "GroupMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasJoined",
                table: "GroupMembers");
        }
    }
}
