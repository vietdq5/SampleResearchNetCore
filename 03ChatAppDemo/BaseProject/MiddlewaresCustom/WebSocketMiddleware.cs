using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace BaseProject.MiddlewaresCustom;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new ConcurrentDictionary<Guid, WebSocket>();

    public WebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            Guid clientId = Guid.NewGuid();

            _sockets.TryAdd(clientId, webSocket);
            try
            {
                await HandleWebSocketAsync(webSocket, clientId);
            }
            catch (WebSocketException)
            {
                _sockets.TryRemove(clientId, out _);
            }
        }
        else
        {
            await _next(context);
        }
    }

    private async Task HandleWebSocketAsync(WebSocket webSocket, Guid clientId)
    {
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await BroadcastMessageAsync(message);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                _sockets.TryRemove(clientId, out _);
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
        }
    }

    private async Task BroadcastMessageAsync(string message)
    {
        Console.WriteLine(message);
        foreach (var socket in _sockets)
        {
            if (socket.Value.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await socket.Value.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}