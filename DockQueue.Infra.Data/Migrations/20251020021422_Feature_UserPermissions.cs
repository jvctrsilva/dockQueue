using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DockQueue.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Feature_UserPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "operator_permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AllowedScreens = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operator_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "operator_box_permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    BoxId = table.Column<int>(type: "integer", nullable: false),
                    OperatorPermissionsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operator_box_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_operator_box_permissions_operator_permissions_OperatorPermi~",
                        column: x => x.OperatorPermissionsId,
                        principalTable: "operator_permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "operator_status_permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    OperatorPermissionsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operator_status_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_operator_status_permissions_operator_permissions_OperatorPe~",
                        column: x => x.OperatorPermissionsId,
                        principalTable: "operator_permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_operator_box_permissions_OperatorPermissionsId",
                table: "operator_box_permissions",
                column: "OperatorPermissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_operator_box_permissions_UserId_BoxId",
                table: "operator_box_permissions",
                columns: new[] { "UserId", "BoxId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_operator_permissions_UserId",
                table: "operator_permissions",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_operator_status_permissions_OperatorPermissionsId",
                table: "operator_status_permissions",
                column: "OperatorPermissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_operator_status_permissions_UserId_StatusId",
                table: "operator_status_permissions",
                columns: new[] { "UserId", "StatusId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operator_box_permissions");

            migrationBuilder.DropTable(
                name: "operator_status_permissions");

            migrationBuilder.DropTable(
                name: "operator_permissions");
        }
    }
}
