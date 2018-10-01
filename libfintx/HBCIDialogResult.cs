using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx
{
    public class HBCIDialogResult
    {
        public bool IsSuccess => messages != null && messages.Any(m => m.IsSuccess);
        public bool HasInfo => messages != null && messages.Any(m => m.IsInfo);
        public bool HasWarning => messages != null && messages.Any(m => m.IsWarning);
        public bool HasError => messages != null && messages.Any(m => m.IsError);
        public bool HasUnknown => messages != null && messages.Any(m => m.IsUnknown);
        private List<HBCIBankMessage> messages;
        public IEnumerable<HBCIBankMessage> Messages => messages;

        public HBCIDialogResult()
        {
        }

        public HBCIDialogResult(IEnumerable<HBCIBankMessage> messages)
        {
            this.messages = messages.ToList();
        }
        private void AddBankMessages(Dictionary<string, string> bankMessages)
        {
            foreach (var bankMessage in bankMessages)
            {
                var code = bankMessage.Key;
                var message = bankMessage.Value;
                messages.Add(new HBCIBankMessage(code, message));
            }
        }

        public override string ToString()
        {
            return string.Join(", ", messages);
        }
    }

    public class HBCIDialogResult<T> : HBCIDialogResult
    {
        public T Data { get; set; }
        public HBCIDialogResult(IEnumerable<HBCIBankMessage> bankMessages) : base(bankMessages) { }
        public HBCIDialogResult(IEnumerable<HBCIBankMessage> bankMessages, T data) : base(bankMessages)
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