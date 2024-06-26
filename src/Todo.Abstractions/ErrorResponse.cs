﻿namespace Todo.Abstractions;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;

    public ErrorResponse() { }

    public ErrorResponse(string message)
    {
        Message = message;
    }
}
