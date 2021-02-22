using BusinessLogic.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Exceptions;

namespace WebApi.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private ILogger<ExceptionFilter> _Logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _Logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            string actionName = context.ActionDescriptor.DisplayName;

            var contextResult = new ContentResult();
            contextResult.Content = context.Exception.Message;

            switch (context.Exception)
            {
                case AuthorizeException authorizeException:
                    contextResult.StatusCode = 401;
                    break;
                case ConflictException conflictException:
                    contextResult.StatusCode = 409;
                    break;
                case StateException stateException:
                    contextResult.StatusCode = 409;
                    break;
                default:
                    contextResult.StatusCode = 500;
                    break;
            }

            context.Result = contextResult;
            context.ExceptionHandled = contextResult.StatusCode < 500;
        }
    }
}
