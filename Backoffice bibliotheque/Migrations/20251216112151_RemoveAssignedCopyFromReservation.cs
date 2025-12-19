using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backoffice_bibliotheque.Migrations
{
    public partial class RemoveAssignedCopyFromReservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "AssignedCopyId",
                table: "Reservations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
        name: "AssignedCopyId",
        table: "Reservations",
        type: "int",
        nullable: true);
        }
    }
}
