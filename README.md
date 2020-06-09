# SignalR.StronglyTypedHubConnection
Allows you to use strongly typed hub connection expressions instead of relying on magic strings (ie: hubConnection.Invoke("myMethod"))

## Usage
Define an interface representing the publicly exposed methods of your hub definition.  ie:
```csharp
public interface IChatHub
{
  Task NotifyAllUsers(string from, object message);
  Task NotifyUser(string from, Guid userId, object message);
}
```
Implement interface in your hub definition.  ie:
```csharp
public class ChatHub : Hub, IChatHub
{
  public async Task NotifyAllUsers(string from, object message)
  {
    await Clients.All.SendAsync("ReceiveMessage", from, message);
  }
  
  public async Task NotifyUser(string from, Guid userId, object message)
  {
    await Clients.User(userId.ToString()).SendAsync("ReceiveMessage", from, message);
  }
}
```
Use the new extension method on your HubConnection instance and enjoy strongly typed calls!.  ie:
```csharp
using SignalR.StronglyTypedHubConnection;
...
var hubConnection = new HubConnectionBuilder()
                .WithUrl("https://myhub.mydomain.com")
                .WithAutomaticReconnect()
                .Build();

await hubConnection.StartAsync();
await hubConnection.InvokeAsync<IChatHub>(hub => hub.NotifyAllUsers("system@mydomain.com", message));
```
