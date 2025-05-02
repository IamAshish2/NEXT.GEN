using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace YourNamespace.Controllers
{
    public class BaseController : ControllerBase
    {
        // Server error response
        protected IActionResult ServerErrorResponse(Exception ex)
        {
            var response = new
            {
                status = StatusCodes.Status500InternalServerError,
                message = ex.Message
            };
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        // Success response for boolean values
        protected IActionResult SuccessMsgBooleanResponse(string resType, bool isOk)
        {
            var response = new
            {
                status = isOk ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                message = isOk ? $"{resType} Success" : $"{resType} Failed"
            };
            return StatusCode(isOk ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, response);
        }

        // List response
        protected IActionResult ListResponse<T>(List<T> list)
        {
            var isEmpty = list == null || !list.Any();
            var response = new
            {
                status = isEmpty ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK,
                data = isEmpty ? Enumerable.Empty<T>() : list
            };
            return StatusCode(StatusCodes.Status200OK, response);
        }

        // Page response (assuming you have a PagedResult<T> or similar class)
        protected IActionResult PageResponse<T>(PagedResult<T> page)
        {
            var isEmpty = page == null || !page.Items.Any();
            var response = new
            {
                status = isEmpty ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK,
                data = page
            };
            return StatusCode(StatusCodes.Status200OK, response);
        }

        // Object response
        protected IActionResult ObjectResponse<T>(T obj) where T : class
        {
            var isNull = obj == null;
            var response = new
            {
                status = isNull ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK,
                message = isNull ? "Object not found" : "Success",
                data = obj
            };
            return StatusCode(isNull ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK, response);
        }

        // For created Object response
        protected IActionResult CreatedObjectResponse<T>(T obj) where T : class
        {
            var isNull = obj == null;
            var response = new
            {
                status = isNull ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK,
                message = isNull ? "Creation Failed" : "Created Successfully",
                data = obj
            };
            return StatusCode(isNull ? StatusCodes.Status400BadRequest : StatusCodes.Status201Created, response);
        }

        // For update Object response
        protected IActionResult UpdatedObjectResponse<T>(T obj) where T : class
        {
            var isNull = obj == null;
            var response = new
            {
                status = isNull ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK,
                message = isNull ? "Update Failed" : "Updated Successfully",
                data = obj
            };
            return StatusCode(isNull ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK, response);
        }

        protected IActionResult CustomObjectResponse<T>(T obj, string message)
        {
            var isNull = obj == null;
            var response = new
            {
                status = isNull ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK,
                message = message,
                data = obj
            };
            return StatusCode(isNull ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK, response);
        }

        protected IActionResult ErrorMessage(int httpStatusCode, string message)
        {
            var response = new
            {
                status = httpStatusCode,
                message = message
            };
            return StatusCode(httpStatusCode, response);
        }

        protected IActionResult SuccessMessage(int httpStatusCode, string message)
        {
            var response = new
            {
                status = httpStatusCode,
                message = message
            };
            return StatusCode(httpStatusCode, response);
        }

        // Duplicate name response
        protected IActionResult DuplicateNameResponse(string message)
        {
            var response = new
            {
                status = 450, // Custom status for duplicate error
                message = $"{message} already exists"
            };
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }

    // Example of a PagedResult class (you might have your own implementation)
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}