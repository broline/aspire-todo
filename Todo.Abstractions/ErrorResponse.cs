using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Abstractions
{
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;

        public ErrorResponse() { }

        public ErrorResponse(string message)
        {
            Message = message;
        }
    }
}
