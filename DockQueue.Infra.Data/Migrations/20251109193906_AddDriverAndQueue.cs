using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DockQueue.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverAndQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_boxes_Driver_DriverId",
                table: "boxes");

            migrationBuilder.DropForeignKey(
                name: "FK_operator_box_permissions_operator_permissions_OperatorPermi~",
                table: "operator_box_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_operator_status_permissions_operator_permissions_OperatorPe~",
                table: "operator_status_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Driver",
                table: "Driver");

            migrationBuilder.RenameTable(
                name: "Driver",
                newName: "drivers");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiryTime",
                table: "users",
                newName: "refreshtokenexpirytime");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "users",
                newName: "refreshtoken");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "users",
                newName: "number");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "users",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_users_Number",
                table: "users",
                newName: "IX_users_number");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "users",
                newName: "IX_users_email");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "system_settings",
                newName: "updatedat");

            migrationBuilder.RenameColumn(
                name: "TimeZone",
                table: "system_settings",
                newName: "timezone");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "system_settings",
                newName: "starttime");

            migrationBuilder.RenameColumn(
                name: "OperatingDays",
                table: "system_settings",
                newName: "operatingdays");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "system_settings",
                newName: "endtime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "system_settings",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "system_settings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "statuses",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "IsTerminal",
                table: "statuses",
                newName: "isterminal");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "statuses",
                newName: "isdefault");

            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                table: "statuses",
                newName: "displayorder");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "statuses",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "statuses",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "statuses",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "statuses",
                newName: "active");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "statuses",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_statuses_Code",
                table: "statuses",
                newName: "IX_statuses_code");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "operator_status_permissions",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "operator_status_permissions",
                newName: "statusid");

            migrationBuilder.RenameColumn(
                name: "OperatorPermissionsId",
                table: "operator_status_permissions",
                newName: "operatorpermissionsid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "operator_status_permissions",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_operator_status_permissions_UserId_StatusId",
                table: "operator_status_permissions",
                newName: "IX_operator_status_permissions_userid_statusid");

            migrationBuilder.RenameIndex(
                name: "IX_operator_status_permissions_OperatorPermissionsId",
                table: "operator_status_permissions",
                newName: "IX_operator_status_permissions_operatorpermissionsid");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "operator_permissions",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "operator_permissions",
                newName: "updatedat");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "operator_permissions",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "AllowedScreens",
                table: "operator_permissions",
                newName: "allowedscreens");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "operator_permissions",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_operator_permissions_UserId",
                table: "operator_permissions",
                newName: "IX_operator_permissions_userid");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "operator_box_permissions",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "OperatorPermissionsId",
                table: "operator_box_permissions",
                newName: "operatorpermissionsid");

            migrationBuilder.RenameColumn(
                name: "BoxId",
                table: "operator_box_permissions",
                newName: "boxid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "operator_box_permissions",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_operator_box_permissions_UserId_BoxId",
                table: "operator_box_permissions",
                newName: "IX_operator_box_permissions_userid_boxid");

            migrationBuilder.RenameIndex(
                name: "IX_operator_box_permissions_OperatorPermissionsId",
                table: "operator_box_permissions",
                newName: "IX_operator_box_permissions_operatorpermissionsid");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "boxes",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "boxes",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "DriverId",
                table: "boxes",
                newName: "driverid");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "boxes",
                newName: "createdat");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "boxes",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_boxes_DriverId",
                table: "boxes",
                newName: "IX_boxes_driverid");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "drivers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "drivers",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "drivers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "document_number",
                table: "drivers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "vehicle_plate",
                table: "drivers",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_drivers",
                table: "drivers",
                column: "id");

            migrationBuilder.CreateTable(
                name: "queue_entries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(type: "integer", nullable: false),
                    driver_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    box_id = table.Column<int>(type: "integer", nullable: true),
                    last_updated_by_user_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_queue_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_queue_entries_boxes_box_id",
                        column: x => x.box_id,
                        principalTable: "boxes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_queue_entries_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_queue_entries_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_queue_entries_users_last_updated_by_user_id",
                        column: x => x.last_updated_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "queue_entry_status_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    queue_entry_id = table.Column<int>(type: "integer", nullable: false),
                    old_status_id = table.Column<int>(type: "integer", nullable: false),
                    new_status_id = table.Column<int>(type: "integer", nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    changed_by_user_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_queue_entry_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_queue_entry_status_history_queue_entries_queue_entry_id",
                        column: x => x.queue_entry_id,
                        principalTable: "queue_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_queue_entry_status_history_users_changed_by_user_id",
                        column: x => x.changed_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_drivers_document_number",
                table: "drivers",
                column: "document_number");

            migrationBuilder.CreateIndex(
                name: "ix_drivers_vehicle_plate",
                table: "drivers",
                column: "vehicle_plate");

            migrationBuilder.CreateIndex(
                name: "IX_queue_entries_box_id",
                table: "queue_entries",
                column: "box_id");

            migrationBuilder.CreateIndex(
                name: "IX_queue_entries_driver_id",
                table: "queue_entries",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "IX_queue_entries_last_updated_by_user_id",
                table: "queue_entries",
                column: "last_updated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_queue_entries_status_id",
                table: "queue_entries",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_queue_entries_type_position",
                table: "queue_entries",
                columns: new[] { "type", "position" });

            migrationBuilder.CreateIndex(
                name: "IX_queue_entry_status_history_changed_by_user_id",
                table: "queue_entry_status_history",
                column: "changed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_queue_entry_status_history_queue_entry_id",
                table: "queue_entry_status_history",
                column: "queue_entry_id");

            migrationBuilder.AddForeignKey(
                name: "FK_boxes_drivers_driverid",
                table: "boxes",
                column: "driverid",
                principalTable: "drivers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_operator_box_permissions_operator_permissions_operatorpermi~",
                table: "operator_box_permissions",
                column: "operatorpermissionsid",
                principalTable: "operator_permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_operator_status_permissions_operator_permissions_operatorpe~",
                table: "operator_status_permissions",
                column: "operatorpermissionsid",
                principalTable: "operator_permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_boxes_drivers_driverid",
                table: "boxes");

            migrationBuilder.DropForeignKey(
                name: "FK_operator_box_permissions_operator_permissions_operatorpermi~",
                table: "operator_box_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_operator_status_permissions_operator_permissions_operatorpe~",
                table: "operator_status_permissions");

            migrationBuilder.DropTable(
                name: "queue_entry_status_history");

            migrationBuilder.DropTable(
                name: "queue_entries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_drivers",
                table: "drivers");

            migrationBuilder.DropIndex(
                name: "ix_drivers_document_number",
                table: "drivers");

            migrationBuilder.DropIndex(
                name: "ix_drivers_vehicle_plate",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "document_number",
                table: "drivers");

            migrationBuilder.DropColumn(
                name: "vehicle_plate",
                table: "drivers");

            migrationBuilder.RenameTable(
                name: "drivers",
                newName: "Driver");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "refreshtokenexpirytime",
                table: "users",
                newName: "RefreshTokenExpiryTime");

            migrationBuilder.RenameColumn(
                name: "refreshtoken",
                table: "users",
                newName: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "number",
                table: "users",
                newName: "Number");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_users_number",
                table: "users",
                newName: "IX_users_Number");

            migrationBuilder.RenameIndex(
                name: "IX_users_email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameColumn(
                name: "updatedat",
                table: "system_settings",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "timezone",
                table: "system_settings",
                newName: "TimeZone");

            migrationBuilder.RenameColumn(
                name: "starttime",
                table: "system_settings",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "operatingdays",
                table: "system_settings",
                newName: "OperatingDays");

            migrationBuilder.RenameColumn(
                name: "endtime",
                table: "system_settings",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "system_settings",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "system_settings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "statuses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "isterminal",
                table: "statuses",
                newName: "IsTerminal");

            migrationBuilder.RenameColumn(
                name: "isdefault",
                table: "statuses",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "displayorder",
                table: "statuses",
                newName: "DisplayOrder");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "statuses",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "statuses",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "statuses",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "active",
                table: "statuses",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "statuses",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_statuses_code",
                table: "statuses",
                newName: "IX_statuses_Code");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "operator_status_permissions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "statusid",
                table: "operator_status_permissions",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "operatorpermissionsid",
                table: "operator_status_permissions",
                newName: "OperatorPermissionsId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "operator_status_permissions",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_operator_status_permissions_userid_statusid",
                table: "operator_status_permissions",
                newName: "IX_operator_status_permissions_UserId_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_operator_status_permissions_operatorpermissionsid",
                table: "operator_status_permissions",
                newName: "IX_operator_status_permissions_OperatorPermissionsId");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "operator_permissions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updatedat",
                table: "operator_permissions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "operator_permissions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "allowedscreens",
                table: "operator_permissions",
                newName: "AllowedScreens");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "operator_permissions",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_operator_permissions_userid",
                table: "operator_permissions",
                newName: "IX_operator_permissions_UserId");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "operator_box_permissions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "operatorpermissionsid",
                table: "operator_box_permissions",
                newName: "OperatorPermissionsId");

            migrationBuilder.RenameColumn(
                name: "boxid",
                table: "operator_box_permissions",
                newName: "BoxId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "operator_box_permissions",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_operator_box_permissions_userid_boxid",
                table: "operator_box_permissions",
                newName: "IX_operator_box_permissions_UserId_BoxId");

            migrationBuilder.RenameIndex(
                name: "IX_operator_box_permissions_operatorpermissionsid",
                table: "operator_box_permissions",
                newName: "IX_operator_box_permissions_OperatorPermissionsId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "boxes",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "boxes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "driverid",
                table: "boxes",
                newName: "DriverId");

            migrationBuilder.RenameColumn(
                name: "createdat",
                table: "boxes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "boxes",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_boxes_driverid",
                table: "boxes",
                newName: "IX_boxes_DriverId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Driver",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Driver",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Driver",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Driver",
                table: "Driver",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_boxes_Driver_DriverId",
                table: "boxes",
                column: "DriverId",
                principalTable: "Driver",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_operator_box_permissions_operator_permissions_OperatorPermi~",
                table: "operator_box_permissions",
                column: "OperatorPermissionsId",
                principalTable: "operator_permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_operator_status_permissions_operator_permissions_OperatorPe~",
                table: "operator_status_permissions",
                column: "OperatorPermissionsId",
                principalTable: "operator_permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
