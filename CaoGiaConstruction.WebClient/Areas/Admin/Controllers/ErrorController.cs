using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.WebClient.Dtos;

namespace CaoGiaConstruction.WebClient.Areas.Admin.Controllers
{
    public class ErrorController : BaseController
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index(int? code)
        {
            var error = new ErrorDto();

            // Dictionary to map error codes to messages
            Dictionary<int, string> errorMessages = new Dictionary<int, string>
             {
                 // 4xx: Lỗi từ phía Khách hàng
                { 400, "Máy chủ không hiểu yêu cầu." },
                { 401, "Trang yêu cầu yêu cầu tên người dùng và mật khẩu." },
                { 402, "Bạn không thể sử dụng mã này vào lúc này." },
                { 403, "Truy cập bị từ chối đối với trang yêu cầu." },
                { 404, "Máy chủ không thể tìm thấy trang yêu cầu." },
                { 405, "Phương thức chỉ định trong yêu cầu không được phép." },
                { 406, "Máy chủ chỉ có thể tạo một phản hồi mà khách hàng không chấp nhận." },
                { 407, "Bạn phải xác thực với máy chủ proxy trước khi yêu cầu này được phục vụ." },
                { 409, "Yêu cầu không thể hoàn thành do xung đột." },
                { 410, "Trang yêu cầu không còn tồn tại." },
                { 411, "Chiều dài nội dung không được xác định. Máy chủ sẽ không chấp nhận yêu cầu mà không có nó." },
                { 412, "Điều kiện tiên nghiệm được đưa ra trong yêu cầu không đúng với máy chủ." },
                { 413, "Máy chủ sẽ không chấp nhận yêu cầu vì thực thể yêu cầu quá lớn." },
                { 414, "Máy chủ sẽ không chấp nhận yêu cầu vì URL quá dài." },
                { 415, "Máy chủ sẽ không chấp nhận yêu cầu vì loại phương tiện không được hỗ trợ." },
                { 416, "Dãy byte yêu cầu không có sẵn và vượt ra ngoài giới hạn." },
                { 417, "Sự kì vọng được đưa ra trong trường tiêu đề Expect của yêu cầu không thể được đáp ứng bởi máy chủ này." },
                
                // 5xx: Lỗi từ phía Máy chủ
                { 500, "Yêu cầu không được hoàn thành. Máy chủ gặp tình trạng không mong muốn." },
                { 501, "Yêu cầu không được hoàn thành. Máy chủ không hỗ trợ chức năng cần thiết." },
                { 502, "Yêu cầu không được hoàn thành. Máy chủ nhận phản hồi không hợp lệ từ máy chủ vận hành trên." },
                { 503, "Yêu cầu không được hoàn thành. Máy chủ đang tải quá tạm thời hoặc bị ngừng hoạt động." },
                { 504, "Cổng đã hết thời gian." },
                { 505, "Máy chủ không hỗ trợ phiên bản giao thức HTTP." }

             };

            if (errorMessages.ContainsKey(code.GetValueOrDefault()))
            {
                error.Code = code.GetValueOrDefault();
                error.Message = errorMessages[code.GetValueOrDefault()];
            }

            return View(error.IsNullOrEmpty() ? new ErrorDto() : error);
        }
    }
}
