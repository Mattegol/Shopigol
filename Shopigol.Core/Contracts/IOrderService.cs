using Shopigol.Core.Models;
using Shopigol.Core.ViewModels;
using System.Collections.Generic;

namespace Shopigol.Core.Contracts
{
    public interface IOrderService
    {
        void CreateOrder(Order order, List<BasketItemViewModel> basketItems);
    }
}
