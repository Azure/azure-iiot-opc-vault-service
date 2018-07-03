// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------


using System;

namespace Microsoft.Azure.IIoT.OpcUa.Services.Gds.Concurrency
{
    public class TimerNotInitializedException : Exception
    {
        public TimerNotInitializedException()
            : base("Timer object not initialized. Call 'Setup()' first.")
        {
        }
    }
}
