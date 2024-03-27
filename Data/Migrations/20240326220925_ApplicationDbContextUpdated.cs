using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationDbContextUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Substep_Tasks_TaskId",
                table: "Substep");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Substep",
                table: "Substep");

            migrationBuilder.RenameTable(
                name: "Substep",
                newName: "Substeps");

            migrationBuilder.RenameIndex(
                name: "IX_Substep_TaskId",
                table: "Substeps",
                newName: "IX_Substeps_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Substeps",
                table: "Substeps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Substeps_Tasks_TaskId",
                table: "Substeps",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Substeps_Tasks_TaskId",
                table: "Substeps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Substeps",
                table: "Substeps");

            migrationBuilder.RenameTable(
                name: "Substeps",
                newName: "Substep");

            migrationBuilder.RenameIndex(
                name: "IX_Substeps_TaskId",
                table: "Substep",
                newName: "IX_Substep_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Substep",
                table: "Substep",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Substep_Tasks_TaskId",
                table: "Substep",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
