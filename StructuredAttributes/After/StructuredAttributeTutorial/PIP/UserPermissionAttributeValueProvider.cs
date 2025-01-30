using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;
using StructuredAttributeTutorial.Services;

namespace StructuredAttributeTutorial.PIP;

public class UserPermissionAttributeValueProvider : RecordAttributeValueProvider<UserPermissionRecord>
{
    private readonly Dictionary<string, List<Permission>> permissions = new()
    {
        {
            Alan, new List<Permission>()
        },
        {
            Bob, new List<Permission>{
                new()
                {
                    Resource = "*",
                    Action = CreatePermission
                }
            }
        }
    };
    
    private const string CreatePermission = "create";
    private const string EditPermission = "edit";
    private const string PublishPermission = "publish";
    
    private const string Alan = "Alan";
    private const string Bob = "Bob";
    
    private IDocumentStore documentStore;

    public UserPermissionAttributeValueProvider(IDocumentStore documentStore)
    {
        this.documentStore = documentStore;

        foreach (var id in this.documentStore.GetDocumentIds())
        {
            AddDocumentPermissions(id);
        }
    }
    
    private void AddDocumentPermissions(string newDocument)
    {
        // Grant users access to the document
        permissions[Alan].Add(new Permission(){ Resource = newDocument, Action = PublishPermission});
        permissions[Alan].Add(new Permission(){ Resource = newDocument, Action = EditPermission});
        
        permissions[Bob].Add(new Permission(){ Resource = newDocument, Action = EditPermission});
    }

    protected override async Task<UserPermissionRecord> GetRecordValue(IAttributeResolver attributeResolver, CancellationToken ct)
    {
        var userAttribute = await attributeResolver.Resolve<string>(
            new PolicyAttribute("request.user", PolicyValueType.String, PolicyAttributeCategories.Subject),
            ct);

        var user = userAttribute.SingleOrDefault();
        
        if (user == null || !permissions.TryGetValue(user, out var documentPermissions)) return null;           
            
        return new UserPermissionRecord
        {
            Permissions = documentPermissions
        };
    }
}