using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Texnomic.SecureDNS.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blacklists",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Domain = table.Column<string>(nullable: true),
                    Timestamp = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blacklists", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Cache",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Domain = table.Column<string>(nullable: true),
                    Response = table.Column<byte[]>(nullable: true),
                    Timestamp = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cache", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Hosts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Domain = table.Column<string>(nullable: true),
                    IPAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hosts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Resolvers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Domain = table.Column<string>(nullable: true),
                    IPAddress = table.Column<string>(nullable: true),
                    Hash = table.Column<byte[]>(nullable: true),
                    UDP = table.Column<bool>(nullable: false),
                    TCP = table.Column<bool>(nullable: false),
                    TLS = table.Column<bool>(nullable: false),
                    HTTPS = table.Column<bool>(nullable: false),
                    CRYPT = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resolvers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    RoleClaimID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.RoleClaimID);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    UserClaimID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.UserClaimID);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserID = table.Column<Guid>(nullable: false),
                    RoleID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserID, x.RoleID });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserID = table.Column<Guid>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserID, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Hosts",
                columns: new[] { "ID", "Domain", "IPAddress" },
                values: new object[] { 1, "www.secure.dns", "127.0.0.1" });

            migrationBuilder.InsertData(
                table: "Resolvers",
                columns: new[] { "ID", "CRYPT", "Domain", "HTTPS", "Hash", "IPAddress", "Name", "TCP", "TLS", "UDP" },
                values: new object[] { 1, false, "cloudflare-dns.com", true, new byte[] { 4, 197, 32, 112, 140, 32, 66, 80, 40, 30, 125, 68, 65, 124, 48, 121, 41, 28, 99, 94, 29, 68, 155, 197, 247, 113, 58, 43, 222, 210, 162, 164, 177, 108, 61, 106, 200, 119, 184, 203, 143, 46, 80, 83, 253, 244, 24, 38, 127, 97, 55, 237, 255, 194, 190, 233, 11, 93, 185, 126, 225, 223, 28, 226, 116 }, "1.1.1.1", "CloudFlare #1", true, true, true });

            migrationBuilder.InsertData(
                table: "Resolvers",
                columns: new[] { "ID", "CRYPT", "Domain", "HTTPS", "Hash", "IPAddress", "Name", "TCP", "TLS", "UDP" },
                values: new object[] { 2, false, "cloudflare-dns.com", true, new byte[] { 4, 197, 32, 112, 140, 32, 66, 80, 40, 30, 125, 68, 65, 124, 48, 121, 41, 28, 99, 94, 29, 68, 155, 197, 247, 113, 58, 43, 222, 210, 162, 164, 177, 108, 61, 106, 200, 119, 184, 203, 143, 46, 80, 83, 253, 244, 24, 38, 127, 97, 55, 237, 255, 194, 190, 233, 11, 93, 185, 126, 225, 223, 28, 226, 116 }, "1.0.0.1", "CloudFlare #2", true, true, true });

            migrationBuilder.InsertData(
                table: "Resolvers",
                columns: new[] { "ID", "CRYPT", "Domain", "HTTPS", "Hash", "IPAddress", "Name", "TCP", "TLS", "UDP" },
                values: new object[] { 3, false, "google-public-dns-a.google.com", true, new byte[] { 48, 130, 1, 10, 2, 130, 1, 1, 0, 157, 31, 164, 239, 93, 62, 136, 51, 25, 171, 231, 154, 109, 200, 43, 247, 42, 60, 29, 49, 46, 173, 93, 171, 65, 67, 54, 143, 4, 45, 69, 250, 129, 155, 200, 221, 30, 63, 2, 39, 162, 162, 57, 141, 185, 69, 224, 171, 63, 26, 177, 67, 168, 127, 131, 136, 75, 252, 203, 64, 35, 13, 170, 103, 60, 42, 68, 30, 202, 223, 57, 42, 187, 219, 124, 163, 103, 125, 206, 4, 187, 146, 72, 11, 188, 95, 100, 170, 202, 26, 91, 178, 149, 166, 106, 14, 220, 231, 6, 15, 5, 184, 139, 202, 8, 208, 171, 54, 41, 11, 25, 40, 21, 80, 78, 88, 151, 45, 96, 175, 249, 247, 236, 141, 171, 72, 129, 7, 227, 46, 47, 137, 176, 219, 205, 245, 206, 2, 37, 48, 235, 50, 209, 116, 8, 38, 251, 117, 132, 168, 18, 37, 186, 204, 112, 0, 91, 116, 69, 62, 173, 207, 62, 226, 160, 174, 117, 233, 179, 216, 90, 52, 22, 161, 13, 118, 112, 205, 31, 254, 53, 212, 60, 3, 163, 113, 221, 251, 228, 246, 135, 188, 249, 2, 222, 203, 104, 85, 34, 66, 240, 33, 31, 169, 87, 46, 97, 211, 187, 191, 238, 231, 130, 33, 154, 229, 121, 54, 8, 238, 6, 128, 11, 52, 125, 99, 64, 115, 37, 33, 238, 184, 19, 171, 84, 75, 182, 3, 20, 140, 234, 247, 193, 170, 166, 46, 244, 218, 42, 194, 52, 21, 5, 44, 22, 185, 2, 3, 1, 0, 1 }, "8.8.8.8", "Google #1", true, true, true });

            migrationBuilder.InsertData(
                table: "Resolvers",
                columns: new[] { "ID", "CRYPT", "Domain", "HTTPS", "Hash", "IPAddress", "Name", "TCP", "TLS", "UDP" },
                values: new object[] { 4, false, "google-public-dns-b.google.com", true, new byte[] { 48, 130, 1, 10, 2, 130, 1, 1, 0, 157, 31, 164, 239, 93, 62, 136, 51, 25, 171, 231, 154, 109, 200, 43, 247, 42, 60, 29, 49, 46, 173, 93, 171, 65, 67, 54, 143, 4, 45, 69, 250, 129, 155, 200, 221, 30, 63, 2, 39, 162, 162, 57, 141, 185, 69, 224, 171, 63, 26, 177, 67, 168, 127, 131, 136, 75, 252, 203, 64, 35, 13, 170, 103, 60, 42, 68, 30, 202, 223, 57, 42, 187, 219, 124, 163, 103, 125, 206, 4, 187, 146, 72, 11, 188, 95, 100, 170, 202, 26, 91, 178, 149, 166, 106, 14, 220, 231, 6, 15, 5, 184, 139, 202, 8, 208, 171, 54, 41, 11, 25, 40, 21, 80, 78, 88, 151, 45, 96, 175, 249, 247, 236, 141, 171, 72, 129, 7, 227, 46, 47, 137, 176, 219, 205, 245, 206, 2, 37, 48, 235, 50, 209, 116, 8, 38, 251, 117, 132, 168, 18, 37, 186, 204, 112, 0, 91, 116, 69, 62, 173, 207, 62, 226, 160, 174, 117, 233, 179, 216, 90, 52, 22, 161, 13, 118, 112, 205, 31, 254, 53, 212, 60, 3, 163, 113, 221, 251, 228, 246, 135, 188, 249, 2, 222, 203, 104, 85, 34, 66, 240, 33, 31, 169, 87, 46, 97, 211, 187, 191, 238, 231, 130, 33, 154, 229, 121, 54, 8, 238, 6, 128, 11, 52, 125, 99, 64, 115, 37, 33, 238, 184, 19, 171, 84, 75, 182, 3, 20, 140, 234, 247, 193, 170, 166, 46, 244, 218, 42, 194, 52, 21, 5, 44, 22, 185, 2, 3, 1, 0, 1 }, "8.8.4.4", "Google #2", true, true, true });

            migrationBuilder.InsertData(
                table: "Resolvers",
                columns: new[] { "ID", "CRYPT", "Domain", "HTTPS", "Hash", "IPAddress", "Name", "TCP", "TLS", "UDP" },
                values: new object[] { 5, false, "dns.quad9.net", true, new byte[] { 4, 125, 139, 215, 29, 3, 133, 13, 24, 37, 179, 52, 28, 41, 161, 39, 212, 172, 1, 37, 72, 138, 160, 241, 234, 2, 185, 216, 81, 44, 8, 106, 172, 114, 86, 236, 250, 61, 166, 160, 159, 73, 9, 85, 142, 172, 254, 185, 115, 23, 92, 2, 251, 120, 204, 36, 145, 148, 111, 67, 35, 137, 14, 29, 102 }, "9.9.9.9", "Quad9", true, true, true });

            migrationBuilder.InsertData(
                table: "Resolvers",
                columns: new[] { "ID", "CRYPT", "Domain", "HTTPS", "Hash", "IPAddress", "Name", "TCP", "TLS", "UDP" },
                values: new object[] { 6, true, "resolver1-fs.opendns.com", false, null, "208.67.222.123", "OpenDNS #1", false, false, true });

            migrationBuilder.InsertData(
                table: "Resolvers",
                columns: new[] { "ID", "CRYPT", "Domain", "HTTPS", "Hash", "IPAddress", "Name", "TCP", "TLS", "UDP" },
                values: new object[] { 7, true, "resolver2-fs.opendns.com", false, null, "208.67.220.123", "OpenDNS #2", false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Blacklists_Domain",
                table: "Blacklists",
                column: "Domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cache_Domain",
                table: "Cache",
                column: "Domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hosts_Domain",
                table: "Hosts",
                column: "Domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resolvers_IPAddress",
                table: "Resolvers",
                column: "IPAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserID",
                table: "UserClaims",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserID",
                table: "UserLogins",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleID",
                table: "UserRoles",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blacklists");

            migrationBuilder.DropTable(
                name: "Cache");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "Hosts");

            migrationBuilder.DropTable(
                name: "Resolvers");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
