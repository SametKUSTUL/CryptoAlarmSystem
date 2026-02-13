using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CryptoAlarmSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlarmTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CryptoSymbols",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoSymbols", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alarms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CryptoSymbolId = table.Column<int>(type: "integer", nullable: false),
                    AlarmTypeId = table.Column<int>(type: "integer", nullable: false),
                    TargetPrice = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    IsTriggered = table.Column<bool>(type: "boolean", nullable: false),
                    TriggeredPrice = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    TriggeredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alarms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alarms_AlarmTypes_AlarmTypeId",
                        column: x => x.AlarmTypeId,
                        principalTable: "AlarmTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alarms_CryptoSymbols_CryptoSymbolId",
                        column: x => x.CryptoSymbolId,
                        principalTable: "CryptoSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlarmNotificationChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlarmId = table.Column<int>(type: "integer", nullable: false),
                    NotificationChannelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmNotificationChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlarmNotificationChannels_Alarms_AlarmId",
                        column: x => x.AlarmId,
                        principalTable: "Alarms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlarmNotificationChannels_NotificationChannels_Notification~",
                        column: x => x.NotificationChannelId,
                        principalTable: "NotificationChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlarmId = table.Column<int>(type: "integer", nullable: false),
                    NotificationChannelId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CryptoSymbolId = table.Column<int>(type: "integer", nullable: false),
                    AlarmTypeId = table.Column<int>(type: "integer", nullable: false),
                    TargetPrice = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    TriggeredPrice = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationLogs_AlarmTypes_AlarmTypeId",
                        column: x => x.AlarmTypeId,
                        principalTable: "AlarmTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationLogs_Alarms_AlarmId",
                        column: x => x.AlarmId,
                        principalTable: "Alarms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationLogs_CryptoSymbols_CryptoSymbolId",
                        column: x => x.CryptoSymbolId,
                        principalTable: "CryptoSymbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationLogs_NotificationChannels_NotificationChannelId",
                        column: x => x.NotificationChannelId,
                        principalTable: "NotificationChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlarmNotificationChannels_AlarmId_NotificationChannelId",
                table: "AlarmNotificationChannels",
                columns: new[] { "AlarmId", "NotificationChannelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlarmNotificationChannels_NotificationChannelId",
                table: "AlarmNotificationChannels",
                column: "NotificationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_AlarmTypeId",
                table: "Alarms",
                column: "AlarmTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_CryptoSymbolId",
                table: "Alarms",
                column: "CryptoSymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_UserId_IsTriggered",
                table: "Alarms",
                columns: new[] { "UserId", "IsTriggered" });

            migrationBuilder.CreateIndex(
                name: "IX_AlarmTypes_Code",
                table: "AlarmTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoSymbols_Code",
                table: "CryptoSymbols",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationChannels_Code",
                table: "NotificationChannels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_AlarmId",
                table: "NotificationLogs",
                column: "AlarmId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_AlarmTypeId",
                table: "NotificationLogs",
                column: "AlarmTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_CryptoSymbolId",
                table: "NotificationLogs",
                column: "CryptoSymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_NotificationChannelId",
                table: "NotificationLogs",
                column: "NotificationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_SentAt",
                table: "NotificationLogs",
                column: "SentAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlarmNotificationChannels");

            migrationBuilder.DropTable(
                name: "NotificationLogs");

            migrationBuilder.DropTable(
                name: "Alarms");

            migrationBuilder.DropTable(
                name: "NotificationChannels");

            migrationBuilder.DropTable(
                name: "AlarmTypes");

            migrationBuilder.DropTable(
                name: "CryptoSymbols");
        }
    }
}
