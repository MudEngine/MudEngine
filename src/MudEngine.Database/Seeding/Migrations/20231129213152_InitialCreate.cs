using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudEngine.Database.Seeding.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "System");

            migrationBuilder.EnsureSchema(
                name: "Mud");

            migrationBuilder.EnsureSchema(
                name: "Transient");

            migrationBuilder.EnsureSchema(
                name: "Enum");

            migrationBuilder.AlterDatabase()
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "Account",
                schema: "System",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "varchar(78)", unicode: false, maxLength: 78, nullable: false),
                    HashedPassword = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAccessed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "CommandAssembly",
                schema: "System",
                columns: table => new
                {
                    CommandAssemblyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Preload = table.Column<bool>(type: "bit", nullable: false),
                    SourceCode = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Binary = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandAssembly", x => x.CommandAssemblyId);
                });

            migrationBuilder.CreateTable(
                name: "CommandList",
                schema: "System",
                columns: table => new
                {
                    CommandListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(78)", unicode: false, maxLength: 78, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandList", x => x.CommandListId);
                });

            migrationBuilder.CreateTable(
                name: "CommandListAssembly",
                schema: "System",
                columns: table => new
                {
                    CommandListId = table.Column<int>(type: "int", nullable: false),
                    CommandAssemblyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrimaryAlias = table.Column<string>(type: "varchar(78)", unicode: false, maxLength: 78, nullable: true),
                    HandlesUnknown = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandListAssembly", x => new { x.CommandListId, x.CommandAssemblyId })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Connection",
                schema: "Transient",
                columns: table => new
                {
                    ConnectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlayerId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastCommandRequestedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("[PK_Connection]", x => x.ConnectionId)
                        .Annotation("SqlServer:Clustered", false);
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "ConnectionCommand",
                schema: "Transient",
                columns: table => new
                {
                    ConnectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommandListId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("[PK_ConnectionCommand]", x => x.ConnectionId)
                        .Annotation("SqlServer:Clustered", false);
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "ConnectionVariable",
                schema: "Transient",
                columns: table => new
                {
                    ConnectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(78)", unicode: false, maxLength: 78, nullable: false),
                    Value = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("[PK_ConnectionVariable]", x => new { x.ConnectionId, x.Name })
                        .Annotation("SqlServer:Clustered", false);
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "ContainerType",
                schema: "Enum",
                columns: table => new
                {
                    ContainerTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(78)", unicode: false, maxLength: 78, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerType", x => x.ContainerTypeId);
                });

            migrationBuilder.CreateTable(
                name: "EntityType",
                schema: "Enum",
                columns: table => new
                {
                    EntityTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(78)", unicode: false, maxLength: 78, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityType", x => x.EntityTypeId);
                });

            migrationBuilder.CreateTable(
                name: "RoomExitVisibility",
                schema: "Enum",
                columns: table => new
                {
                    RoomExitVisibilityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(78)", unicode: false, maxLength: 78, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomExitVisibility", x => x.RoomExitVisibilityId);
                });

            migrationBuilder.CreateTable(
                name: "Entity",
                schema: "Mud",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    ParentEntityId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entity", x => x.EntityId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Entity_EntityType",
                        column: x => x.EntityTypeId,
                        principalSchema: "Enum",
                        principalTable: "EntityType",
                        principalColumn: "EntityTypeId");
                });

            migrationBuilder.CreateTable(
                name: "RoomExit",
                schema: "Mud",
                columns: table => new
                {
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    PrimaryAlias = table.Column<string>(type: "varchar(78)", unicode: false, maxLength: 78, nullable: false),
                    RoomExitVisibilityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomExit", x => new { x.SourceId, x.DestinationId, x.PrimaryAlias })
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_RoomExit_RoomExitVisibility",
                        column: x => x.RoomExitVisibilityId,
                        principalSchema: "Enum",
                        principalTable: "RoomExitVisibility",
                        principalColumn: "RoomExitVisibilityId");
                });

            migrationBuilder.CreateTable(
                name: "Clothing",
                schema: "Mud",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clothing", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Clothing_Entity_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "Mud",
                        principalTable: "Entity",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Container",
                schema: "Mud",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    ContainerTypeId = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Container", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Container_ContainerType",
                        column: x => x.ContainerTypeId,
                        principalSchema: "Enum",
                        principalTable: "ContainerType",
                        principalColumn: "ContainerTypeId");
                    table.ForeignKey(
                        name: "FK_Container_Entity_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "Mud",
                        principalTable: "Entity",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Living",
                schema: "Mud",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Living", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Living_Entity",
                        column: x => x.EntityId,
                        principalSchema: "Mud",
                        principalTable: "Entity",
                        principalColumn: "EntityId");
                });

            migrationBuilder.CreateTable(
                name: "Mobile",
                schema: "Mud",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mobile", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Mobile_Entity",
                        column: x => x.EntityId,
                        principalSchema: "Mud",
                        principalTable: "Entity",
                        principalColumn: "EntityId");
                });

            migrationBuilder.CreateTable(
                name: "Mud",
                schema: "Mud",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    OnNewConnectionCommandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginScreen = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    MessageOfTheDay = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mud", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Mud_Entity",
                        column: x => x.EntityId,
                        principalSchema: "Mud",
                        principalTable: "Entity",
                        principalColumn: "EntityId");
                });

            migrationBuilder.CreateTable(
                name: "Player",
                schema: "Mud",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Player_Entity",
                        column: x => x.EntityId,
                        principalSchema: "Mud",
                        principalTable: "Entity",
                        principalColumn: "EntityId");
                });

            migrationBuilder.CreateTable(
                name: "Room",
                schema: "Mud",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Room_Entity",
                        column: x => x.EntityId,
                        principalSchema: "Mud",
                        principalTable: "Entity",
                        principalColumn: "EntityId");
                });

            migrationBuilder.CreateTable(
                name: "Weapon",
                schema: "Mud",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapon", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Weapon_Entity_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "Mud",
                        principalTable: "Entity",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zone",
                schema: "Mud",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zone", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Zone_Entity",
                        column: x => x.EntityId,
                        principalSchema: "Mud",
                        principalTable: "Entity",
                        principalColumn: "EntityId");
                });

            migrationBuilder.CreateTable(
                name: "PlayerAlias",
                schema: "Mud",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Alias = table.Column<string>(type: "varchar(78)", unicode: false, maxLength: 78, nullable: false),
                    Replacement = table.Column<string>(type: "varchar(78)", unicode: false, maxLength: 78, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAlias", x => new { x.PlayerId, x.Alias });
                    table.ForeignKey(
                        name: "FK_PlayerAlias_Player",
                        column: x => x.PlayerId,
                        principalSchema: "Mud",
                        principalTable: "Player",
                        principalColumn: "EntityId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Name",
                schema: "System",
                table: "Account",
                column: "AccountId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_CommandAssembly_Preload",
                schema: "System",
                table: "CommandAssembly",
                column: "Preload");

            migrationBuilder.CreateIndex(
                name: "IX_CommandList_Name",
                schema: "System",
                table: "CommandList",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommandListAssembly_CommandListId",
                schema: "System",
                table: "CommandListAssembly",
                column: "CommandListId")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_CommandListAssembly_HandlesUnknown",
                schema: "System",
                table: "CommandListAssembly",
                column: "HandlesUnknown");

            migrationBuilder.CreateIndex(
                name: "IX_CommandListAssembly_PrimaryAlias",
                schema: "System",
                table: "CommandListAssembly",
                column: "PrimaryAlias");

            migrationBuilder.CreateIndex(
                name: "IX_Container_ContainerTypeId",
                schema: "Mud",
                table: "Container",
                column: "ContainerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Entity_EntityTypeId",
                schema: "Mud",
                table: "Entity",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Entity_InActive",
                schema: "Mud",
                table: "Entity",
                column: "InActive");

            migrationBuilder.CreateIndex(
                name: "IX_Entity_ParentEntityId",
                schema: "Mud",
                table: "Entity",
                column: "ParentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAlias_PlayerId",
                schema: "Mud",
                table: "PlayerAlias",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomExit_RoomExitVisibilityId",
                schema: "Mud",
                table: "RoomExit",
                column: "RoomExitVisibilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomExit_Source",
                schema: "Mud",
                table: "RoomExit",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomExit_SourceAlias",
                schema: "Mud",
                table: "RoomExit",
                columns: new[] { "SourceId", "PrimaryAlias" })
                .Annotation("SqlServer:Clustered", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account",
                schema: "System");

            migrationBuilder.DropTable(
                name: "Clothing",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "CommandAssembly",
                schema: "System");

            migrationBuilder.DropTable(
                name: "CommandList",
                schema: "System");

            migrationBuilder.DropTable(
                name: "CommandListAssembly",
                schema: "System");

            migrationBuilder.DropTable(
                name: "Connection",
                schema: "Transient")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.DropTable(
                name: "ConnectionCommand",
                schema: "Transient")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.DropTable(
                name: "ConnectionVariable",
                schema: "Transient")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.DropTable(
                name: "Container",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "Living",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "Mobile",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "Mud",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "PlayerAlias",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "Room",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "RoomExit",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "Weapon",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "Zone",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "ContainerType",
                schema: "Enum");

            migrationBuilder.DropTable(
                name: "Player",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "RoomExitVisibility",
                schema: "Enum");

            migrationBuilder.DropTable(
                name: "Entity",
                schema: "Mud");

            migrationBuilder.DropTable(
                name: "EntityType",
                schema: "Enum");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("SqlServer:MemoryOptimized", true);
        }
    }
}
