using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NEXT.GEN.Migrations
{
    /// <inheritdoc />
    public partial class createpostinGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_GroupName",
                table: "Posts",
                column: "GroupName");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupName",
                table: "Posts",
                column: "GroupName",
                principalTable: "Groups",
                principalColumn: "GroupName",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupName",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_GroupName",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "Posts");
        }
    }
}
