using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rsk.Enforcer.AspNetCore;
using SecureMVCApp.Models;
using SecureMVCApp.Services;

namespace SecureMVCApp.Controllers
{
    [Authorize]
    [EnforcerAuthorization(ResourceType="PurchaseOrder")]
    public class PurchaseOrdersController : Controller
    {
        private readonly IManagePurchaseOrders purchaseOrders;

        public PurchaseOrdersController(IManagePurchaseOrders purchaseOrders)
        {
            this.purchaseOrders = purchaseOrders;
        }

        [HttpGet]
        [Route("/PurchaseOrders", Name = "Index")]
        public IActionResult ListAllPurchaseOrders()
        {
            return View("PurchaseOrders", purchaseOrders.All);
        }

        [HttpPost]
        [Route("/PurchaseOrders")]
        [EnforcerAuthorization(ResourceType = "PurchaseOrder" , Action="Create")]
        public IActionResult CreatePurchaseOrder(PurchaseOrderRequest request)
        {
            string department = User.Claims.First(c => c.Type == "department").Value;

            purchaseOrders.Add(new PurchaseOrder(request.Amount.Value, request.Description, department));
            return View("PurchaseOrders", purchaseOrders.All);
        }

        [HttpGet]
        [Route("/PurchaseOrders/{id}", Name = "Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var po = purchaseOrders.FindById(id);

            return View("PurchaseOrder", po);
        }

        [HttpPost]
        [Route("/PurchaseOrders/{id}", Name = "Edit")]
        public async Task<IActionResult> Edit(int id, PurchaseOrderRequest request)
        {
            var po = purchaseOrders.FindById(id);

            po.Description = request.Description;
            po.Amount = request.Amount.Value;

            return RedirectToRoute("Index");
        }

    }
}
