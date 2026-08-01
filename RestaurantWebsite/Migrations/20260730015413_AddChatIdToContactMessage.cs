using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddChatIdToContactMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "ContactMessages",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "ContactMessages");
        }
    }
}
