namespace MyWebAPI.Dto
{
    public class FieldErrorDto
    {
        public string? Message { get; set; }
        public string? Field { get; set; }

        public FieldErrorDto()
        {

        }

        public FieldErrorDto(string message, string field)
        {
            Message = message;
            Field = field;
        }
    }
}
