using System.Collections.Concurrent;
using EcommerceAPI.Hubs.IHubs;

namespace EcommerceAPI.Hubs
{
    public class Connections : IConnections
    {
        private readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        public void AddConnection(string id, string connectionId)
        {
            _connections.TryAdd(id, connectionId);
        }

        public void RemoveConnection(string id, string connectionId)
        {
            _connections.TryRemove(id, out string removedConnectionId);
        }

        public string GetConnectionId(string id)
        {
            return _connections.TryGetValue(id, out string connectionId) ? connectionId : null;
        }
    }
}
