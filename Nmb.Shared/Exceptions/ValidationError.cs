using Nmb.Shared.Extensions;

namespace Nmb.Shared.Exceptions
{
    public class ValidationError
    {
        public ValidationError(string field, string message)
        {
            Field = field?.ToCamelCasing();
            Message = message;
        }

        public ValidationError(string message)
        {
            Field = string.Empty;
            Message = message;
        }

        public string Field { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"Field = {Field} | Message = {Message}";
        }
    }
}
