using EcommerceAPI.Hubs.IHubs;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class ChatHub : Hub
{
    private readonly IConnections _connections;
    private readonly IChatHubRepository _repository;

    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IConnections connections, IChatHubRepository repository, ILogger<ChatHub> logger)
    {
        _connections = connections;
        _repository = repository;
        _logger = logger;
    }

    public async Task SendMessageToAdmin(ClaimsPrincipal user, string message)
    {
        if (user.IsAdmin())
        {
            var adminConnectionId = _connections.GetConnectionId(ClaimsPrincipalExtensions.AdminRole);
            if (adminConnectionId != null)
            {
                await Clients.Client(adminConnectionId).SendAsync("ReceiveMessage", user, message);
                try
                {
                    await _repository.SaveMessage(user.Identity.Name, ClaimsPrincipalExtensions.AdminRole, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while saving the message to the database");
                }
            }
        }
    }

    public async Task SendMessageToUser(ClaimsPrincipal admin, string message)
    {
        if (!admin.IsAdmin())
        {
            return;
        }

        var userConnectionId = _connections.GetConnectionId("user");
        if (userConnectionId != null)
        {
            await Clients.Client(userConnectionId).SendAsync("ReceiveMessage", admin, message);
            await _repository.SaveMessage(ClaimsPrincipalExtensions.AdminRole, "user", message);
        }
    }
       public void AddConnection(string userId, string connectionId)
    {
        _connections.AddConnection(userId, connectionId);
    }

    public void RemoveConnection(string userId, string connectionId)
    {
        _connections.RemoveConnection(userId, connectionId);
    }
}

