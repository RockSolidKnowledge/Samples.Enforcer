using Rsk.Enforcer.PEP;

namespace PolicyTestingSample
{
    public class TestObligationHandlerFactory : OutcomeActionHandlerFactory
    {
        private OutcomeActionHandler instance = new AuditObligationHandler();
        
        public override OutcomeActionHandler Create()
        {
            return instance;
        }

        public override string Name => "Audit.Log";
    }
}