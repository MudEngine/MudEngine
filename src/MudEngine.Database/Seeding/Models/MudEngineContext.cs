using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MudEngine.Database.Seeding.Models;

public partial class MudEngineContext : DbContext
{
    public MudEngineContext()
    {
    }

    public MudEngineContext(DbContextOptions<MudEngineContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Clothing> Clothings { get; set; }

    public virtual DbSet<CommandAssembly> CommandAssemblies { get; set; }

    public virtual DbSet<CommandList> CommandLists { get; set; }

    public virtual DbSet<CommandListAssembly> CommandListAssemblies { get; set; }

    public virtual DbSet<Connection> Connections { get; set; }

    public virtual DbSet<ConnectionCommand> ConnectionCommands { get; set; }

    public virtual DbSet<ConnectionVariable> ConnectionVariables { get; set; }

    public virtual DbSet<Container> Containers { get; set; }

    public virtual DbSet<ContainerType> ContainerTypes { get; set; }

    public virtual DbSet<Entity> Entities { get; set; }

    public virtual DbSet<EntityType> EntityTypes { get; set; }

    public virtual DbSet<Living> Livings { get; set; }

    public virtual DbSet<Mobile> Mobiles { get; set; }

    public virtual DbSet<Mud> Mud { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerAlias> PlayerAliases { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomExit> RoomExits { get; set; }

    public virtual DbSet<RoomExitVisibility> RoomExitVisibilities { get; set; }

    public virtual DbSet<Weapon> Weapons { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), true)
            .Build();
        var connectionString =
            configuration.GetConnectionString("MudEngine")
            ?? "Data Source=.;Initial Catalog=MudEngine;Integrated Security=SSPI;MultipleActiveResultSets=True;TrustServerCertificate=true";
        options.UseSqlServer(connectionString);
        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).IsClustered(false);

            entity.ToTable("Account", "System");

            entity.HasIndex(e => e.AccountId, "IX_Account_Name")
                .IsUnique()
                .IsClustered();

            entity.Property(e => e.AccountId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.HashedPassword).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(78)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Clothing>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("Clothing", "Mud");

            entity.Property(e => e.EntityId).ValueGeneratedNever();

            entity.HasOne(d => d.Entity).WithOne(p => p.Clothing).HasForeignKey<Clothing>(d => d.EntityId);
        });

        modelBuilder.Entity<CommandAssembly>(entity =>
        {
            entity.ToTable("CommandAssembly", "System");

            entity.HasIndex(e => e.Preload, "IX_CommandAssembly_Preload");

            entity.Property(e => e.CommandAssemblyId).ValueGeneratedNever();
            entity.Property(e => e.SourceCode).IsUnicode(false);
        });

        modelBuilder.Entity<CommandList>(entity =>
        {
            entity.ToTable("CommandList", "System");

            entity.HasIndex(e => e.Name, "IX_CommandList_Name").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(78)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CommandListAssembly>(entity =>
        {
            entity.HasKey(e => new { e.CommandListId, e.CommandAssemblyId }).IsClustered(false);

            entity.ToTable("CommandListAssembly", "System");

            entity.HasIndex(e => e.CommandListId, "IX_CommandListAssembly_CommandListId").IsClustered();

            entity.HasIndex(e => e.HandlesUnknown, "IX_CommandListAssembly_HandlesUnknown");

            entity.HasIndex(e => e.PrimaryAlias, "IX_CommandListAssembly_PrimaryAlias");

            entity.Property(e => e.PrimaryAlias)
                .HasMaxLength(78)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Connection>(entity =>
        {
            entity.HasKey(e => e.ConnectionId)
                .HasName("[PK_Connection]")
                .IsClustered(false);

            entity
                .ToTable("Connection", "Transient", t => t.IsMemoryOptimized());

            entity.Property(e => e.ConnectionId).ValueGeneratedNever();
        });

        modelBuilder.Entity<ConnectionCommand>(entity =>
        {
            entity.HasKey(e => e.ConnectionId)
                .HasName("[PK_ConnectionCommand]")
                .IsClustered(false);

            entity
                .ToTable("ConnectionCommand", "Transient", t => t.IsMemoryOptimized());

            entity.Property(e => e.ConnectionId).ValueGeneratedNever();
        });

        modelBuilder.Entity<ConnectionVariable>(entity =>
        {
            entity.HasKey(e => new { e.ConnectionId, e.Name })
                .HasName("[PK_ConnectionVariable]")
                .IsClustered(false);

            entity
                .ToTable("ConnectionVariable", "Transient", t => t.IsMemoryOptimized());

            entity.Property(e => e.Name)
                .HasMaxLength(78)
                .IsUnicode(false);
            entity.Property(e => e.Value).IsUnicode(false);
        });

        modelBuilder.Entity<Container>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("Container", "Mud");

            entity.Property(e => e.EntityId).ValueGeneratedNever();

            entity.HasOne(d => d.ContainerType).WithMany(p => p.Containers)
                .HasForeignKey(d => d.ContainerTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Container_ContainerType");

            entity.HasOne(d => d.Entity).WithOne(p => p.Container).HasForeignKey<Container>(d => d.EntityId);
        });

        modelBuilder.Entity<ContainerType>(entity =>
        {
            entity.ToTable("ContainerType", "Enum");

            entity.Property(e => e.Name)
                .HasMaxLength(78)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Entity>(entity =>
        {
            entity.HasKey(e => e.EntityId).IsClustered(false);

            entity.ToTable("Entity", "Mud", tb => tb.HasTrigger("TRG_Entity_OnDelete"));

            entity.HasIndex(e => e.EntityTypeId, "IX_Entity_EntityTypeId");

            entity.HasIndex(e => e.InActive, "IX_Entity_InActive");

            entity.HasIndex(e => e.ParentEntityId, "IX_Entity_ParentEntityId");

            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);

            entity.HasOne(d => d.EntityType).WithMany(p => p.Entities)
                .HasForeignKey(d => d.EntityTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Entity_EntityType");
        });

        modelBuilder.Entity<EntityType>(entity =>
        {
            entity.ToTable("EntityType", "Enum");

            entity.Property(e => e.Name)
                .HasMaxLength(78)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Living>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("Living", "Mud");

            entity.Property(e => e.EntityId).ValueGeneratedNever();

            entity.HasOne(d => d.Entity).WithOne(p => p.Living)
                .HasForeignKey<Living>(d => d.EntityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Living_Entity");
        });

        modelBuilder.Entity<Mobile>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("Mobile", "Mud");

            entity.Property(e => e.EntityId).ValueGeneratedNever();

            entity.HasOne(d => d.Entity).WithOne(p => p.Mobile)
                .HasForeignKey<Mobile>(d => d.EntityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mobile_Entity");
        });

        modelBuilder.Entity<Mud>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("Mud", "Mud");

            entity.Property(e => e.EntityId).ValueGeneratedNever();
            entity.Property(e => e.LoginScreen).IsUnicode(false);
            entity.Property(e => e.MessageOfTheDay).IsUnicode(false);

            entity.HasOne(d => d.Entity).WithOne(p => p.Mud)
                .HasForeignKey<Mud>(d => d.EntityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mud_Entity");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("Player", "Mud");

            entity.Property(e => e.EntityId).ValueGeneratedNever();

            entity.HasOne(d => d.Entity).WithOne(p => p.Player)
                .HasForeignKey<Player>(d => d.EntityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Player_Entity");
        });

        modelBuilder.Entity<PlayerAlias>(entity =>
        {
            entity.HasKey(e => new { e.PlayerId, e.Alias });

            entity.ToTable("PlayerAlias", "Mud");

            entity.HasIndex(e => e.PlayerId, "IX_PlayerAlias_PlayerId");

            entity.Property(e => e.Alias)
                .HasMaxLength(78)
                .IsUnicode(false);
            entity.Property(e => e.Replacement)
                .HasMaxLength(78)
                .IsUnicode(false);

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerAliases)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlayerAlias_Player");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("Room", "Mud");

            entity.Property(e => e.EntityId).ValueGeneratedNever();

            entity.HasOne(d => d.Entity).WithOne(p => p.Room)
                .HasForeignKey<Room>(d => d.EntityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Room_Entity");
        });

        modelBuilder.Entity<RoomExit>(entity =>
        {
            entity.HasKey(e => new { e.SourceId, e.DestinationId, e.PrimaryAlias }).IsClustered(false);

            entity.ToTable("RoomExit", "Mud");

            entity.HasIndex(e => e.SourceId, "IX_RoomExit_Source");

            entity.HasIndex(e => new { e.SourceId, e.PrimaryAlias }, "IX_RoomExit_SourceAlias").IsClustered();

            entity.Property(e => e.PrimaryAlias)
                .HasMaxLength(78)
                .IsUnicode(false);

            entity.HasOne(d => d.RoomExitVisibility).WithMany(p => p.RoomExits)
                .HasForeignKey(d => d.RoomExitVisibilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomExit_RoomExitVisibility");
        });

        modelBuilder.Entity<RoomExitVisibility>(entity =>
        {
            entity.ToTable("RoomExitVisibility", "Enum");

            entity.Property(e => e.Name)
                .HasMaxLength(78)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Weapon>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("Weapon", "Mud");

            entity.Property(e => e.EntityId).ValueGeneratedNever();

            entity.HasOne(d => d.Entity).WithOne(p => p.Weapon).HasForeignKey<Weapon>(d => d.EntityId);
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.EntityId);

            entity.ToTable("Zone", "Mud");

            entity.Property(e => e.EntityId).ValueGeneratedNever();

            entity.HasOne(d => d.Entity).WithOne(p => p.Zone)
                .HasForeignKey<Zone>(d => d.EntityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zone_Entity");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
