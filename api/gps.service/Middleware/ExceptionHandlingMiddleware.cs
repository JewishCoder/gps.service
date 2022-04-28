using System;
using System.Net;
using System.Text.Json;

using gps.common.Exceptions;

using Serilog;

namespace gps.service.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly Serilog.ILogger _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next, Serilog.ILogger logger)
		{
			_next = next;
			_logger = logger ?? Log.Logger;
		}

		public async Task InvokeAsync(HttpContext context) 
		{
			try
			{
				await _next(context);
			}
			catch(Exception exc) 
			{
				switch(exc) 
				{
					case AuthException authException:
						_logger.Warning(authException, authException.Message);
						await WriteErrorAsync(new { message = authException.Message }, HttpStatusCode.Unauthorized, context.Response);
						break;
					case PasswordInvalidException passwordException:
						_logger.Warning(passwordException, passwordException.Message);
						await WriteErrorAsync(new { message = "Не корректный пароль" }, HttpStatusCode.Unauthorized, context.Response);
						break;
					case NotFoundException notFound:
						_logger.Warning(notFound, $"{{entity}}={{id}} {notFound.Message}", notFound.Id ?? 0, notFound.Entity ?? string.Empty);
						context.Response.StatusCode = (int)HttpStatusCode.NotFound;
						await WriteErrorAsync(new { message = notFound.Message }, HttpStatusCode.NotFound, context.Response);
						break;
					case BadRequestException dublicate:
						_logger.Warning(dublicate, dublicate.Message);
						await WriteErrorAsync(new { message = dublicate.Message }, HttpStatusCode.BadRequest, context.Response);
						break;
					case ForbiddenException noAccess:
						_logger.Debug(noAccess, noAccess.Message);
						context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
						break;
					case UpdateConcurrencyException concurrency:
						_logger.Warning(concurrency, "concurrency error");
						await WriteErrorAsync(new { message = concurrency.Message }, HttpStatusCode.Conflict, context.Response);
						break;
					case UnauthorizedAccessException unauthorized:
						_logger.Warning(unauthorized, "Unauthorized");
						context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
						break;
					default:
						_logger.Error(exc, $"Internal server error");
						await WriteErrorAsync(new
						{
							message = exc.Message,
							exception = exc.ToString(),
						}, 
						HttpStatusCode.InternalServerError, 
						context.Response);
						break;
				}
			}
		}

		private Task WriteErrorAsync<T>(T data, HttpStatusCode code, HttpResponse response) 
		{
			response.StatusCode = (int)code;
			response.ContentType = "application/json; charset=utf-8";
			var json = JsonSerializer.Serialize(data);
			return response.WriteAsync(json);
		}
	}
}
