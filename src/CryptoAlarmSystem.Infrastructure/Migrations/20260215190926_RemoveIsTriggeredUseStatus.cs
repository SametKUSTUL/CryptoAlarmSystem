using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoAlarmSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsTriggeredUseStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Alarms_UserId_Status_IsTriggered",
                table: "Alarms");

            migrationBuilder.DropColumn(
                name: "IsTriggered",
                table: "Alarms");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_UserId_Status",
                table: "Alarms",
                columns: new[] { "UserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Alarms_UserId_Status",
                table: "Alarms");

            migrationBuilder.AddColumn<bool>(
                name: "IsTriggered",
                table: "Alarms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_UserId_Status_IsTriggered",
                table: "Alarms",
                columns: new[] { "UserId", "Status", "IsTriggered" });
        }
    }
}
