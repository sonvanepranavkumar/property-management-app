using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPropertyStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Properties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
