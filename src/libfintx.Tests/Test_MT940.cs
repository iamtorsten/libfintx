using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace libfintx.Tests
{
    public class Test_MT940
    {
        /// <summary>
        /// Deutsche Bank
        /// </summary>
        [Fact]
        public void Test_10070124()
        {
            string mt940 =
@"
:20:DEUTDEFFXXXX
:25:10070124/123456789
:28C:00000/000
:60F:C191204EUR89,45
:61:191204D1,00NMSCNONREF
:86:116?20EREF+NOTPROVIDED?21KREF+NOTPROVIDED?22SVWZ+Geld?30BELADEBEX
XX?31DE12100500000123456789?32Mustermann, Max
:62F:C191204EUR88,45
";
            var result = MT940.Serialize(mt940, "123456789");

            Assert.Single(result);

            var stmt = result[0];
            Assert.Equal("DEUTDEFFXXXX", stmt.type);
            Assert.Equal("10070124", stmt.bankCode);
            Assert.Equal("123456789", stmt.accountCode);
            Assert.Equal(new DateTime(2019, 12, 4), stmt.startDate);
            Assert.Equal(89.45m, stmt.startBalance);

            Assert.Equal(new DateTime(2019, 12, 4), stmt.endDate);
            Assert.Equal(88.45m, stmt.endBalance);

            Assert.Single(stmt.SWIFTTransactions);
            
            var tx = stmt.SWIFTTransactions[0];
            
            Assert.Equal(new DateTime(2019, 12, 4), tx.valueDate);
            Assert.Equal(new DateTime(2019, 12, 4), tx.inputDate);
            Assert.Equal(-1m, tx.amount);
            Assert.Equal("MSC", tx.transactionTypeId);
            Assert.Equal("NONREF", tx.customerReference);
            Assert.Null(tx.bankReference);

            Assert.Equal("116", tx.typecode);
            Assert.Null(tx.text);
            Assert.Null(tx.primanota);
            Assert.Equal("EREF+NOTPROVIDEDKREF+NOTPROVIDEDSVWZ+Geld", tx.description);
            Assert.Equal("NOTPROVIDED", tx.EREF);
            Assert.Equal("NOTPROVIDED", tx.KREF);
            Assert.Equal("Geld", tx.SVWZ);
            Assert.Equal("BELADEBEXXX", tx.bankCode);
            Assert.Equal("DE12100500000123456789", tx.accountCode);
            Assert.Equal("Mustermann, Max", tx.partnerName);
        }

        /// <summary>
        /// Berliner Sparkasse
        /// </summary>
        [Fact]
        public void Test_10050000()
        {
            string mt940 =
@"
:20:STARTUMSE
:25:10050000/0123456789
:28C:00000/001
:60F:C191205EUR11565,61
:61:1912061206DR5,95NDDTNONREF
:86:105?00FOLGELASTSCHRIFT?109218?20EREF+124565?21MREF+124565?22C
RED+DE20ZZZ00000013480?23SVWZ+123456, BelNr. 123456?24FAX.de A?30
NOLADE21HAM?31DE71207500000060017852?32FAX.de GmbH?34992
:62F:C191206EUR11559,66
";
            var result = MT940.Serialize(mt940, "123456789");

            Assert.Single(result);

            var stmt = result[0];
            Assert.Equal("STARTUMSE", stmt.type);
            Assert.Equal("10050000", stmt.bankCode);
            Assert.Equal("123456789", stmt.accountCode);
            Assert.Equal(new DateTime(2019, 12, 5), stmt.startDate);
            Assert.Equal(11565.61m, stmt.startBalance);

            Assert.Equal(new DateTime(2019, 12, 6), stmt.endDate);
            Assert.Equal(11559.66m, stmt.endBalance);

            Assert.Single(stmt.SWIFTTransactions);

            var tx = stmt.SWIFTTransactions[0];

            Assert.Equal(new DateTime(2019, 12, 6), tx.valueDate);
            Assert.Equal(new DateTime(2019, 12, 6), tx.inputDate);
            Assert.Equal(-5.95m, tx.amount);
            Assert.Equal("DDT", tx.transactionTypeId);
            Assert.Equal("NONREF", tx.customerReference);
            Assert.Null(tx.bankReference);

            Assert.Equal("105", tx.typecode);
            Assert.Equal("FOLGELASTSCHRIFT", tx.text);
            Assert.Equal("9218", tx.primanota);
            Assert.Equal("EREF+124565MREF+124565CRED+DE20ZZZ00000013480SVWZ+123456, BelNr. 123456FAX.de A", tx.description);
            Assert.Equal("124565", tx.EREF);
            Assert.Equal("124565", tx.MREF);
            Assert.Equal("DE20ZZZ00000013480", tx.CRED);
            Assert.Equal("123456, BelNr. 123456FAX.de A", tx.SVWZ);
            Assert.Equal("NOLADE21HAM", tx.bankCode);
            Assert.Equal("DE71207500000060017852", tx.accountCode);
            Assert.Equal("FAX.de GmbH", tx.partnerName);
            Assert.Equal("992", tx.textKeyAddition);
        }

        /// <summary>
        /// Postbank Dortmund
        /// </summary>
        [Fact]
        public void Test_44010046()
        {
            string mt940 =
@"
:20:STARTUMS
:21:NONREF
:25:44010046/123456789
:28C:0
:60F:C191203EUR2696,19
:61:1912031203D149,99N005NONREF
:86:106?00KARTENZAHLUNG?20Referenz 654204984863570212?2119163028?22Ma
ndat 174962?23Einreicher-ID DE21Z01000006?2442216?25XXXS026XXX SA
TURN E//BERLIN?26/DE?27Terminal 65420498?282019-12-02T16:30:28?29
Folgenr. 03 Verfalld. 2312?30WELADEDDXXX?31DE38300500000001107713
?32SATURN SAGT DANKE.?34011
:62F:C191209EUR2546,20
";
            var result = MT940.Serialize(mt940, "123456789");

            Assert.Single(result);

            var stmt = result[0];
            Assert.Equal("STARTUMS", stmt.type);
            Assert.Equal("44010046", stmt.bankCode);
            Assert.Equal("123456789", stmt.accountCode);
            Assert.Equal(new DateTime(2019, 12, 3), stmt.startDate);
            Assert.Equal(2696.19m, stmt.startBalance);

            Assert.Equal(new DateTime(2019, 12, 9), stmt.endDate);
            Assert.Equal(2546.20m, stmt.endBalance);

            Assert.Single(stmt.SWIFTTransactions);

            var tx = stmt.SWIFTTransactions[0];

            Assert.Equal(new DateTime(2019, 12, 3), tx.valueDate);
            Assert.Equal(new DateTime(2019, 12, 3), tx.inputDate);
            Assert.Equal(-149.99m, tx.amount);
            Assert.Equal("005", tx.transactionTypeId);
            Assert.Equal("NONREF", tx.customerReference);
            Assert.Null(tx.bankReference);

            Assert.Equal("106", tx.typecode);
            Assert.Equal("KARTENZAHLUNG", tx.text);
            Assert.Null(tx.primanota);
            Assert.Equal("Referenz 65420498486357021219163028Mandat 174962Einreicher-ID DE21Z0100000642216XXXS026XXX SATURN E//BERLIN/DETerminal 654204982019-12-02T16:30:28Folgenr. 03 Verfalld. 2312", tx.description);
            Assert.Equal("WELADEDDXXX", tx.bankCode);
            Assert.Equal("DE38300500000001107713", tx.accountCode);
            Assert.Equal("SATURN SAGT DANKE.", tx.partnerName);
            Assert.Equal("011", tx.textKeyAddition);
        }

        /// <summary>
        /// Hypovereinsbank
        /// </summary>
        [Fact]
        public void Test_10020890()
        {
            string mt940 =
@"
:20:191101
:25:10020890/123456789
:28C:12/1
:60F:C191001EUR22077,2
:61:1911011101C25,NSTONONREF//00900280012998
BANKREFCTC191101IST000002400296725
:86:152?00SEPA-Dauerauftrag?100050?20SVWZ+HVB WILLKOMMENSKONTO?30HYVE
DEMM488?31DE16100208900001234567?32MUSTERMANN MAX
:62F:C191101EUR22102,2
";
            var result = MT940.Serialize(mt940, "123456789");

            Assert.Single(result);

            var stmt = result[0];
            Assert.Equal("191101", stmt.type);
            Assert.Equal("10020890", stmt.bankCode);
            Assert.Equal("123456789", stmt.accountCode);
            Assert.Equal(new DateTime(2019, 10, 1), stmt.startDate);
            Assert.Equal(22077.20m, stmt.startBalance);

            Assert.Equal(new DateTime(2019, 11, 1), stmt.endDate);
            Assert.Equal(22102.20m, stmt.endBalance);

            Assert.Single(stmt.SWIFTTransactions);

            var tx = stmt.SWIFTTransactions[0];

            Assert.Equal(new DateTime(2019, 11, 1), tx.valueDate);
            Assert.Equal(new DateTime(2019, 11, 1), tx.inputDate);
            Assert.Equal(25m, tx.amount);
            Assert.Equal("STO", tx.transactionTypeId);
            Assert.Equal("NONREF", tx.customerReference);
            Assert.Equal("00900280012998", tx.bankReference);
            Assert.Equal("BANKREFCTC191101IST000002400296725", tx.otherInformation);

            Assert.Equal("152", tx.typecode);
            Assert.Equal("SEPA-Dauerauftrag", tx.text);
            Assert.Equal("0050", tx.primanota);
            Assert.Equal("SVWZ+HVB WILLKOMMENSKONTO", tx.description);
            Assert.Equal("HVB WILLKOMMENSKONTO", tx.SVWZ);
            Assert.Equal("HYVEDEMM488", tx.bankCode);
            Assert.Equal("DE16100208900001234567", tx.accountCode);
            Assert.Equal("MUSTERMANN MAX", tx.partnerName);
        }
    }
}
