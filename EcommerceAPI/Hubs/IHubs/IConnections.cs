namespace EcommerceAPI.Hubs.IHubs
{
    public interface IConnections
    {
        void AddConnection(string id, string connectionId);
        void RemoveConnection(string id, string connectionId);
        string GetConnectionId(string id);
    }
}
