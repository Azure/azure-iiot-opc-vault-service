// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Services.GdsVault.App.Filters
{
    using Microsoft.AspNetCore.Authentication.AzureAD.UI;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Security;
    using System.Threading.Tasks;

    /// <summary>
    /// Detect all the unhandled exceptions returned by the API controllers
    /// and decorate the response accordingly, managing the HTTP status code
    /// and preparing a JSON response with useful error details.
    /// When including the stack trace, split the text in multiple lines
    /// for an easier parsing.
    /// @see https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters
    /// </summary>
    public class ExceptionsFilterAttribute : ExceptionFilterAttribute
    {

        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception == null)
            {
                base.OnException(context);
                return;
            }
            if (context.Exception is AggregateException ae)
            {
                var root = ae.GetBaseException();
                if (root is AggregateException)
                {
                    context.Exception = ae.InnerExceptions.First();
                }
                else
                {
                    context.Exception = root;
                }
            }
            switch (context.Exception)
            {
                case AdalException adal:
                case UnauthorizedAccessException ue:
                case SecurityException se:
                    // 
                    context.Result = ReAuthenticateUser(context.HttpContext);
                    break;
            }
        }

        /// <inheritdoc />
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            try
            {
                OnException(context);
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                return base.OnExceptionAsync(context);
            }
        }

        /// <summary>
        /// Create result
        /// </summary>
        /// <param name="code"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private ObjectResult GetResponse(HttpStatusCode code, Exception exception)
        {
            var result = new ObjectResult(exception)
            {
                StatusCode = (int)code
            };
            return result;
        }

        private IActionResult ReAuthenticateUser(HttpContext context)
        {
            return new ChallengeResult(AzureADDefaults.AuthenticationScheme);
        }

    }
}
