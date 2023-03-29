using System;
using System.Collections.Generic;
using System.Linq;
using Rsk.Enforcer.AspNetCore.PEP;
using Rsk.Enforcer.PEP;

namespace PolicyServer.Client;

public static class AuthorizationFailedExtensions
{
    public static List<string> MapAuthorizationFailureAdviceToString(this UnresolvedAdvice advice)
    {
        return advice
            .Where(a => a.Name == "AuthorizationFailure")
            .SelectMany(a => a.Arguments)
            .Where(a => a.Name == "AuthorizationFailureMessage")
            .Select(arg => String.Join(",", arg.GetValue<string>()))
            .DefaultIfEmpty("Not Authorized")
            .ToList();
    }
}
public class GeneralViewDenyHandler : EnforcerAuthorizationViewDenyHandler
{
    public override DenyViewResult GetDenyViewResult(PolicyEvaluationOutcome outcome)
    {
        // Advice and attribute values are available through the outcome argument

        return new DenyViewResult(
            "~/Views/Shared/NotAuthorized.cshtml",
            outcome.UnresolvedAdvice.MapAuthorizationFailureAdviceToString());
    }


    public GeneralViewDenyHandler(IDenyPageRenderer renderer) : base(renderer)
    {

    }
}

