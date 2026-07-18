using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertBridge.Worker.Migrations
{
    /// <inheritdoc />
    public partial class SyncQuartzModelEf10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "misfire_orig_fire_time",
                schema: "quartz",
                table: "qrtz_triggers",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "misfire_orig_fire_time",
                schema: "quartz",
                table: "qrtz_triggers");
        }
    }
}
