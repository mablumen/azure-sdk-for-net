﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Azure.Core.Pipeline;
using Azure.Core.Testing;
using Azure.Storage.Blobs;
using Azure.Storage.Test;
using Azure.Storage.Test.Shared;

namespace Azure.Storage.Common.Test
{
    /// <summary>
    /// Base class for Common tests
    /// </summary>
    public class CommonTestBase : StorageTestBase
    {
        public CommonTestBase(bool async, RecordedTestMode? mode = null)
            : base(async, mode /* RecordedTestMode.Record to re-record */)
        {
        }

        public string GetNewContainerName() => $"test-container-{Recording.Random.NewGuid()}";

        /// <summary>
        /// Get BlobClientOptions instrumented for recording.
        /// </summary>
        protected BlobClientOptions GetBlobOptions()
        {
            var options = new BlobClientOptions
            {
                Diagnostics = { IsLoggingEnabled = true },
                Retry =
                {
                    Mode = RetryMode.Exponential,
                    MaxRetries = Azure.Storage.Constants.MaxReliabilityRetries,
                    Delay = TimeSpan.FromSeconds(Mode == RecordedTestMode.Playback? 0.01 : 0.5),
                    MaxDelay = TimeSpan.FromSeconds(Mode == RecordedTestMode.Playback ? 0.1 : 10)
                }
            };
            if (Mode != RecordedTestMode.Live)
            {
                options.AddPolicy(new RecordedClientRequestIdPolicy(Recording), HttpPipelinePosition.PerCall);
            }

            return Recording.InstrumentClientOptions(options);
        }

        public BlobServiceClient GetSecondaryStorageReadEnabledServiceClient(TenantConfiguration config, int numberOfReadFailuresToSimulate, bool simulate404 = false)
        {
            BlobClientOptions options = GetBlobOptions();
            options.GeoRedundantSecondaryUri = new Uri(config.BlobServiceSecondaryEndpoint);
            options.Retry.MaxRetries = 4;
            options.AddPolicy(new TestExceptionPolicy(numberOfReadFailuresToSimulate, options.GeoRedundantSecondaryUri, simulate404), HttpPipelinePosition.PerRetry);

            return InstrumentClient(
                 new BlobServiceClient(
                    new Uri(config.BlobServiceEndpoint),
                    new StorageSharedKeyCredential(config.AccountName, config.AccountKey),
                    options));
        }
    }
}
