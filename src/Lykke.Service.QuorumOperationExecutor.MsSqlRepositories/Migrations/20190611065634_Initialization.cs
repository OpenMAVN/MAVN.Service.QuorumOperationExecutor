// ReSharper disable All
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.QuorumOperationExecutor.MsSqlRepositories.Migrations
{
    public partial class Initialization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "quorum_operation_executor");

            migrationBuilder.CreateTable(
                name: "operations",
                schema: "quorum_operation_executor",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    transactionhash = table.Column<string>(name: "transaction_hash", nullable: true),
                    transactiondata = table.Column<string>(name: "transaction_data", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operations", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operations",
                schema: "quorum_operation_executor");
        }
    }
}
