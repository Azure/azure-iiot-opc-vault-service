﻿// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Test.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.Test.v1.Controllers
{
    public class SomeControllerTest
    {
        /// <summary>The test logger</summary>
        private readonly ITestOutputHelper log;

        public SomeControllerTest(ITestOutputHelper log)
        {
            this.log = log;
        }

        [Fact, Trait(Constants.Type, Constants.ControllerTest)]
        public void SomeTestShowingHowToLog()
        {
            this.log.WriteLine("Some log from the test");
            Assert.True(true);
        }

        [Fact(Skip = "work in progress"), Trait(Constants.Type, Constants.UnitTest)]
        public void IncompleteTestNotReadyToRun()
        {
            Assert.True(false);
        }
    }
}
