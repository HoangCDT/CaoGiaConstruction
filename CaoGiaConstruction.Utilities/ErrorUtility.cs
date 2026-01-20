using CaoGiaConstruction.Utilities.Dtos;

namespace CaoGiaConstruction.Utilities
{
    public static class ErrorUtility
    {
        public static OperationResult GetMessageError(this Exception ex)
        {
            return new OperationResult
            {
                StatusCode = 400,
                Message = "Đã xảy ra lỗi với yêu cầu này. Chúng tôi đang cố gắng sửa lỗi sớm nhất có thể.",
                Success = false
            };
        }

        public static OperationResult NotFoundResult()
        {
            return new OperationResult()
            {
                StatusCode = 204,
                Success = false,
                Message = "Rất tiếc, Không tìm thấy dữ liệu phù hợp trong hệ thống !"
            };
        }
    }
}