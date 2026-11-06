namespace SerieA.API.Common
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty; 
        public List<string>? Errors { get; set; }

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
            return new ApiResponse<T>
            {
                Message = message,
                IsSuccess = true,
                Data = data
            };
        }

        public static new ApiResponse<T> Fail(string message = "İşlem başarısız!", List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Message = message,
                IsSuccess = false,
                Errors = errors,
                Data = default
            };
        }
    }
}