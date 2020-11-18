using System;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Enforcer;
using Rsk.Enforcer.PAP;
using Rsk.Enforcer.PDP;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;
using Rsk.Enforcer.Services.Logging;
using Xunit;

namespace PolicyTestingSample
{
    public class DoorAccessPolicyPolicyTests
    {
        private readonly ServiceProvider serviceProvider;
        private readonly TestingLogger logger;
        private readonly AuditObligationHandler obligationHandler;

        
        private const string Licensee = "DEMO";
        private const string LicenseKey =
            "Obtain a demo license key at https://www.identityserver.com/products/enforcer";
        private const string RootPolicyName = "AcmeCorp.DoorPolicy.global";
        private const string EmbeddedPolicyRoot = "PolicyTestingSample.Policies";
        
        public DoorAccessPolicyPolicyTests()
        {
            var obligationHandlerFactory = new TestObligationHandlerFactory();
            obligationHandler = (AuditObligationHandler) obligationHandlerFactory.Create();
            
            var serviceCollection = new ServiceCollection();

            logger = new TestingLogger();
            
            serviceCollection
                .AddLogging()
                .AddEnforcer(
                    RootPolicyName, o =>
                    {
                        o.Licensee = Licensee;
                        o.LicenseKey = LicenseKey;
                        o.PolicyInformationPointFailureBehavior = PolicyInformationPointFailureBehavior.Continue;
                        o.OmitStandardPIPs = true;
                    })
                .AddLoggerFactory(logger)
                .AddPolicyEnforcementPoint(o => o.Bias = PepBias.Deny)
                .AddEmbeddedPolicyStore(this.GetType().Assembly, EmbeddedPolicyRoot)
                .AddOutcomeActionHandlerFactory(obligationHandlerFactory);

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private IPolicyEnforcementPoint CreateSystemUnderTest()
        {
            return serviceProvider.GetService<IPolicyEnforcementPoint>();
        }

        [Fact]
        public async Task DoorAccessPolicy_ShouldCompileWithoutErrors()
        {
            try
            {
                var sut = CreateSystemUnderTest();

                var outcome = await sut.Evaluate();
            }
            catch (PolicyCompilationException e)
            {
                var errors = new StringBuilder();

                foreach (PolicyCompilationIssue issue in e.Errors)
                {
                    errors.AppendLine($"Compiler Error at ({issue.Line}:{issue.Column}): {issue.Message}");
                }
                
                Assert.True(false, errors.ToString());
            }
        }

        [Fact]
        public async Task Employee_OpeningMainDoorDuringOfficeHours_ShouldBePermitted()
        {
            string subjectId = "alice";
            string role = "employee";
            string resourceType = "door";
            string resourceAction = "open";
            string resourceName = "mainDoor";
            
            Time timeOfDay = new Time(10, 00, 00);
            DateTime timeNow = DateTime.Now;

            var request = new DynamicAttributeValueProvider();

            request
                .AddString(Rsk.Enforcer.Oasis.Attributes.Subject.Role, role)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Subject.Identifier, subjectId)
                .AddString(Rsk.Enforcer.Oasis.Attributes.ResourceType, resourceType)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Action, resourceAction)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Resource, resourceName)
                .AddTime(Rsk.Enforcer.Oasis.Attributes.CurrentTime, timeOfDay)
                .AddDateTime(Rsk.Enforcer.Oasis.Attributes.CurrentDateTime, timeNow);
            
            var sut = CreateSystemUnderTest();
            
            var outcome = await sut.Evaluate(request);
            
