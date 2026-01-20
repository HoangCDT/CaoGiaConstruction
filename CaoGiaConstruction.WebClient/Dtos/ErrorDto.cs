namespace CaoGiaConstruction.WebClient.Dtos
{
    public class ErrorDto
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public int Code { get; set; }

        public string Message { get; set; }
    }
}
