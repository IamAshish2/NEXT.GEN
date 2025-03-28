using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NEXT.GEN.Migrations
{
    /// <inheritdoc />
    public partial class update_comment_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommentText",
                table: "Comments",
                newName: "Content");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Comments",
                newName: "CommentText");
        }
    }
}
