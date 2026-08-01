using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "ContactMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "ContactMessages");
        }
    }
}
