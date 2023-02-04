using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Hubs.IHubs
{
    public interface IStockClient
    {
        Task SendAsync(int stock);
    }

}
