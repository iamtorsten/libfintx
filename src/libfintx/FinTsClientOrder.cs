using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using libfintx.Data;
using static libfintx.HKCDE;

namespace libfintx
{
    public partial class FinTsClient
    {
        /// <summary>
        /// Get banker's orders
        /// </summary>
        /// <param name="tanDialog">The TAN dialog</param>         
        /// <returns>
        /// Banker's orders
        /// </returns>
        public HBCIDialogResult<List<BankersOrder>> GetBankersOrders(TANDialog tanDialog)
        {
            var result = InitializeConnection();
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            // Success
            string BankCode = Transaction.HKCDB(this);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result.TypedResult<List<BankersOrder>>();

            BankCode = result.RawData;
            int startIdx = BankCode.IndexOf("HICDB");
            if (startIdx < 0)
                return result.TypedResult<List<BankersOrder>>();

            var data = new List<BankersOrder>();

            string BankCode_ = BankCode.Substring(startIdx);
            for (; ; )
            {
                var match = Regex.Match(BankCode_, @"HICDB.+?(?<xml><\?xml.+?</Document>)\+(?<orderid>.*?)\+(?<firstdate>\d*):(?<turnus>[MW]):(?<rota>\d+):(?<execday>\d+)(:(?<lastdate>\d+))?", RegexOptions.Singleline);
                if (match.Success)
                {
                    string xml = match.Groups["xml"].Value;
                    // xml ist UTF-8
                    xml = Converter.ConvertEncoding(xml, Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8);

                    string orderId = match.Groups["orderid"].Value;

                    string firstExecutionDateStr = match.Groups["firstdate"].Value;
                    var firstExecutionDate = !string.IsNullOrWhiteSpace(firstExecutionDateStr) ? DateTime.ParseExact(firstExecutionDateStr, "yyyyMMdd", CultureInfo.InvariantCulture) : default(DateTime?);

                    string timeUnitStr = match.Groups["turnus"].Value;
                    var timeUnit = timeUnitStr == "M" ? TimeUnit.Monthly : TimeUnit.Weekly;

                    string rota = match.Groups["rota"].Value;

                    string executionDayStr = match.Groups["execday"].Value;
                    int executionDay = Convert.ToInt32(executionDayStr);

                    string lastExecutionDateStr = match.Groups["lastdate"].Value;
                    var lastExecutionDate = !string.IsNullOrWhiteSpace(lastExecutionDateStr) ? DateTime.ParseExact(lastExecutionDateStr, "yyyyMMdd", CultureInfo.InvariantCulture) : default(DateTime?);

                    var painData = Pain00100103CtData.Create(xml);

                    if (firstExecutionDate.HasValue && executionDay > 0)
                    {
                        var item = new BankersOrder(orderId, painData, firstExecutionDate.Value, timeUnit, rota, executionDay, lastExecutionDate);
                        data.Add(item);
                    }
                }

                int endIdx = BankCode_.IndexOf("'");
                if (BankCode_.Length <= endIdx + 1)
                    break;

                BankCode_ = BankCode_.Substring(endIdx + 1);
                startIdx = BankCode_.IndexOf("HICDB");
                if (startIdx < 0)
                    break;
            }

            // Success
            return result.TypedResult(data);
        }

        public HBCIDialogResult DeleteBankersOrder(TANDialog tanDialog, string orderId, string receiverName, string receiverIBAN,
                string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, TimeUnit timeUnit, string rota, int executionDay, DateTime? lastExecutionDay, string hirms)
        {
            var result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCDL(this, orderId, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, lastExecutionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }

        /// <summary>
        /// Submit bankers order - General method
        /// </summary>
        /// <param name="receiverName"></param>
        /// <param name="receiverIBAN"></param>
        /// <param name="receiverBIC"></param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="purpose">Short description of the transfer (dt. Verwendungszweck)</param>      
        /// <param name="firstTimeExecutionDay"></param>
        /// <param name="timeUnit"></param>
        /// <param name="rota"></param>
        /// <param name="executionDay"></param>
        /// <param name="HIRMS">Numerical SecurityMode; e.g. 911 for "Sparkasse chipTan optisch"</param>
        /// <param name="pictureBox">Picturebox which shows the TAN</param>
        /// <param name="flickerImage">(Out) reference to an image object that shall receive the FlickerCode as GIF image</param>
        /// <param name="flickerWidth">Width of the flicker code</param>
        /// <param name="flickerHeight">Height of the flicker code</param>
        /// <param name="renderFlickerCodeAsGif">Renders flicker code as GIF, if 'true'</param>
        /// <returns>
        /// Bank return codes
        /// </returns>
        public HBCIDialogResult SubmitBankersOrder(TANDialog tanDialog, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, TimeUnit timeUnit, string rota,
           int executionDay, DateTime? lastExecutionDay, string hirms)
        {
            var result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCDE(this, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, lastExecutionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }

        public HBCIDialogResult ModifyBankersOrder(TANDialog tanDialog, string OrderId, string receiverName, string receiverIBAN,
           string receiverBIC, decimal amount, string purpose, DateTime firstTimeExecutionDay, TimeUnit timeUnit, string rota,
           int executionDay, DateTime? lastExecutionDay, string hirms)
        {
            var result = InitializeConnection();
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);
            if (!result.IsSuccess)
                return result;

            TransactionConsole.Output = string.Empty;

            if (!string.IsNullOrEmpty(hirms))
                HIRMS = hirms;

            string BankCode = Transaction.HKCDN(this, OrderId, receiverName, receiverIBAN, receiverBIC, amount, purpose, firstTimeExecutionDay, timeUnit, rota, executionDay, lastExecutionDay);
            result = new HBCIDialogResult(Helper.Parse_BankCode(BankCode), BankCode);
            if (!result.IsSuccess)
                return result;

            result = ProcessSCA(result, tanDialog);

            return result;
        }
    }
}
