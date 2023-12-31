﻿using MudEngine.Library.Domain.Enum;
using MudEngine.Library.System;
namespace MudEngine.Library.Commands.Basic;

[Command("fc8b6041-ba8a-4c99-97f7-b01495eff307")]
public class Look : BaseCommand, ICommand
{
    public override CommandResponse Execute(CommandRequest Request)
    {
        base.Execute(Request);
        var entityId = IdentifySubject();
        if (entityId <= 0)
        {
            AddMessage("Look at what?[CR]");
            return Response;
        }
        var entityDetails = GetEntityDetails(entityId);
        if (entityDetails.EntityId <= 0)
        {
            AddMessage("Look at what?[CR]");
            return Response;
        }
        switch (entityDetails.EntityType)
        {
            case EntityType.Room:
                var thisPlayer = ThisPlayer();
                AddMessage($"[YELLOW]{entityDetails.Name}[RESET][CR]");
                if (!string.IsNullOrWhiteSpace(entityDetails.Description))
                {
                    AddMessage($"{entityDetails.Description}[CR]");
                }
                var exits = GetRoomExits(entityId);
                var obviousExits = exits.Where(e => e.RoomExitVisibility == RoomExitVisibility.Obvious)
                    .Select(e => e.PrimaryAlias!).ToArray();
                switch (obviousExits.Length)
                {
                    case 0:
                        AddMessage("[GREEN]There are no obvious exits.[RESET][CR]");
                        break;
                    case 1:
                        AddMessage($"[GREEN]There is one obvious exit: {FormatArray(obviousExits)}[RESET][CR]");
                        break;
                    default:
                        AddMessage(
                            $"[GREEN]There are {obviousExits.Length} obvious exits: {FormatArray(obviousExits)}[RESET][CR]");
                        break;
                }
                var living = entityDetails.Entities.Where(e => e.IsLiving && e.EntityId != thisPlayer.EntityId)
                    .Select(e => "[RED][BOLD]" + e.Name! + "[RESET]").ToArray();
                if (living.Length > 0)
                {
                    AddMessage($"{FormatArray(living)}[CR]");
                }
                var objectsOnFloor = entityDetails.Entities.Where(e => !e.IsLiving).Select(e => e.Name!).ToArray();
                if (objectsOnFloor.Length > 0)
                {
                    AddMessage($"[MAGENTA]{FormatArray(objectsOnFloor)}[RESET][CR]");
                }
                break;
            case EntityType.Mobile:
            case EntityType.Player:
            case EntityType.Object:
            case EntityType.Mud:
            case EntityType.Zone:
            case EntityType.Weapon:
            case EntityType.Clothing:
            case EntityType.Container:
            default:
                AddMessage($"{entityDetails.Name}[RESET][CR]");
                if (!string.IsNullOrWhiteSpace(entityDetails.Description))
                {
                    AddMessage($"{entityDetails.Description}[CR]");
                }
                break;
        }
        return Response;
    }
}