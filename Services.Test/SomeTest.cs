// Copyright (c) Microsoft. All rights reserved.

using Services.Test.helpers;
using Xunit;
using Xunit.Abstractions;

namespace Services.Test
{
    public class SomeTest
    {
        /// <summary>The test logger</summary>
        private readonly ITestOutputHelper log;

        public SomeTest(ITestOutputHelper log)
        {
            this.log = log;
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public void Test1()
        {
            Assert.True(true);
        }
    }
}
