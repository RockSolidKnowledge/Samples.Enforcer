using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyServer.Client;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;
using SecureMVCApp.Models;
using SecureMVCApp.Services;

namespace SecureMVCApp.Controllers
{
    [Authorize]
    // TODO: PreferredHandlerType can't be set as its nullable
    [EnforcerAuthorization(ResourceType="PurchaseOrder" )]
    public class PurchaseOrdersController : Controller
    {
        private readonly IManagePurchaseOrders purchaseOrders;
        private readonly IPolicyEnforcementPoint pep;

        public PurchaseOrdersController(IManagePurchaseOrders purchaseOrders , IPolicyEnforcementPoint pep)
        {
            this.purchaseOrders = purchaseOrders;
            this.pep = pep;
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
            
            IAttributeValueProvider context = new EditPurchaseOrderAuthorizationContext("Edit")
            {
                PurchaseOrderDepartment = po.Department,
            };

            var authorizationResult = await pep.Evaluate(context);
            if (authorizationResult.Outcome != PolicyOutcome.Permit)
            {
                return AuthorizationFailed(authorizationResult);
            }
            
            return View("PurchaseOrder", po);
        }

        [HttpPost]
        [Route("/PurchaseOrders/{id}", Name = "Edit")]
        public async Task<IActionResult> Edit(int id, PurchaseOrderRequest request)
        {
            var po = purchaseOrders.FindById(id);

            IAttributeValueProvider context = new EditPurchaseOrderAuthorizationContext("Update")
            {
                PurchaseOrderAmount = request.Amount,
                PurchaseOrderDepartment = po.Department
            };

            var authorizationResult = await pep.Evaluate(context);
            if (authorizationResult.Outcome != PolicyOutcome.Permit)
            {
                return AuthorizationFailed(authorizationResult);
            }

            po.Description = request.Description;
            po.Amount = request.Amount.Value;

            return RedirectToRoute("Index");
        }

        private IActionResult AuthorizationFailed(PolicyEvaluationOutcome po)
        {
            
            // TODO: Failing to get the advice failure message
            // GetValue<string> is failing as the value is a JsonElement

            var view = View("NotAuthorized",
                po.UnresolvedAdvice.MapAuthorizationFailureAdviceToString());
            
            view.StatusCode = (int) HttpStatusCode.Forbidden;
            return view;
        }

    }
}
