namespace CaoGiaConstruction.Utilities.Dtos
{
    public class OperationResult
    {
        public OperationResult()
        {
        }

        public OperationResult(int status, string message, object data = null)
        {
            this.StatusCode = status;
            this.Message = message;
            this.Data = data;
            this.Success = status == 200 ? true : false;
        }

        public int StatusCode { set; get; }
        public string Message { set; get; }
        public bool Success { set; get; }
        public object Data { set; get; }
    }
}