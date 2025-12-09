using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSchedule.Migrations
{
    /// <inheritdoc />
    public partial class FixUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SportEvents_Competition_Sport_Time_Channel",
                table: "SportEvents");

            migrationBuilder.CreateIndex(
                name: "IX_SportEvents_Competition_Sport_Time_Event_Channel",
                table: "SportEvents",
                columns: new[] { "Competition", "Sport", "Time", "Event", "Channel" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SportEvents_Competition_Sport_Time_Event_Channel",
                table: "SportEvents");

            migrationBuilder.CreateIndex(
                name: "IX_SportEvents_Competition_Sport_Time_Channel",
                table: "SportEvents",
                columns: new[] { "Competition", "Sport", "Time", "Channel" },
                unique: true);
        }
    }
}
