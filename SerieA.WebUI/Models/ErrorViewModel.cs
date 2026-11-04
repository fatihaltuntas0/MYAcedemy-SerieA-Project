namespace SerieA.WebUI.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public int StatusCode { get; set; } = 500;
        public string Message { get; set; } = "Beklenmedik bir hata oluştu.";
        public string? ExceptionMessage { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
