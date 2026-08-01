using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddTableReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TableReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Time = table.Column<string>(type: "TEXT", nullable: false),
                    GuestCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreateedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableReservations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TableReservations");
        }
    }
}
