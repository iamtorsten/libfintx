using System;
using System.Linq;
using Xunit;

namespace libfintx.Tests
{
    public class Test_MT942
    {
        /// <summary>
        /// Deutsche Bank
        /// </summary>
        [Fact]
        public void Test_10070848()
        {
            string mt942 =
@"
:20:DEUTDEFFXXXX
:25:10070848/12345670000
:28C:00000/001
:34F:EUR0,
:13:1912092359
:90D:0EUR0,
:90C:0EUR0,
-
:20:DEUTDEFFXXXX
:25:00000000/DE78100708480123456700
:28C:00000/001
:34F:EURC0,
:13D:1912090901+0100
:90D:0EUR0,
:90C:0EUR0,
-
";
            var result = MT940.Serialize(mt942, "123456700", false, true);

            Assert.Equal(2, result.Count);
            Assert.True(result.All(s => s.Pending));

            var stmt = result[0];
            Assert.Equal("DEUTDEFFXXXX", stmt.Type);
            Assert.Equal("10070848", stmt.BankCode);
            Assert.Equal("12345670000", stmt.AccountCode);

            Assert.Equal(new DateTime(2019, 12, 9, 23, 59, 0), stmt.CreationDate);

            Assert.Equal(0, stmt.CountDebit);
            Assert.Equal(0, stmt.AmountDebit);
            Assert.Equal(0, stmt.CountCredit);
            Assert.Equal(0, stmt.AmountCredit);

            Assert.Empty(stmt.SwiftTransactions);

            stmt = result[1];
            Assert.Equal("DEUTDEFFXXXX", stmt.Type);
            Assert.Equal("00000000", stmt.BankCode);
            Assert.Equal("DE78100708480123456700", stmt.AccountCode);

            Assert.Equal(new DateTime(2019, 12, 9, 9, 1, 00), stmt.CreationDate);

            Assert.Equal(0, stmt.CountDebit);
            Assert.Equal(0, stmt.AmountDebit);
            Assert.Equal(0, stmt.CountCredit);
            Assert.Equal(0, stmt.AmountCredit);

            Assert.Empty(stmt.SwiftTransactions);
        }

        /// <summary>
        /// Berliner Sparkasse
        /// </summary>
        [Fact]
        public void Test_10050000()
        {
            string mt942 =
@"
:20:STARTDISPE
:25:10050000/0123456789
:28C:00000/001
:34F:EURD826,90
:13:1911071300
:61:1911081105DR826,90NDDTNONREF
:86:105?00FOLGELASTSCHRIFT?109248?20EREF+Zahlbeleg 372309853527?2
1MREF+DE00020500061000000000?220000005061316?23CRED+DE93ZZZ000000
78611?24SVWZ+Mobilfunk Kundenkonto?250012345678 Mobilfunk-Geraet?
26 00001234567890/01.11.2019?30HYVEDEMMXXX?31DE687002027006673022
69?32Telekom Deutschland GmbH?34992
:61:1911081106DR5,95NDDTNONREF
:86:105?00FOLGELASTSCHRIFT?109218?20EREF+124565?21MREF+124565?22C
RED+DE20ZZZ00000013480?23SVWZ+933701, BelNr. 933701?24FAX.de A?30
NOLADE21HAM?31DE71207500000060017852?32FAX.de GmbH?34992
:61:1911081107DR5,98N011NONREF
:86:107?00SEPA-ELV-LASTSCHRIFT?109248?20EREF+ELV91400493 06.11 17
.5?214 ME1?22MREF+7402568179871911061754?23CRED+DE86ZZZ0000023752
6?24SVWZ+ELV91400493 06.11 17.5?254 ME1 STAR TST RHINSTRASSE?2647
?30HELADEFFXXX?31DE39500500000096500038?32Orlen Deutschland GmbH?
34019
:90D:3EUR838,83
:90C:0EUR0,00
-
";
            var result = MT940.Serialize(mt942, "0123456789", false, true);

            Assert.Single(result);
            Assert.True(result.All(s => s.Pending));

            var stmt = result[0];
            Assert.Equal("STARTDISPE", stmt.Type);
            Assert.Equal("10050000", stmt.BankCode);
            Assert.Equal("123456789", stmt.AccountCode);

            Assert.Equal(new DateTime(2019, 11, 7, 13, 0, 0), stmt.CreationDate);

            Assert.Equal(3, stmt.CountDebit);
            Assert.Equal(-838.83m, stmt.AmountDebit);
            Assert.Equal(0, stmt.CountCredit);
            Assert.Equal(0, stmt.AmountCredit);

            Assert.Equal(3, stmt.SwiftTransactions.Count);

        }
    }
}
