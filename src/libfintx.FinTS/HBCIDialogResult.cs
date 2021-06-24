/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using System.Collections.Generic;
using System.Linq;

namespace libfintx.FinTS
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
        public bool IsTanRequired => GetMessage("0030") != null;

        /// <summary>
        /// Returns true if there is any message with code <i>3955</i>.
        /// </summary>
        public bool IsApprovalRequired => GetMessage("3955") != null;

        /// <summary>
        /// Returns true if there is any message with code <i>3955</i>.
        /// </summary>
        public bool IsWaitingForApproval => GetMessage("3956") != null;

        private readonly List<HBCIBankMessage> messages;
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
