// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.Services.Models;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Controllers;
using Microsoft.Azure.IoTSolutions.ProjectNameHere.WebService.v1.Models;
using Moq;
using Newtonsoft.Json.Linq;
using WebService.Test.helpers;
using Xunit;
using Xunit.Abstractions;

namespace WebService.Test.Controllers
{
    public class DevicesControllerTest
    {
        /// <summary>The test logger</summary>
        private readonly ITestOutputHelper log;

        private readonly Mock<IDevices> devices;
        private readonly DevicesController target;

        private const string DateFormat = "yyyy-MM-dd'T'HH:mm:sszzz";

        // Execute this code before every test
        // Note: for complex setups, where many dependencies need to be
        // prepared before a test, and this method grows too big:
        // 1. First try to reduce the complexity of the class under test
        // 2. If #1 is not possible, use a context object, e.g.
        //      see https://dzone.com/articles/introducing-unit-testing
        public DevicesControllerTest(ITestOutputHelper log)
        {
            // This is a logger, in case we want to output some text during the test
            this.log = log;

            // This is a dependency of the controller, that we mock, so that
            // we can test the class in isolation
            // Moq Quickstart: https://github.com/Moq/moq4/wiki/Quickstart
            this.devices = new Mock<IDevices>();

            // By convention we call "target" the class under test
            this.target = new DevicesController(this.devices.Object);
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public void ItFetchesADeviceFromTheServiceLayer()
        {
            // Test syntax: "AAA" see https://msdn.microsoft.com/en-us/library/hh694602.aspx
            // *Arrange*: setup the current context, in preparation for the test
            // *Act*:     execute an action in the system under test (SUT)
            // *Assert*:  verify that the test succeeded

            // Arrange
            // Note: for complex setups, where many dependencies need to be
            // prepared before a test:
            // 1. First try to reduce the complexity of the class under test
            // 2. If #1 is not possible, use a context object, e.g.
            //      see https://dzone.com/articles/introducing-unit-testing
            // Prepare a fake response to be returned by Devices.GetAsync()
            var etag = "foo";
            var id = "123";
            var date1 = DateTimeOffset.UtcNow;
            var date2 = DateTimeOffset.MinValue;
            var device = new Device(
                eTag: etag,
                id: id,
                c2DMessageCount: 345,
                lastActivity: date1,
                connected: true,
                enabled: true,
                lastStatusUpdated: date2,
                twin: new DeviceTwin(
                    eTag: etag,
                    deviceId: id,
                    desiredProperties: new Dictionary<string, JToken>(),
                    reportedProperties: new Dictionary<string, JToken>(),
                    tags: new Dictionary<string, JToken>(),
                    isSimulated: false));

            // Inject a fake response when Devices.GetAsync() is invoked
            // Moq Quickstart: https://github.com/Moq/moq4/wiki/Quickstart
            this.devices.Setup(x => x.GetAsync(id)).ReturnsAsync(device);

            // Act
            // Note: don't use "var" so to implicitly assert that the
            // method is returning an object of the expected type. We test only
            // public methods, i.e. to test code inside private methods, we
            // write a test that starts from a public method.
            DeviceApiModel result = this.target.GetAsync(id).Result;

            // Assert
            // Verify that the result looks like it should be, e.g. values and
            // format are correct.
            Assert.Equal(etag, result.Etag);
            Assert.Equal(id, result.Id);
            Assert.Equal(345, result.C2DMessageCount);
            Assert.Equal(device.LastActivity.ToString(DateFormat), result.LastActivity);
            Assert.Equal(device.LastStatusUpdated.ToString(DateFormat), result.LastStatusUpdated);
            Assert.Equal(device.Enabled, result.IsEnabled);
            Assert.Equal(device.Connected, result.IsConnected);
            Assert.Equal(etag, result.Twin.ETag);
            Assert.Equal(id, result.Twin.DeviceId);

            // Verify that Devices.GetAsync() has been called, exactly once
            // with the correct parameters
            this.devices.Verify(x => x.GetAsync(It.Is<string>(s => s == id)), Times.Once);
        }

        [Fact, Trait(Constants.Type, Constants.UnitTest)]
        public void TestTemplate()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}
