﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Http;
using Azure.Core.Pipeline;
using NUnit.Framework;

namespace Azure.Core.Tests
{
    public abstract class PolicyTestBase
    {
        protected static async Task<Response> SendGetRequest(HttpPipelineTransport transport, HttpPipelinePolicy policy, ResponseClassifier responseClassifier = null)
        {
            Assert.IsInstanceOf<SynchronousHttpPipelinePolicy>(policy, "Use SyncAsyncPolicyTestBase base type for non-sync policies");

            using (Request request = transport.CreateRequest())
            {
                request.Method = RequestMethod.Get;
                request.Uri.Reset(new Uri("http://example.com"));
                var pipeline = new HttpPipeline(transport, new[] { policy }, responseClassifier);
                return await pipeline.SendRequestAsync(request, CancellationToken.None);
            }
        }
    }
}
