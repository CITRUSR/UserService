using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDateTypeToDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FiredAt",
                table: "Teachers",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "DroppedOutAt",
                table: "Students",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "Groups",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "GraduatedAt",
                table: "Groups",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldNullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FiredAt",
                table: "Teachers",
                type: "timestamp",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "DroppedOutAt",
                table: "Students",
                type: "timestamp",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "Groups",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "GraduatedAt",
                table: "Groups",
                type: "timestamp",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true
            );
        }
    }
}