            outcome.Outcome.Should().Be(PolicyOutcome.Permit);
        }

        [Fact]
        public async Task Employee_OpeningMainDoorOutsideOfOfficeHours_ShouldBeDenied()
        {
            string subjectId = "alice";
            string role = "employee";
            string resourceType = "door";
            string resourceAction = "open";
            string resourceName = "mainDoor";
            Time timeOfDay = new Time(20, 00, 00);
            DateTime timeNow = DateTime.Now;

            var request = new DynamicAttributeValueProvider();
                
            request
                .AddString(Rsk.Enforcer.Oasis.Attributes.Subject.Role, role)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Subject.Identifier, subjectId)
                .AddString(Rsk.Enforcer.Oasis.Attributes.ResourceType, resourceType)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Action, resourceAction)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Resource, resourceName)
                .AddTime(Rsk.Enforcer.Oasis.Attributes.CurrentTime, timeOfDay)
                .AddDateTime(Rsk.Enforcer.Oasis.Attributes.CurrentDateTime, timeNow);
                
            var sut = CreateSystemUnderTest();

            var outcome = await sut.Evaluate(request);

            outcome.Outcome.Should().Be(PolicyOutcome.Deny);
        }
        
        [Fact]
        public async Task Employee_OpeningMainDoor_ShouldPermitAndCaptureAuditTrail()
        {
            string subjectId = "alice";
            string role = "employee";
            string resourceType = "door";
            string resourceAction = "open";
            string resourceName = "mainDoor";
            Time timeOfDay = new Time(15, 00, 00);
            DateTime timeNow = DateTime.Now;

            var request = new DynamicAttributeValueProvider();
                
            request
                .AddString(Rsk.Enforcer.Oasis.Attributes.Subject.Role, role)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Subject.Identifier, subjectId)
                .AddString(Rsk.Enforcer.Oasis.Attributes.ResourceType, resourceType)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Action, resourceAction)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Resource, resourceName)
                .AddTime(Rsk.Enforcer.Oasis.Attributes.CurrentTime, timeOfDay)
                .AddDateTime(Rsk.Enforcer.Oasis.Attributes.CurrentDateTime, timeNow);
            
            var sut = CreateSystemUnderTest();

            var outcome = await sut.Evaluate(request);
            
            outcome.Outcome.Should().Be(PolicyOutcome.Permit);

            var auditLogs = obligationHandler.Invocations.ToArray();

            auditLogs.Length.Should().Be(1);
            auditLogs[0].Subject.Should().Be(subjectId);
            auditLogs[0].When.Should().Be(timeNow);
        }
        
        [Fact]
        public async Task Employee_OpeningServerRoomDoor_ShouldBeDenied()
        {
            string subjectId = "alice";
            string role = "employee";
            string resourceType = "door";
            string resourceAction = "open";
            string resourceName = "serverRoomDoor";
            Time timeOfDay = new Time(20, 00, 00);
            DateTime timeNow = DateTime.Now;

            var request = new DynamicAttributeValueProvider();
                
            request
                .AddString(Rsk.Enforcer.Oasis.Attributes.Subject.Role, role)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Subject.Identifier, subjectId)
                .AddString(Rsk.Enforcer.Oasis.Attributes.ResourceType, resourceType)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Action, resourceAction)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Resource, resourceName)
                .AddTime(Rsk.Enforcer.Oasis.Attributes.CurrentTime, timeOfDay)
                .AddDateTime(Rsk.Enforcer.Oasis.Attributes.CurrentDateTime, timeNow);
            
            var sut = CreateSystemUnderTest();

            var outcome = await sut.Evaluate(request);

            outcome.Outcome.Should().Be(PolicyOutcome.Deny);
        }
        
        [Fact]
        public async Task ITAdministrator_OpeningServerRoomDoor_ShouldBePermitted()
        {
            string subjectId = "bob";
            string[] roles = new[] {"employee", "ITAdmin"};
            string resourceType = "door";
            string resourceAction = "open";
            string resourceName = "serverRoomDoor";
            Time timeOfDay = new Time(20, 00, 00);
            DateTime timeNow = DateTime.Now;

            var request = new DynamicAttributeValueProvider();
                
            request
                .AddString(Rsk.Enforcer.Oasis.Attributes.Subject.Role, roles)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Subject.Identifier, subjectId)
                .AddString(Rsk.Enforcer.Oasis.Attributes.ResourceType, resourceType)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Action, resourceAction)
                .AddString(Rsk.Enforcer.Oasis.Attributes.Resource, resourceName)
                .AddTime(Rsk.Enforcer.Oasis.Attributes.CurrentTime, timeOfDay)
                .AddDateTime(Rsk.Enforcer.Oasis.Attributes.CurrentDateTime, timeNow);
            
            var sut = CreateSystemUnderTest();

            var outcome = await sut.Evaluate(request);

            outcome.Outcome.Should().Be(PolicyOutcome.Permit);
        }
    }
}