namespace SerieA.WebUI.Models
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public List<string>? Errors { get; set; }
        public string Message { get; set; }

        public static ApiResponse Success(string message = "İşlem başarılı.")
        {
            return new ApiResponse { IsSuccess = true, Message = message };
        }
        public static ApiResponse Fail(string message, List<string>? errors = null)
        {
            return new ApiResponse { IsSuccess = false, Message = message, Errors = errors };
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = "İşlem başarılı.")
        {
            return new ApiResponse<T> { IsSuccess = true, Message = message, Data = data };
        }
        public static new ApiResponse<T> Fail(string message, List<string>? errors = null)
        {
            return new ApiResponse<T> { IsSuccess = false, Message = message, Errors = errors, Data = default };
        }
    }
}
