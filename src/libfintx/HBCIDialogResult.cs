using System.Collections.Generic;
using System.Linq;

namespace libfintx
{
    public class HBCIDialogResult
    {
        public string RawData { get; internal set; }

        public bool IsSuccess => messages.Any(m => m.IsSuccess);
        public bool HasInfo => messages.Any(m => m.IsInfo);
        public bool HasWarning => messages.Any(m => m.IsWarning);
        public bool HasError => messages.Any(m => m.IsError);
        public bool HasUnknown => messages.Any(m => m.IsUnknown);

        /// <summary>
        /// Returns true if there is any message with code <i>0030</i>.
        /// </summary>
        public bool IsSCARequired => GetMessage("0030") != null;

        private List<HBCIBankMessage> messages;
        public IEnumerable<HBCIBankMessage> Messages => messages;

        public HBCIDialogResult(IEnumerable<HBCIBankMessage> messages, string rawData)
        {
            this.messages = messages.ToList();
            RawData = rawData;
        }

        public string GetMessage(string code)
        {
            return messages.FirstOrDefault(m => m.Code == code)?.Message;
        }

        public HBCIDialogResult<T> TypedResult<T>()
        {
            return new HBCIDialogResult<T>(Messages, RawData);
        }

        public HBCIDialogResult<T> TypedResult<T>(T data)
        {
            return new HBCIDialogResult<T>(Messages, RawData, data);
        }

        public override string ToString()
        {
            return string.Join(", ", messages);
        }
    }

    public class HBCIDialogResult<T> : HBCIDialogResult
    {
        public T Data { get; set; }
        public HBCIDialogResult(IEnumerable<HBCIBankMessage> bankMessages, string rawData) : base(bankMessages, rawData) { }
        public HBCIDialogResult(IEnumerable<HBCIBankMessage> bankMessages, string rawData, T data) : base(bankMessages, rawData)
        {
            Data = data;
        }
    }

    public class HBCIBankMessage
    {
        public enum TypeEnum
        {
            Success, Info, Warning, Error, Unknown
        }
        public TypeEnum Type { get; }
        public bool IsSuccess { get => TypeEnum.Success == Type; }
        public bool IsInfo { get => TypeEnum.Info == Type; }
        public bool IsWarning { get => TypeEnum.Warning == Type; }
        public bool IsError { get => TypeEnum.Error == Type; }
        public bool IsUnknown { get => TypeEnum.Error == Type; }
        public string Code { get; }
        public string Message { get; }
        public HBCIBankMessage(string code, string message)
        {
            Code = code;
            Message = message;
            if (code.StartsWith("0"))
                Type = TypeEnum.Success;
            else if (code.StartsWith("1"))
                Type = TypeEnum.Info;
            else if (code.StartsWith("3"))
                Type = TypeEnum.Warning;
            else if (code.StartsWith("9"))
                Type = TypeEnum.Error;
            else
                Type = TypeEnum.Unknown;
        }

        public override string ToString()
        {
            return $"{Code}: {Message}";
        }
    }
}