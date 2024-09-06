using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTracker2.Migrations
{
    /// <inheritdoc />
    public partial class AddBodyweightProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBodyWeight",
                table: "Exercises",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBodyWeight",
                table: "Exercises");
        }
    }
}
