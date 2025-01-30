using Microsoft.AspNetCore.Mvc;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;
using StructuredAttributeTutorial.Models;
using StructuredAttributeTutorial.Services;

namespace StructuredAttributeTutorial.Controllers
{
    [ApiController]
    [Route("cms")]
    public class CmsController : ControllerBase
    {
        private readonly IDocumentStore documentStore;

        public CmsController(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        [HttpPost]
        [Route("create")]
        [EnforcerAuthorization(ResourceType = "document", Resource = "*", Action = "create")]
        public IActionResult Create([FromBody] CmsRequest request)
        {
            return Ok($"Created document {documentStore.CreateDocument()}");
        }

        [HttpPut]
        [Route("{documentId}/edit")]
        [EnforcerAuthorization(ResourceType = "document", Action = "edit")]
        public IActionResult Edit(
            [FromBody] CmsRequest request, 
            [FromRoute] 
            [PolicyAttributeValue(PolicyAttributeCategories.Resource, "Resource")]
            string documentId)
        {
            return Ok($"Editing document {documentId}");
        }

        [HttpPut]
        [Route("{documentId}/publish")]
        [EnforcerAuthorization(ResourceType = "document", Action = "publish")]
        public IActionResult Publish(
            [FromBody] CmsRequest request, 
            [FromRoute]
            [PolicyAttributeValue(PolicyAttributeCategories.Resource, "Resource")]
            string documentId)
        {
            return Ok($"Publishing document {documentId}");
        }
    }
}