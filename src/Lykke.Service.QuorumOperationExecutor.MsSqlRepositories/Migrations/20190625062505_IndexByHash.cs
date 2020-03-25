using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.QuorumOperationExecutor.MsSqlRepositories.Migrations
{
    public partial class IndexByHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "transaction_hash",
                schema: "quorum_operation_executor",
                table: "operations",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_operations_transaction_hash",
                schema: "quorum_operation_executor",
                table: "operations",
                column: "transaction_hash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_operations_transaction_hash",
                schema: "quorum_operation_executor",
                table: "operations");

            migrationBuilder.AlterColumn<string>(
                name: "transaction_hash",
                schema: "quorum_operation_executor",
                table: "operations",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
