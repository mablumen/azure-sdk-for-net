﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.Amqp;

namespace TrackOne.Amqp
{
    internal sealed class ActiveClientLink : ActiveClientLinkObject
    {
        private readonly AmqpLink link;

        public ActiveClientLink(AmqpLink link, string audience, string endpointUri, string[] requiredClaims, bool isClientToken, DateTime authorizationValidToUtc)
            : base(link, audience, endpointUri, requiredClaims, isClientToken, authorizationValidToUtc)
        {
            this.link = link;
        }

        public AmqpLink Link => link;

        public override AmqpConnection Connection => link.Session.Connection;
    }
}
