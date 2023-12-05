﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MudEngine.Database.Seeding.Models;

#nullable disable

namespace MudEngine.Database.Seeding.Migrations
{
    [DbContext(typeof(MudEngineContext))]
    [Migration("20231129213152_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Account", b =>
                {
                    b.Property<Guid>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("HashedPassword")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<DateTime?>("LastAccessed")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(78)
                        .IsUnicode(false)
                        .HasColumnType("varchar(78)");

                    b.HasKey("AccountId");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("AccountId"), false);

                    b.HasIndex(new[] { "AccountId" }, "IX_Account_Name")
                        .IsUnique();

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex(new[] { "AccountId" }, "IX_Account_Name"));

                    b.ToTable("Account", "System");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Clothing", b =>
                {
                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.HasKey("EntityId");

                    b.ToTable("Clothing", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.CommandAssembly", b =>
                {
                    b.Property<Guid>("CommandAssemblyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("Binary")
                        .HasColumnType("varbinary(max)");

                    b.Property<bool>("Preload")
                        .HasColumnType("bit");

                    b.Property<string>("SourceCode")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.HasKey("CommandAssemblyId");

                    b.HasIndex(new[] { "Preload" }, "IX_CommandAssembly_Preload");

                    b.ToTable("CommandAssembly", "System");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.CommandList", b =>
                {
                    b.Property<int>("CommandListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommandListId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(78)
                        .IsUnicode(false)
                        .HasColumnType("varchar(78)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.HasKey("CommandListId");

                    b.HasIndex(new[] { "Name" }, "IX_CommandList_Name")
                        .IsUnique();

                    b.ToTable("CommandList", "System");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.CommandListAssembly", b =>
                {
                    b.Property<int>("CommandListId")
                        .HasColumnType("int");

                    b.Property<Guid>("CommandAssemblyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("HandlesUnknown")
                        .HasColumnType("bit");

                    b.Property<string>("PrimaryAlias")
                        .HasMaxLength(78)
                        .IsUnicode(false)
                        .HasColumnType("varchar(78)");

                    b.HasKey("CommandListId", "CommandAssemblyId");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("CommandListId", "CommandAssemblyId"), false);

                    b.HasIndex(new[] { "CommandListId" }, "IX_CommandListAssembly_CommandListId");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex(new[] { "CommandListId" }, "IX_CommandListAssembly_CommandListId"));

                    b.HasIndex(new[] { "HandlesUnknown" }, "IX_CommandListAssembly_HandlesUnknown");

                    b.HasIndex(new[] { "PrimaryAlias" }, "IX_CommandListAssembly_PrimaryAlias");

                    b.ToTable("CommandListAssembly", "System");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Connection", b =>
                {
                    b.Property<Guid>("ConnectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastCommandRequestedOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PlayerId")
                        .HasColumnType("int");

                    b.HasKey("ConnectionId")
                        .HasName("[PK_Connection]");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("ConnectionId"), false);

                    b.ToTable("Connection", "Transient");

                    SqlServerEntityTypeBuilderExtensions.IsMemoryOptimized(b);
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.ConnectionCommand", b =>
                {
                    b.Property<Guid>("ConnectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CommandListId")
                        .HasColumnType("int");

                    b.HasKey("ConnectionId")
                        .HasName("[PK_ConnectionCommand]");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("ConnectionId"), false);

                    b.ToTable("ConnectionCommand", "Transient");

                    SqlServerEntityTypeBuilderExtensions.IsMemoryOptimized(b);
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.ConnectionVariable", b =>
                {
                    b.Property<Guid>("ConnectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasMaxLength(78)
                        .IsUnicode(false)
                        .HasColumnType("varchar(78)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.HasKey("ConnectionId", "Name")
                        .HasName("[PK_ConnectionVariable]");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("ConnectionId", "Name"), false);

                    b.ToTable("ConnectionVariable", "Transient");

                    SqlServerEntityTypeBuilderExtensions.IsMemoryOptimized(b);
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Container", b =>
                {
                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<int>("ContainerTypeId")
                        .HasColumnType("int");

                    b.HasKey("EntityId");

                    b.HasIndex("ContainerTypeId");

                    b.ToTable("Container", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.ContainerType", b =>
                {
                    b.Property<int>("ContainerTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ContainerTypeId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(78)
                        .IsUnicode(false)
                        .HasColumnType("varchar(78)");

                    b.HasKey("ContainerTypeId");

                    b.ToTable("ContainerType", "Enum");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Entity", b =>
                {
                    b.Property<int>("EntityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EntityId"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<int>("EntityTypeId")
                        .HasColumnType("int");

                    b.Property<bool>("InActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<int?>("ParentEntityId")
                        .HasColumnType("int");

                    b.HasKey("EntityId");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("EntityId"), false);

                    b.HasIndex(new[] { "EntityTypeId" }, "IX_Entity_EntityTypeId");

                    b.HasIndex(new[] { "InActive" }, "IX_Entity_InActive");

                    b.HasIndex(new[] { "ParentEntityId" }, "IX_Entity_ParentEntityId");

                    b.ToTable("Entity", "Mud", t =>
                        {
                            t.HasTrigger("TRG_Entity_OnDelete");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.EntityType", b =>
                {
                    b.Property<int>("EntityTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EntityTypeId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(78)
                        .IsUnicode(false)
                        .HasColumnType("varchar(78)");

                    b.HasKey("EntityTypeId");

                    b.ToTable("EntityType", "Enum");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Living", b =>
                {
                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.HasKey("EntityId");

                    b.ToTable("Living", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Mobile", b =>
                {
                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.HasKey("EntityId");

                    b.ToTable("Mobile", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Mud", b =>
                {
                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.Property<string>("LoginScreen")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("News")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<Guid>("OnNewConnectionCommandId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("EntityId");

                    b.ToTable("Mud", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Player", b =>
                {
                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("EntityId");

                    b.ToTable("Player", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.PlayerAlias", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<string>("Alias")
                        .HasMaxLength(78)
                        .IsUnicode(false)
                        .HasColumnType("varchar(78)");

                    b.Property<string>("Replacement")
                        .IsRequired()
                        .HasMaxLength(78)
                        .IsUnicode(false)
                        .HasColumnType("varchar(78)");

                    b.HasKey("PlayerId", "Alias");

                    b.HasIndex(new[] { "PlayerId" }, "IX_PlayerAlias_PlayerId");

                    b.ToTable("PlayerAlias", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Room", b =>
                {
                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.HasKey("EntityId");

                    b.ToTable("Room", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.RoomExit", b =>
                {
                    b.Property<int>("SourceId")
                        .HasColumnType("int");

                    b.Property<int>("DestinationId")
                        .HasColumnType("int");

                    b.Property<string>("PrimaryAlias")
                        .HasMaxLength(78)
                        .IsUnicode(false)
                        .HasColumnType("varchar(78)");

                    b.Property<int>("RoomExitVisibilityId")
                        .HasColumnType("int");

                    b.HasKey("SourceId", "DestinationId", "PrimaryAlias");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("SourceId", "DestinationId", "PrimaryAlias"), false);

                    b.HasIndex("RoomExitVisibilityId");

                    b.HasIndex(new[] { "SourceId" }, "IX_RoomExit_Source");

                    b.HasIndex(new[] { "SourceId", "PrimaryAlias" }, "IX_RoomExit_SourceAlias");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex(new[] { "SourceId", "PrimaryAlias" }, "IX_RoomExit_SourceAlias"));

                    b.ToTable("RoomExit", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.RoomExitVisibility", b =>
                {
                    b.Property<int>("RoomExitVisibilityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoomExitVisibilityId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(78)
                        .IsUnicode(false)
                        .HasColumnType("varchar(78)");

                    b.HasKey("RoomExitVisibilityId");

                    b.ToTable("RoomExitVisibility", "Enum");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Weapon", b =>
                {
                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.HasKey("EntityId");

                    b.ToTable("Weapon", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Zone", b =>
                {
                    b.Property<int>("EntityId")
                        .HasColumnType("int");

                    b.HasKey("EntityId");

                    b.ToTable("Zone", "Mud");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Clothing", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.Entity", "Entity")
                        .WithOne("Clothing")
                        .HasForeignKey("MudEngine.Database.Seeding.Models.Clothing", "EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Container", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.ContainerType", "ContainerType")
                        .WithMany("Containers")
                        .HasForeignKey("ContainerTypeId")
                        .IsRequired()
                        .HasConstraintName("FK_Container_ContainerType");

                    b.HasOne("MudEngine.Database.Seeding.Models.Entity", "Entity")
                        .WithOne("Container")
                        .HasForeignKey("MudEngine.Database.Seeding.Models.Container", "EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContainerType");

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Entity", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.EntityType", "EntityType")
                        .WithMany("Entities")
                        .HasForeignKey("EntityTypeId")
                        .IsRequired()
                        .HasConstraintName("FK_Entity_EntityType");

                    b.Navigation("EntityType");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Living", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.Entity", "Entity")
                        .WithOne("Living")
                        .HasForeignKey("MudEngine.Database.Seeding.Models.Living", "EntityId")
                        .IsRequired()
                        .HasConstraintName("FK_Living_Entity");

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Mobile", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.Entity", "Entity")
                        .WithOne("Mobile")
                        .HasForeignKey("MudEngine.Database.Seeding.Models.Mobile", "EntityId")
                        .IsRequired()
                        .HasConstraintName("FK_Mobile_Entity");

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Mud", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.Entity", "Entity")
                        .WithOne("Mud")
                        .HasForeignKey("MudEngine.Database.Seeding.Models.Mud", "EntityId")
                        .IsRequired()
                        .HasConstraintName("FK_Mud_Entity");

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Player", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.Entity", "Entity")
                        .WithOne("Player")
                        .HasForeignKey("MudEngine.Database.Seeding.Models.Player", "EntityId")
                        .IsRequired()
                        .HasConstraintName("FK_Player_Entity");

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.PlayerAlias", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.Player", "Player")
                        .WithMany("PlayerAliases")
                        .HasForeignKey("PlayerId")
                        .IsRequired()
                        .HasConstraintName("FK_PlayerAlias_Player");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Room", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.Entity", "Entity")
                        .WithOne("Room")
                        .HasForeignKey("MudEngine.Database.Seeding.Models.Room", "EntityId")
                        .IsRequired()
                        .HasConstraintName("FK_Room_Entity");

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.RoomExit", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.RoomExitVisibility", "RoomExitVisibility")
                        .WithMany("RoomExits")
                        .HasForeignKey("RoomExitVisibilityId")
                        .IsRequired()
                        .HasConstraintName("FK_RoomExit_RoomExitVisibility");

                    b.Navigation("RoomExitVisibility");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Weapon", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.Entity", "Entity")
                        .WithOne("Weapon")
                        .HasForeignKey("MudEngine.Database.Seeding.Models.Weapon", "EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Zone", b =>
                {
                    b.HasOne("MudEngine.Database.Seeding.Models.Entity", "Entity")
                        .WithOne("Zone")
                        .HasForeignKey("MudEngine.Database.Seeding.Models.Zone", "EntityId")
                        .IsRequired()
                        .HasConstraintName("FK_Zone_Entity");

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.ContainerType", b =>
                {
                    b.Navigation("Containers");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Entity", b =>
                {
                    b.Navigation("Clothing");

                    b.Navigation("Container");

                    b.Navigation("Living");

                    b.Navigation("Mobile");

                    b.Navigation("Mud");

                    b.Navigation("Player");

                    b.Navigation("Room");

                    b.Navigation("Weapon");

                    b.Navigation("Zone");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.EntityType", b =>
                {
                    b.Navigation("Entities");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.Player", b =>
                {
                    b.Navigation("PlayerAliases");
                });

            modelBuilder.Entity("MudEngine.Database.Seeding.Models.RoomExitVisibility", b =>
                {
                    b.Navigation("RoomExits");
                });
#pragma warning restore 612, 618
        }
    }
}
