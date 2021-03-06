﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Enforcer;
using Rsk.Enforcer.PEP;
using Rsk.Enforcer.PEP.OutcomeActionHandlers;
using Rsk.Enforcer.PolicyModels;
using Rsk.Enforcer.Services.DataMasking;

namespace Masking
{
    class Program
    {
         static async Task Main(string[] args)
        {
            // See identityserver.com/products/enforcer for a new license key
            string licenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjEtMDctMDlUMDE6MDA6MDUuOTg1MDM5KzAxOjAwIiwiaWF0IjoiMjAyMS0wNi0wOVQwMDowMDowNSIsIm9yZyI6IkRFTU8iLCJhdWQiOjd9.K6ojh1tCXt3AdROXQUWD3RRqrxfHdVd26YzNbWB6QOyvWJhOjmz4unzUmUUkei3IzEpcCcPN8k9nGMM1l0SW0+lz0gK/Y+EQMT5FPRFCyGsU5Gs4TFbS2umlAYWh2CDaqtUVS6iMiK5SzKbFPvd/GAbzmdYliaZtExbLveaEWLt23exxZn9nKppETR6UhHtqM6nBm/k5Ekc83i//MiA3XnMcu11hHb3XyOquVlX2rbr/cyLeajJBmCvUUqJYognHJ3gQYPZygQdw4ECCdpuSmTVAtyB+4W/GuICWFjq5AbbgbZOznNQMJ8ODiasXc1+OyjzzPkiX9oOFxhjAClaVVZU4fc3Yi7C1TfugJ+PzwLMm9Omzr0qRfs2lkNlQERIpEogfh26QhOlB6y4V98C0ADKiJHzW+heSnTKZEZzMvaMBiamgbxOwSuzBy+UOzEdnzCor+kzFXa3FMADuWX4u/zdpYWikq6UpzRP+QqapLFKI1IMUOuNAkLEhGcHMbvcp6yLfPfkts4iOucePSrhz/6SXWO+RGuhTXzp+6QCyHxB00+gDqm4HO/BVk8mvBTs2tmJAQDByEC5oXT+AIg7xBlLWPhob25zd8ucPmFemO11KrkvV6viWeHW0TfskH7910pT77BrpRFEg3a5XIDEiCXnm9rsnKnaPe82//ZaJOHU=";
            string rootPolicy = "Policies.TestPolicy";
            ServiceCollection containerBuilder = new ServiceCollection();

            containerBuilder.AddEnforcer(rootPolicy, options =>
                {
                    options.Licensee = "Demo";
                    options.LicenseKey = licenseKey;
                })
                .AddEmbeddedPolicyStore("Masking")
                .AddPolicyEnforcementPoint(options =>
                {
                    options.Bias = PepBias.Deny;
                });

            var container = containerBuilder.BuildServiceProvider();

            var pep = container.GetService<IPolicyEnforcementPoint>();

            var context = new EmptyContext();

            var masking = new MaskingOutcomeActionHandler();
            await pep.Evaluate(context, new[] {masking});

            var resultDataToMask = new Response()
            {
                Message = "Top secret",
                From = "Andy",
                To = "Sally"
            };

            masking.ApplyMask(resultDataToMask);

            Console.WriteLine(resultDataToMask);
            
        }
    }

    internal class Response
    {
        [ConstantValueMaskingCategory("secret",MaskedValue = "********")]
        public string Message { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public override string ToString()
        {
            return $"{nameof(Message)}: {Message}, {nameof(From)}: {From}, {nameof(To)}: {To}";
        }
    }
}