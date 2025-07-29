using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanbanFlow.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionAndWipLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "TaskItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WipLimit",
                table: "Columns",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.Sql(@"
                WITH NumberedTasks AS (
                    SELECT Id, ROW_NUMBER() OVER(PARTITION BY ColumnId ORDER BY Id) as rn
                    FROM TaskItems
                )
                UPDATE TaskItems
                SET Position = (SELECT rn FROM NumberedTasks WHERE NumberedTasks.Id = TaskItems.Id) - 1;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_Position_ColumnId",
                table: "TaskItems",
                columns: new[] { "Position", "ColumnId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskItems_Position_ColumnId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "WipLimit",
                table: "Columns");
        }
    }
}
