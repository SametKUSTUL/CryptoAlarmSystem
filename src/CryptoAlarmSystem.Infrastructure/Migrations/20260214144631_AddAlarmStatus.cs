using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoAlarmSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlarmStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Alarms_UserId_IsTriggered",
                table: "Alarms");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Alarms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_UserId_Status_IsTriggered",
                table: "Alarms",
                columns: new[] { "UserId", "Status", "IsTriggered" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Alarms_UserId_Status_IsTriggered",
                table: "Alarms");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Alarms");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_UserId_IsTriggered",
                table: "Alarms",
                columns: new[] { "UserId", "IsTriggered" });
        }
    }
}
