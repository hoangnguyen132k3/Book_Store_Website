using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace PresentationLayer.Exceptions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureBuildInExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var contextRequest = context.Features.Get<IHttpRequestFeature>();

                    if (contextFeature != null)
                    {
                        var exception = contextFeature.Error;

                        int statusCode;
                        string message;

                        switch (exception)
                        {
                            case ExceptionNotFound: 
                                statusCode = (int)HttpStatusCode.NotFound;
                                message = exception.Message;
                                break;

                            case UnauthorizedAccessException:
                                statusCode = (int)HttpStatusCode.Unauthorized;
                                message = "Truy cập trái phép.";
                                break;

                            case ArgumentException:
                            case ExceptionBusinessLogic:
                            case ExceptionForeignKeyViolation: 
                                statusCode = (int)HttpStatusCode.BadRequest;
                                message = exception.Message; 
                                break;

                            case InvalidOperationException:
                                statusCode = (int)HttpStatusCode.BadRequest;
                                message = exception.Message;
                                break;

                            case SqlException:
                                statusCode = (int)HttpStatusCode.InternalServerError;
                                message = "Đã xảy ra lỗi cơ sở dữ liệu. Vui lòng thử lại sau.";
                                break;

                            case TimeoutException:
                                statusCode = (int)HttpStatusCode.RequestTimeout;
                                message = "Yêu cầu đã hết thời gian chờ. Vui lòng thử lại sau.";
                                break;

                            case ValidationException:
                                statusCode = (int)HttpStatusCode.BadRequest;
                                message = "Xác thực không thành công đối với dữ liệu được cung cấp.";
                                break;

                            default:
                                statusCode = (int)HttpStatusCode.InternalServerError;
                                message = $"Một loại lỗi không mong muốn '{exception.GetType().Name}' đã xảy ra. Chi tiết: {exception.Message}";
                                break;
                        }


                        context.Response.StatusCode = statusCode;

                        var errorResponse = new ErrorVm
                        {
                            StatusCode = statusCode,
                            Message = message,
                            Path = contextRequest?.Path
                        };

                        await context.Response.WriteAsync(errorResponse.ToString());
                    }
                });
            });
        }
    }
}
