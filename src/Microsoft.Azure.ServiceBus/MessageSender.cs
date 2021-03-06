﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.ServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class MessageSender : ClientEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1126:PrefixCallsCorrectly",
            Justification = "This is not a method call, but a type.")]
        protected MessageSender(TimeSpan operationTimeout)
            : base(nameof(MessageSender) + StringUtility.GetRandomString())
        {
            this.OperationTimeout = operationTimeout;
        }

        internal TimeSpan OperationTimeout { get; }

        protected MessagingEntityType EntityType { get; set; }

        public Task SendAsync(BrokeredMessage brokeredMessage)
        {
            return this.SendAsync(new BrokeredMessage[] { brokeredMessage });
        }

        public Task SendAsync(IEnumerable<BrokeredMessage> brokeredMessages)
        {
            MessageSender.ValidateMessages(brokeredMessages);
            return this.OnSendAsync(brokeredMessages);
        }

        protected abstract Task OnSendAsync(IEnumerable<BrokeredMessage> brokeredMessages);

        static void ValidateMessages(IEnumerable<BrokeredMessage> brokeredMessages)
        {
            if (brokeredMessages == null || !brokeredMessages.Any())
            {
                throw Fx.Exception.ArgumentNull(nameof(brokeredMessages));
            }

            if (brokeredMessages.Any(brokeredMessage => brokeredMessage.IsLockTokenSet))
            {
                throw Fx.Exception.Argument(nameof(brokeredMessages), "Cannot Send ReceivedMessages");
            }
        }
    }
}