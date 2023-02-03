namespace EcommerceAPI.Hubs.IHubs
{
    public interface IStockClient
    {
        Task SendAsync(int stock);
    }

}
