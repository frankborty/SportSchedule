using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SportSchedule.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSportsAndChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SportEvents_Sports_SportId",
                table: "SportEvents");

            migrationBuilder.DropTable(
                name: "ChannelSportEvent");

            migrationBuilder.DropTable(
                name: "Sports");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_SportEvents_SportId",
                table: "SportEvents");

            migrationBuilder.DropColumn(
                name: "SportId",
                table: "SportEvents");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "SportEvents",
                newName: "Sport");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "SportEvents",
                newName: "Event");

            migrationBuilder.RenameColumn(
                name: "Canale",
                table: "SportEvents",
                newName: "Competition");

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "SportEvents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "SportEvents");

            migrationBuilder.RenameColumn(
                name: "Sport",
                table: "SportEvents",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Event",
                table: "SportEvents",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "Competition",
                table: "SportEvents",
                newName: "Canale");

            migrationBuilder.AddColumn<int>(
                name: "SportId",
                table: "SportEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelSportEvent",
                columns: table => new
                {
                    ChannelsId = table.Column<int>(type: "integer", nullable: false),
                    EventsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelSportEvent", x => new { x.ChannelsId, x.EventsId });
                    table.ForeignKey(
                        name: "FK_ChannelSportEvent_Channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelSportEvent_SportEvents_EventsId",
                        column: x => x.EventsId,
                        principalTable: "SportEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SportEvents_SportId",
                table: "SportEvents",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Name",
                table: "Channels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelSportEvent_EventsId",
                table: "ChannelSportEvent",
                column: "EventsId");

            migrationBuilder.CreateIndex(
                name: "IX_Sports_Name",
                table: "Sports",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SportEvents_Sports_SportId",
                table: "SportEvents",
                column: "SportId",
                principalTable: "Sports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
