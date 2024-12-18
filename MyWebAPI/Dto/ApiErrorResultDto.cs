namespace MyWebAPI.Dto
{
    public class ApiErrorResultDto
    {
        public List<FieldErrorDto> ErrorsMessages { get; set; } = new();
    }
}
