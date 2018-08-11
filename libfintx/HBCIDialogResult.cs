using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libfintx
{
    public class HBCIDialogResult
    {
        public bool IsSuccess { get; }

        public string ErrorMessage { get; }

        protected HBCIDialogResult()
        {
            IsSuccess = true;
        }

        protected HBCIDialogResult(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }

        protected HBCIDialogResult(bool success, string errorMessage)
        {
            IsSuccess = success;
            ErrorMessage = errorMessage;
        }

        public static HBCIDialogResult DefaultSuccess()
        {
            return new HBCIDialogResult();
        }

        public static HBCIDialogResult DefaultError(string errorMessage)
        {
            return new HBCIDialogResult(errorMessage);
        }
    }

    public class HBCIDialogResult<T> : HBCIDialogResult
    {
        public T Data { get; }

        public static HBCIDialogResult<T> Error(string errorMessage)
        {
            return new HBCIDialogResult<T>(false, errorMessage, default(T));
        }

        public static HBCIDialogResult<T> Success(T data)
        {
            return new HBCIDialogResult<T>(true, null, data);
        }

        private HBCIDialogResult(bool success, string errorMessage, T data) : base(success, errorMessage)
        {
            Data = data;
        }
    }
}
