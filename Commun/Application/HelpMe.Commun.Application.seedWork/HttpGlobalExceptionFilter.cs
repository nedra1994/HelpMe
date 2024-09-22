﻿namespace Netcom.Commun.Application.seedWork.Filters
{

    
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Netcom.commun.domain.Data.DomainException;
    using Newtonsoft.Json;
    using System.Net;
    using System.Text;

    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public  void OnException(ExceptionContext context)
        {
            logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

           // if (context.Exception.GetType() == typeof(DomainException))
            {
                var problemDetails = new ValidationProblemDetails()
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                };

                problemDetails.Errors.Add("DomainValidations", new string[] { context.Exception.Message.ToString() });

                context.Result = new BadRequestObjectResult(problemDetails);
              //  context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
               // string jsonString = JsonConvert.SerializeObject(problemDetails);
               // await context.HttpContext.Response.WriteAsync(jsonString, Encoding.UTF8);
            }
            //else
            //{
            //    var json = new JsonErrorResponse
            //    {
            //        Messages = new[] { "An error occur.Try it again." }
            //    };

            //    if (env.IsDevelopment())
            //    {
            //        json.DeveloperMessage = context.Exception;
            //    }

            //    // Result asigned to a result object but in destiny the response is empty. This is a known bug of .net core 1.1
            //    // It will be fixed in .net core 1.1.2. See https://github.com/aspnet/Mvc/issues/5594 for more information
            //    context.Result = new InternalServerErrorObjectResult(json);
            //    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //    string jsonString = JsonConvert.SerializeObject(json);
            //    await context.HttpContext.Response.WriteAsync(jsonString, Encoding.UTF8);
            //}
            context.ExceptionHandled = true;
        }

        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }

            public object DeveloperMessage { get; set; }
        }
    }

      public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error)
            : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
