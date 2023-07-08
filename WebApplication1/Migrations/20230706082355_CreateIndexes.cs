using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIAdverts.Migrations
{
    /// <inheritdoc />
    public partial class CreateIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Announcements_Id",
                table: "Announcements",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_Number",
                table: "Announcements",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_Rating",
                table: "Announcements",
                column: "Rating");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Announcements_Id",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_Number",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_Rating",
                table: "Announcements");
        }
    }
}
