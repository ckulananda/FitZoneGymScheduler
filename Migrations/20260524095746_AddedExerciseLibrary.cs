using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitZoneGymScheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddedExerciseLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExerciseName",
                table: "WorkoutExercises");

            migrationBuilder.AddColumn<int>(
                name: "ExerciseLibraryId",
                table: "WorkoutExercises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExerciseLibraries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExerciseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetArea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseLibraries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseLibraryId",
                table: "WorkoutExercises",
                column: "ExerciseLibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_ExerciseLibraries_ExerciseLibraryId",
                table: "WorkoutExercises",
                column: "ExerciseLibraryId",
                principalTable: "ExerciseLibraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_ExerciseLibraries_ExerciseLibraryId",
                table: "WorkoutExercises");

            migrationBuilder.DropTable(
                name: "ExerciseLibraries");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_ExerciseLibraryId",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "ExerciseLibraryId",
                table: "WorkoutExercises");

            migrationBuilder.AddColumn<string>(
                name: "ExerciseName",
                table: "WorkoutExercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
