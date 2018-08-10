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
        public T Result { get; }

        public static HBCIDialogResult<T> Error(string errorMessage)
        {
            return new HBCIDialogResult<T>(false, errorMessage, default(T));
        }

        public static HBCIDialogResult<T> Success(T result)
        {
            return new HBCIDialogResult<T>(true, null, result);
        }

        private HBCIDialogResult(bool success, string errorMessage, T result) : base(success, errorMessage)
        {
            Result = result;
        }
    }
}
