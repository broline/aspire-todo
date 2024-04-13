using System.Text.Json.Serialization;

namespace Todo.Abstractions;

[JsonSerializable(typeof(TodoItem))]
[JsonSerializable(typeof(TodoList))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = false, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class TodoApiSerializationContext : JsonSerializerContext
{
}
