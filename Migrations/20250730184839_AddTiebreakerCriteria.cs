using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossfitLeaderboard.Migrations
{
    /// <inheritdoc />
    public partial class AddTiebreakerCriteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FirstPlaceCount",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecondPlaceCount",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstPlaceCount",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "SecondPlaceCount",
                table: "Teams");
        }
    }
}
