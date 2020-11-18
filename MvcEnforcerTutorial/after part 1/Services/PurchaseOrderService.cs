using System.Collections.Generic;
using System.Linq;
using SecureMVCApp.Models;

namespace SecureMVCApp.Services
{
    public interface IManagePurchaseOrders
    {
        void Add(PurchaseOrder purchaseOrder);
        PurchaseOrder FindById(int id);
        
        IEnumerable<PurchaseOrder> All { get; }
        
    }
    public class PurchaseOrderService : IManagePurchaseOrders
    {
        private readonly List<PurchaseOrder> purchaseOrders = new List<PurchaseOrder>();
        
        public void Add(PurchaseOrder purchaseOrder)
        {
           purchaseOrders.Add(purchaseOrder);
        }

        public PurchaseOrder FindById(int id)
        {
            return purchaseOrders.First(po => po.Id == id);
        }

        public IEnumerable<PurchaseOrder> All => purchaseOrders;
    }
}