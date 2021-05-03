﻿using Microsoft.AspNetCore.Http;
using System;
//using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace Core.Extentios
{
    partial class Class1
    {
        public class ExceptionMiddleware
        {
            private RequestDelegate _next;

            public ExceptionMiddleware(RequestDelegate next)
            {
                _next = next;
            }
            //Tum methodlardan bu try catch yapısını geicirir hata varsa handle ı calıştırı.
            public async Task InvokeAsync(HttpContext httpContext)
            {
                try
                {
                    await _next(httpContext);
                }
                catch (Exception e)
                {
                    await HandleExceptionAsync(httpContext, e);
                }
            }
            //Hangle ise hataya gore message ve gorunusu verir.
            private Task HandleExceptionAsync(HttpContext httpContext, Exception e)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                string message = "Internal Server Error";
                IEnumerable<ValidationFailure> errors;
                if (e.GetType() == typeof(ValidationException))
                {
                    message = e.Message;
                    errors = ((ValidationException)e).Errors;
                    httpContext.Response.StatusCode = 400;

                    return httpContext.Response.WriteAsync(new ValidationErrorDetails 
                    {
                        StatusCode = 400,
                        Message = message,
                        Errors = errors
                    }.ToString());
                }

                //sistem hata verirse bunu dondur dedik.
                return httpContext.Response.WriteAsync(new ErrorDetails
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = message
                }.ToString());
            }
        }
    }
}