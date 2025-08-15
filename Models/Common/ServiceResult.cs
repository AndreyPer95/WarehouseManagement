namespace WarehouseManagement.Models.Common
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public List<string> Errors { get; private set; }

        private ServiceResult()
        {
            Errors = new List<string>();
        }

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        public static ServiceResult<T> Failure(List<string> errors)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Errors = errors,
                ErrorMessage = string.Join("; ", errors)
            };
        }
    }

    // Версия без данных для операций, которые не возвращают результат
    public class ServiceResult
    {
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
        public List<string> Errors { get; private set; }

        private ServiceResult()
        {
            Errors = new List<string>();
        }

        public static ServiceResult Success()
        {
            return new ServiceResult { IsSuccess = true };
        }

        public static ServiceResult Failure(string errorMessage)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        public static ServiceResult Failure(List<string> errors)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                Errors = errors,
                ErrorMessage = string.Join("; ", errors)
            };
        }
    }
}