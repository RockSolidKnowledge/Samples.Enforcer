using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PolicyModels;
using Rsk.Enforcer.Services.Logging;

namespace PolicyTestingSample
{
    internal class AuditObligationHandler : OutcomeActionHandler<AuditLogArguments>
    {
        public List<AuditLogArguments> Invocations { get; } = new List<AuditLogArguments>();

        public override string Name => "Audit.Log";
        
        public AuditObligationHandler()
        {
            Invocations?.Clear();
        }

        protected override Task Execute(AuditLogArguments parameters, IEnforcerLogger evaluationLogger)
        {
            Invocations.Add(parameters);
            
            return Task.CompletedTask;
        }
    }
    
    public class AuditLogArguments
    {
        [PolicyAttribute(PolicyAttributeCategories.Subject, "audit.subject")]
        public string Subject { get; set; }
        
        [PolicyAttribute(PolicyAttributeCategories.Subject, "audit.message")]
        public string Message { get; set; }
        
        [PolicyAttribute(PolicyAttributeCategories.Subject, "audit.when")]
        public DateTime? When { get; set; }
    }
}