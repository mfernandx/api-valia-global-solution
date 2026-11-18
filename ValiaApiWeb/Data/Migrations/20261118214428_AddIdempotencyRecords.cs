using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValiaApiWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencyRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IDEMPOTENCY_RECORDS",
                columns: table => new
                {
                    IDEMPOTENCY_RECORD_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    USER_ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OPERATION = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    IDEMPOTENCY_KEY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    REQUEST_HASH = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false),
                    RESPONSE_BODY = table.Column<string>(type: "CLOB", nullable: false),
                    STATUS_CODE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IDEMPOTENCY_RECORDS", x => x.IDEMPOTENCY_RECORD_ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IDEMPOTENCY_RECORDS_USER_ID_OPERATION_IDEMPOTENCY_KEY",
                table: "IDEMPOTENCY_RECORDS",
                columns: new[] { "USER_ID", "OPERATION", "IDEMPOTENCY_KEY" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IDEMPOTENCY_RECORDS");
        }
    }
}
