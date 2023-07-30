using System;
using System.IO;
using libfintx.FinTS.Statement;
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
            Assert.Equal("DEUTDEFFXXXX", stmt.Type);
            Assert.Equal("10070124", stmt.BankCode);
            Assert.Equal("123456789", stmt.AccountCode);
            Assert.Equal(new DateTime(2019, 12, 4), stmt.StartDate);
            Assert.Equal(89.45m, stmt.StartBalance);

            Assert.Equal(new DateTime(2019, 12, 4), stmt.EndDate);
            Assert.Equal(88.45m, stmt.EndBalance);

            Assert.Single(stmt.SwiftTransactions);

            var tx = stmt.SwiftTransactions[0];

            Assert.Equal(new DateTime(2019, 12, 4), tx.ValueDate);
            Assert.Equal(new DateTime(2019, 12, 4), tx.InputDate);
            Assert.Equal(-1m, tx.Amount);
            Assert.Equal("NMSC", tx.TransactionTypeId);
            Assert.Equal("NONREF", tx.CustomerReference);
            Assert.Null(tx.BankReference);

            Assert.Equal("116", tx.TypeCode);
            Assert.Null(tx.Text);
            Assert.Null(tx.Primanota);
            Assert.Equal("EREF+NOTPROVIDEDKREF+NOTPROVIDEDSVWZ+Geld", tx.Description);
            Assert.Equal("NOTPROVIDED", tx.EREF);
            Assert.Equal("NOTPROVIDED", tx.KREF);
            Assert.Equal("Geld", tx.SVWZ);
            Assert.Equal("BELADEBEXXX", tx.BankCode);
            Assert.Equal("DE12100500000123456789", tx.AccountCode);
            Assert.Equal("Mustermann, Max", tx.PartnerName);
        }

        /// <summary>
        /// Deutsche Bank
        /// </summary>
        [Fact]
        public void Test_10070848()
        {
            string mt940 =
@"
:20:DEUTDEFFXXXX
:25:10070848/123456789
:28C:00000/000
:60F:C191129EUR3930,41
:61:191129C66,20NMSCNONREF
:86:166?20EREF+0041961450206?21SVWZ+65-489042-01 /00419614?2250206 65
-48904201 RENTE?30WELADEDDXXX?31DE90300500000072000003?32GOTHAER 
LEBENSVERSICHERUNG
:62F:C191129EUR3996,61
";
            var result = MT940.Serialize(mt940, "123456789");

            Assert.Single(result);

            var stmt = result[0];
            Assert.Equal("DEUTDEFFXXXX", stmt.Type);
            Assert.Equal("10070848", stmt.BankCode);
            Assert.Equal("123456789", stmt.AccountCode);
            Assert.Equal(new DateTime(2019, 11, 29), stmt.StartDate);
            Assert.Equal(3930.41m, stmt.StartBalance);

            Assert.Equal(new DateTime(2019, 11, 29), stmt.EndDate);
            Assert.Equal(3996.61m, stmt.EndBalance);

            Assert.Single(stmt.SwiftTransactions);

            var tx = stmt.SwiftTransactions[0];

            Assert.Equal(new DateTime(2019, 11, 29), tx.ValueDate);
            Assert.Equal(new DateTime(2019, 11, 29), tx.InputDate);
            Assert.Equal(66.20m, tx.Amount);
            Assert.Equal("NMSC", tx.TransactionTypeId);
            Assert.Equal("NONREF", tx.CustomerReference);
            Assert.Null(tx.BankReference);

            Assert.Equal("166", tx.TypeCode);
            Assert.Null(tx.Text);
            Assert.Null(tx.Primanota);
            Assert.Equal("EREF+0041961450206SVWZ+65-489042-01 /0041961450206 65-48904201 RENTE", tx.Description);
            Assert.Equal("0041961450206", tx.EREF);
            Assert.Equal("65-489042-01 /0041961450206 65-48904201 RENTE", tx.SVWZ);
            Assert.Equal("WELADEDDXXX", tx.BankCode);
            Assert.Equal("DE90300500000072000003", tx.AccountCode);
            Assert.Equal("GOTHAER LEBENSVERSICHERUNG", tx.PartnerName);
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
            Assert.Equal("STARTUMSE", stmt.Type);
            Assert.Equal("10050000", stmt.BankCode);
            Assert.Equal("123456789", stmt.AccountCode);
            Assert.Equal(new DateTime(2019, 12, 5), stmt.StartDate);
            Assert.Equal(11565.61m, stmt.StartBalance);

            Assert.Equal(new DateTime(2019, 12, 6), stmt.EndDate);
            Assert.Equal(11559.66m, stmt.EndBalance);

            Assert.Single(stmt.SwiftTransactions);

            var tx = stmt.SwiftTransactions[0];

            Assert.Equal(new DateTime(2019, 12, 6), tx.ValueDate);
            Assert.Equal(new DateTime(2019, 12, 6), tx.InputDate);
            Assert.Equal(-5.95m, tx.Amount);
            Assert.Equal("NDDT", tx.TransactionTypeId);
            Assert.Equal("NONREF", tx.CustomerReference);
            Assert.Null(tx.BankReference);

            Assert.Equal("105", tx.TypeCode);
            Assert.Equal("FOLGELASTSCHRIFT", tx.Text);
            Assert.Equal("9218", tx.Primanota);
            Assert.Equal("EREF+124565MREF+124565CRED+DE20ZZZ00000013480SVWZ+123456, BelNr. 123456FAX.de A", tx.Description);
            Assert.Equal("124565", tx.EREF);
            Assert.Equal("124565", tx.MREF);
            Assert.Equal("DE20ZZZ00000013480", tx.CRED);
            Assert.Equal("123456, BelNr. 123456FAX.de A", tx.SVWZ);
            Assert.Equal("NOLADE21HAM", tx.BankCode);
            Assert.Equal("DE71207500000060017852", tx.AccountCode);
            Assert.Equal("FAX.de GmbH", tx.PartnerName);
            Assert.Equal("992", tx.TextKeyAddition);
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
            Assert.Equal("STARTUMS", stmt.Type);
            Assert.Equal("44010046", stmt.BankCode);
            Assert.Equal("123456789", stmt.AccountCode);
            Assert.Equal(new DateTime(2019, 12, 3), stmt.StartDate);
            Assert.Equal(2696.19m, stmt.StartBalance);

            Assert.Equal(new DateTime(2019, 12, 9), stmt.EndDate);
            Assert.Equal(2546.20m, stmt.EndBalance);

            Assert.Single(stmt.SwiftTransactions);

            var tx = stmt.SwiftTransactions[0];

            Assert.Equal(new DateTime(2019, 12, 3), tx.ValueDate);
            Assert.Equal(new DateTime(2019, 12, 3), tx.InputDate);
            Assert.Equal(-149.99m, tx.Amount);
            Assert.Equal("N005", tx.TransactionTypeId);
            Assert.Equal("NONREF", tx.CustomerReference);
            Assert.Null(tx.BankReference);

            Assert.Equal("106", tx.TypeCode);
            Assert.Equal("KARTENZAHLUNG", tx.Text);
            Assert.Null(tx.Primanota);
            Assert.Equal("Referenz 65420498486357021219163028Mandat 174962Einreicher-ID DE21Z0100000642216XXXS026XXX SATURN E//BERLIN/DETerminal 654204982019-12-02T16:30:28Folgenr. 03 Verfalld. 2312", tx.Description);
            Assert.Equal("WELADEDDXXX", tx.BankCode);
            Assert.Equal("DE38300500000001107713", tx.AccountCode);
            Assert.Equal("SATURN SAGT DANKE.", tx.PartnerName);
            Assert.Equal("011", tx.TextKeyAddition);
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
            Assert.Equal("191101", stmt.Type);
            Assert.Equal("10020890", stmt.BankCode);
            Assert.Equal("123456789", stmt.AccountCode);
            Assert.Equal(new DateTime(2019, 10, 1), stmt.StartDate);
            Assert.Equal(22077.20m, stmt.StartBalance);

            Assert.Equal(new DateTime(2019, 11, 1), stmt.EndDate);
            Assert.Equal(22102.20m, stmt.EndBalance);

            Assert.Single(stmt.SwiftTransactions);

            var tx = stmt.SwiftTransactions[0];

            Assert.Equal(new DateTime(2019, 11, 1), tx.ValueDate);
            Assert.Equal(new DateTime(2019, 11, 1), tx.InputDate);
            Assert.Equal(25m, tx.Amount);
            Assert.Equal("NSTO", tx.TransactionTypeId);
            Assert.Equal("NONREF", tx.CustomerReference);
            Assert.Equal("00900280012998", tx.BankReference);
            Assert.Equal("BANKREFCTC191101IST000002400296725", tx.OtherInformation);

            Assert.Equal("152", tx.TypeCode);
            Assert.Equal("SEPA-Dauerauftrag", tx.Text);
            Assert.Equal("0050", tx.Primanota);
            Assert.Equal("SVWZ+HVB WILLKOMMENSKONTO", tx.Description);
            Assert.Equal("HVB WILLKOMMENSKONTO", tx.SVWZ);
            Assert.Equal("HYVEDEMM488", tx.BankCode);
            Assert.Equal("DE16100208900001234567", tx.AccountCode);
            Assert.Equal("MUSTERMANN MAX", tx.PartnerName);
        }

        /// <summary>
        /// Sparkasse Freising
        /// </summary>
        [Fact]
        public void Test_70051003()
        {
            string mt940 =
@"
:20:STARTUMSE
:25:10050000/0123456789
:28C:00000/001
:60F:C190930EUR11565,61
:61:1910011001DR3393,40N029NONREF
:86:116?00EINZELUEBERWEISUNG?109301?20EREF+4927411196-0000001?21K
REF+4927411196?22SVWZ+RNr. 20190930001 RDat.?23 30.09.2019 KNr. C
-20170100?241?30BYLADEM1FSI?31DE49700510030025617937?32GEILER KAF
FEE GmbH?34997
:62F:C190930EUR8172,21
";
            var result = MT940.Serialize(mt940, "123456789");

            Assert.Single(result);

            var stmt = result[0];
            Assert.Equal("STARTUMSE", stmt.Type);
            Assert.Equal("10050000", stmt.BankCode);
            Assert.Equal("123456789", stmt.AccountCode);
            Assert.Equal(new DateTime(2019, 09, 30), stmt.StartDate);
            Assert.Equal(11565.61m, stmt.StartBalance);

            Assert.Equal(new DateTime(2019, 09, 30), stmt.EndDate);
            Assert.Equal(8172.21m, stmt.EndBalance);

            Assert.Single(stmt.SwiftTransactions);

            var tx = stmt.SwiftTransactions[0];

            Assert.Equal(new DateTime(2019, 10, 1), tx.ValueDate);
            Assert.Equal(new DateTime(2019, 10, 1), tx.InputDate);
            Assert.Equal(-3393.40m, tx.Amount);
            Assert.Equal("N029", tx.TransactionTypeId);
            Assert.Equal("NONREF", tx.CustomerReference);
            Assert.Null(tx.BankReference);

            Assert.Equal("116", tx.TypeCode);
            Assert.Equal("EINZELUEBERWEISUNG", tx.Text);
            Assert.Equal("9301", tx.Primanota);
            Assert.Equal("EREF+4927411196-0000001KREF+4927411196SVWZ+RNr. 20190930001 RDat. 30.09.2019 KNr. C-201701001", tx.Description);
            Assert.Equal("4927411196-0000001", tx.EREF);
            Assert.Equal("4927411196", tx.KREF);
            Assert.Equal("RNr. 20190930001 RDat. 30.09.2019 KNr. C-201701001", tx.SVWZ);
            Assert.Equal("BYLADEM1FSI", tx.BankCode);
            Assert.Equal("DE49700510030025617937", tx.AccountCode);
            Assert.Equal("GEILER KAFFEE GmbH", tx.PartnerName);
            Assert.Equal("997", tx.TextKeyAddition);
        }

        [Fact]
        public void TestLeo()
        {
            using var sr = new StreamReader(@"C:\git\leo-schick\SharpMt940Lib.Core\Raptorious.SharpMt940Lib.Tests\Samples\MaerzUmsaetze_10944511_15042023.mta");
            var f = sr.ReadToEnd();

            var statements = MT940.Serialize(f);
        }
    }
}
