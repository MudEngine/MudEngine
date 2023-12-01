using MudEngine.Library.System;
namespace MudEngine.Library.Commands.NewConnection;

[Command("93c8ce1f-60bb-4a1c-9d38-fc8abea4756b")]
public class Login : BaseCommand, ICommand
{
    public override CommandResponse Execute(CommandRequest Request)
    {
        base.Execute(Request);
        if (!int.TryParse(ConnectionVariables("LoginStatus"), out var loginStatus))
        {
            loginStatus = 0;
        }
        if (!int.TryParse(ConnectionVariables("LoginAttempts"), out var loginAttempts))
        {
            loginAttempts = 0;
        }

        switch (loginStatus)
        {
            case 1: // Enter Account Name
                var accountName = Request.CommandLine?.Replace(" ", string.Empty) ?? string.Empty;
                if (accountName.Length is > 2 and < 79)
                {
                    SetConnectionVariable("LoginAccountSelection", accountName);
                    loginStatus = 2;
                    AddMessage("Password: [CR]");
                }
                else
                {
                    loginAttempts += 1;
                    AddMessage("Login failed.[CR]Account: ");
                }
                break;
            case 2: // Password
                var loginAccountSelection = ConnectionVariables("LoginAccountSelection");
                var accountId = ValidateAccountPassword(loginAccountSelection, Request.CommandLine!);
                if (accountId == Guid.Empty)
                {
                    loginAttempts += 1;
                    loginStatus = 1;
                    AddMessage("Login failed.[CR]Account: [CR]");
                }
                else
                {
                    var players = SetConnectionAccount(accountId).ToList();
                    AddMessage("[YELLOW][BOLD]Login successful[RESET][CR]");
                    loginStatus = 3;
                    if (players.Count != 1)
                    {
                    }
                    else
                    {
                        SetConnectionPlayer(players.First().EntityId);
                        RemoveCommandList("New Connections");
                        AddCommandList("Basic");
                        AddFollowOnCommand("Basic", "motd", "motd");
                        AddFollowOnCommand("Basic", "look", "look here");
                    }
                }
                break;
            case 3: // Debug
                AddMessage($"[YELLOW][BOLD]Debug:[RESET] [{ConnectionVariables("LoginAccountSelection")}][CR]");
                break;
            default:
                AddMessage("[CR]Account:[CR]");
                loginStatus = 1;
                break;
        }
        if (loginAttempts > 2)
        {
            AddSystemMessage("Disconnect");
        }

        SetConnectionVariable("LoginStatus", loginStatus.ToString());
        SetConnectionVariable("LoginAttempts", loginAttempts.ToString());

        return Response;
    }
}