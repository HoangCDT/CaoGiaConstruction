using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CaoGiaConstruction.WebClient.TagHelpers
{
    public enum AlertTypeEnum
    {
        PRIMARY,
        SUCCESS,
        DANGER,
        WARNING,
        INFO,
        LIGHT,
        DARK
    }

    public static class AlertComponent
    {
        public static string AlertVisible { get; set; }
        public static string AlertType { get; set; }
        public static string AlertMessage { get; set; }
        public static string IsShowClose { get; set; }
    }

    public class MessageTagHelper : TagHelper
    {
        public AlertTypeEnum? Type { get; set; } = AlertTypeEnum.INFO;

        public string Content { get; set; } = string.Empty;
        public bool IsShowBackHome { get; set; } = false;
        public bool IsShowClose { get; set; } = true;
        public bool IsHidden { get; set; } = false;
        public bool Visible { get; set; } = false;
        public bool IsAutoHidden { get; set; } = false;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Visible)
            {
                string type = this.Type.ToString().ToLower();
                string content = this.Content;

                if (string.IsNullOrEmpty(content))
                {
                    var elemContent = await output.GetChildContentAsync();
                    content = elemContent.GetContent();
                    this.Content = content;
                }

                string template = $@"
                                <div class='alert alert-{type} alert-dismissible fade show w-100 {(IsAutoHidden ? "alert-auto-hidden" : "")} <2>' role='alert'>
                                  {content}  <0>
                                 <1>
                                </div>
                                 ";

                template = IsShowBackHome ? template.Replace("<0>", "<a href='/' class='alert-link'>Quay trở về trang chủ.</a>") : template.Replace("<0>", string.Empty);
                template = IsShowClose ? template.Replace("<1>", "<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>") : template.Replace("<1>", string.Empty);
                template = IsHidden ? template.Replace("<2>", "alert-close") : template.Replace("<2>", string.Empty);
                output.TagName = string.Empty;
                if (content != string.Empty)
                    output.Content.SetHtmlContent(template);
                else
                    output.Content.SetHtmlContent(string.Empty);
            }
            else
            {
                output.Content.SetHtmlContent(string.Empty);
            }
        }
    }
}
