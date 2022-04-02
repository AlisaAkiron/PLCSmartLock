using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PLCHost.Migrations
{
    public partial class ChangeTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Persistents",
                table: "Persistents");

            migrationBuilder.RenameTable(
                name: "Persistents",
                newName: "persistent");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "persistent",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "persistent",
                newName: "key");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "persistent",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_persistent",
                table: "persistent",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_persistent",
                table: "persistent");

            migrationBuilder.RenameTable(
                name: "persistent",
                newName: "Persistents");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "Persistents",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "key",
                table: "Persistents",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Persistents",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Persistents",
                table: "Persistents",
                column: "Id");
        }
    }
}
