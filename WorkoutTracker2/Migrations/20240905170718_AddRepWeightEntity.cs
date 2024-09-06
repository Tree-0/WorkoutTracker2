using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTracker2.Migrations
{
    /// <inheritdoc />
    public partial class AddRepWeightEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sets",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "reps",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "weights",
                table: "Exercises");

            migrationBuilder.CreateTable(
                name: "RepWeights",
                columns: table => new
                {
                    RepWeightId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Reps = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepWeights", x => x.RepWeightId);
                    table.ForeignKey(
                        name: "FK_RepWeights_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "ExerciseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepWeights_ExerciseId",
                table: "RepWeights",
                column: "ExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepWeights");

            migrationBuilder.AddColumn<int>(
                name: "Sets",
                table: "Exercises",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "reps",
                table: "Exercises",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "weights",
                table: "Exercises",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
