using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using libfintx.FinTS;
using libfintx.FinTS.Data;
using libfintx.FinTS.Data.Segment;
using Xunit;

namespace libfintx.Tests
{
    public class HelperTest
    {
        [Fact]
        public void TestSplitSegments_1()
        {
            var message =
@"HNHBK:1:3+000000000443+300+kQhXXcecBq4120210429134136079%+2+kQhXXcecBq4120210429134136079%:2'
HNVSK:998:3+PIN:1+998+1+2::37971492958550005K551VUX2BCW3U+1:20210429:134136+2:2:13:@8@        :5:1+280:10070124:XXXXXX:V:0:0+0'
HNVSD:999:1+@187@
HIRMG:2:2+9050::Teilweise fehlerhaft.'
HIRMS:3:2:3+9210::Wert widerspricht Bankvorgaben.'
HIRMS:4:2:4+9210::Auftrag abgelehnt - Zwei-Schritt-TAN inkonsistent. Eingereichter Auftrag gelösch''
HNHBS:5:1+2'".Replace(Environment.NewLine, string.Empty);

            var segments = Helper.SplitEncryptedSegments(message);
            Assert.Equal(5, segments.Count);

            Assert.StartsWith("HNHBK", segments[0]);
            Assert.StartsWith("HIRMG", segments[1]);
            Assert.StartsWith("HIRMS", segments[2]);
            Assert.StartsWith("HIRMS", segments[3]);
            Assert.StartsWith("HNHBS", segments[4]);
        }

        [Fact]
        public void TestSplitSegments_2()
        {
            var message = "HNHBK:1:3+000000000605+300+IL1061013552403+1+IL1061013552403:1'HNVSK:998:3+PIN:2+998+1+2::bTZX3AOq9XkBAABqEKiFhWuowAQA+1:20210610:135525+2:2:13:@8@00000000:5:1+280:10090000:XXXXXXXXXXX:V:0:0+0'HNVSD:999:1+@382@HNSHK:2:4+PIN:2+944+967729067269966+1+1+2::bTZX3AOq9XkBAABqEKiFhWuowAQA+1+1:20210610:135525+1:999:1+6:10:16+280:10090000:XXXXXXXXXXX:S:0:0'HIRMG:3:2+0010::Nachricht entgegengenommen.'HIRMS:4:2:5+0030::Auftrag empfangen - Sicherheitsfreigabe erforderlich'HITAN:5:6:5+4++g9thgHmp9XkBAACc9QLMh2uowAQA+Ihre TAN wurde an Ihre App ?'Apple iPhone10,4?' gesendet.'HNSHA:6:2+967729067269966''HNHBS:7:1+1'";

            var segments = Helper.SplitEncryptedSegments(message);
            Assert.Equal(7, segments.Count);

            Assert.StartsWith("HNHBK", segments[0]);
            Assert.StartsWith("HNSHK", segments[1]);
            Assert.StartsWith("HIRMG", segments[2]);
            Assert.StartsWith("HIRMS", segments[3]);
            Assert.StartsWith("HITAN", segments[4]);
            Assert.StartsWith("HNSHA", segments[5]);
            Assert.StartsWith("HNHBS", segments[6]);
        }

        [Fact]
        public void TestSplitSegments_3()
        {
            var message = "HNHBK:1:3+000000002856+300+3810064427621000L0v=y9JIeV.B11+1+3810064427621000L0v=y9JIeV.B11:1'HNVSK:998:3+PIN:1+998+1+2::380924516911200017E7AI9RVLHYSY+1:20210926:011347+2:2:13:@8@        :5:1+280:44010046:XXXXXXXX:V:0:0+0'HNVSD:999:1+@2602@HIRMG:2:2+9050::Teilweise fehlerhaft.'HIRMS:3:2:3+0020::Information fehlerfrei entgegengenommen.'HIRMS:4:2:4+1040::BPD nicht mehr aktuell. Aktuelle Version folgt.+3920::Meldung unterstützter Ein- und Zwei-Schritt-Verfahren:920:930'HIRMS:5:2:5+9964::Der Auftrag kann nicht mit dem gewählten Seal One legitimiert werden.'HIBPA:6:3:4+15+280:10010010+Postbank Dortmund+0+1+300+9999'HIKOM:7:4:4+280:10010010+1+3:https?://hbci.postbank.de/banking/hbci.do::MIM:1'HIPINS:8:1:4+1+1+0+5:50:6:::HKCCS:J:HKKAZ:N:HKKAU:N:HKTAN:N:HKCDL:J:HKCSL:J:HKTAB:N:HKCDB:N:HKCSB:N:HKDMC:J:HKCSE:J:DKPSA:J:HKCDE:J:HKDSC:J:HKCCM:J:HKCMB:N:HKSAL:N:HKCML:J:HKCME:J:HKBME:J:HKEKA:N:HKSPA:N:HKPAE:J:HKPRO:N:HKDME:J:HKPSA:J:DKPAE:J:HKCDN:J'DIPINS:9:1:4+1+1+HKCCS:J:HKKAZ:N:HKKAU:N:HKTAN:N:HKCDL:J:HKCSL:J:HKTAB:N:HKCDB:N:HKCSB:N:HKDMC:J:HKCSE:J:DKPSA:J:HKCDE:J:HKDSC:J:HKCCM:J:HKCMB:N:HKSAL:N:HKCML:J:HKCME:J:HKBME:J:HKEKA:N:HKSPA:N:HKPRO:N:HKDME:J:DKPAE:J:HKCDN:J'HIPAES:10:1:4+1+1+0'DIPAES:11:1:4+1+1'HIPSAS:12:1:4+1+1+0'DIPSAS:13:1:4+1+1'HITANS:14:6:4+1+1+0+N:N:0:910:2:HHD1.3.2OPT:HHDOPT1:1.3.2:chipTAN optisch HHD1.3.2:6:1:Challenge:999:N:1:N:0:2:N:J:00:2:N:9:911:2:HHD1.3.2:HHD:1.3.2:chipTAN manuell HHD1.3.2:6:1:Challenge:999:N:1:N:0:2:N:J:00:2:N:9:912:2:HHD1.4OPT:HHDOPT1:1.4:chipTAN optisch HHD1.4:6:1:Challenge:999:N:1:N:0:2:N:J:00:2:N:9:913:2:HHD1.4:HHD:1.4:chipTAN manuell HHD1.4:6:1:Challenge:999:N:1:N:0:2:N:J:00:2:N:9:920:2:BestSign:BestSign::BestSign:6:2:BestSign:999:N:1:N:0:2:N:J:00:2:N:9:930:2:mobileTAN:mobileTAN::mobileTAN:6:2:mobileTAN:999:N:1:N:0:2:N:J:00:2:N:9'HITABS:15:2:4+1+1+0'HITABS:16:4:4+1+1+0'HIPROS:17:3:4+1+1'HISPAS:18:1:4+1+1+0+J:N:J:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.003.03:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.003.02:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.001.02'HIKAZS:19:5:4+1+1+90:N:N'HIKAZS:20:6:4+1+1+0+90:N:N'HISALS:21:5:4+1+1'HISALS:22:6:4+1+1+0'HIEKAS:23:3:4+1+1+0+J:N:N:3'HIKAUS:24:1:4+1+1+0'HICCSS:25:1:4+1+1+0'HICSES:26:1:4+1+1+0+0:180'HICSBS:27:1:4+1+1+1+N:J'HICSLS:28:1:4+1+1+1+J'HICDES:29:1:4+1+1+1+4:1:180:00:00:00:12345'HICDBS:30:1:4+1+1+1+N'HICDNS:31:1:4+1+1+1+0:1:180:J:J:J:J:J:J:J:J:J:00:00:00:12345'HICDLS:32:1:4+1+1+1+1:1:N:J'HICCMS:33:1:4+1+1+0+1000:J:J'HICMES:34:1:4+1+1+0+1:180:1000:J:J'HICMBS:35:1:4+1+1+1+N:J'HICMLS:36:1:4+1+1+1'HIDMES:37:1:4+1+1+0+1:30:1:30:1000:J:J'HIBMES:38:1:4+1+1+0+1:30:1:30:1000:J:J'HIDSCS:39:1:4+1+1+1+1:30:1:30::urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.003.02'HIDMCS:40:1:4+1+1+1+1000:J:J:1:30:1:30::urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.003.02''HNHBS:41:1+1'";

            var segments = Helper.Parse_Segments(new FinTsClient(new ConnectionDetails()), message);
            Assert.Equal("HIBPA:6:3:4+15+280:10010010+Postbank Dortmund+0+1+300+9999'HIKOM:7:4:4+280:10010010+1+3:https?://hbci.postbank.de/banking/hbci.do::MIM:1'HIPINS:8:1:4+1+1+0+5:50:6:::HKCCS:J:HKKAZ:N:HKKAU:N:HKTAN:N:HKCDL:J:HKCSL:J:HKTAB:N:HKCDB:N:HKCSB:N:HKDMC:J:HKCSE:J:DKPSA:J:HKCDE:J:HKDSC:J:HKCCM:J:HKCMB:N:HKSAL:N:HKCML:J:HKCME:J:HKBME:J:HKEKA:N:HKSPA:N:HKPAE:J:HKPRO:N:HKDME:J:HKPSA:J:DKPAE:J:HKCDN:J'DIPINS:9:1:4+1+1+HKCCS:J:HKKAZ:N:HKKAU:N:HKTAN:N:HKCDL:J:HKCSL:J:HKTAB:N:HKCDB:N:HKCSB:N:HKDMC:J:HKCSE:J:DKPSA:J:HKCDE:J:HKDSC:J:HKCCM:J:HKCMB:N:HKSAL:N:HKCML:J:HKCME:J:HKBME:J:HKEKA:N:HKSPA:N:HKPRO:N:HKDME:J:DKPAE:J:HKCDN:J'HIPAES:10:1:4+1+1+0'DIPAES:11:1:4+1+1'HIPSAS:12:1:4+1+1+0'DIPSAS:13:1:4+1+1'HITANS:14:6:4+1+1+0+N:N:0:910:2:HHD1.3.2OPT:HHDOPT1:1.3.2:chipTAN optisch HHD1.3.2:6:1:Challenge:999:N:1:N:0:2:N:J:00:2:N:9:911:2:HHD1.3.2:HHD:1.3.2:chipTAN manuell HHD1.3.2:6:1:Challenge:999:N:1:N:0:2:N:J:00:2:N:9:912:2:HHD1.4OPT:HHDOPT1:1.4:chipTAN optisch HHD1.4:6:1:Challenge:999:N:1:N:0:2:N:J:00:2:N:9:913:2:HHD1.4:HHD:1.4:chipTAN manuell HHD1.4:6:1:Challenge:999:N:1:N:0:2:N:J:00:2:N:9:920:2:BestSign:BestSign::BestSign:6:2:BestSign:999:N:1:N:0:2:N:J:00:2:N:9:930:2:mobileTAN:mobileTAN::mobileTAN:6:2:mobileTAN:999:N:1:N:0:2:N:J:00:2:N:9'HITABS:15:2:4+1+1+0'HITABS:16:4:4+1+1+0'HIPROS:17:3:4+1+1'HISPAS:18:1:4+1+1+0+J:N:J:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.003.03:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.001.001.03:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.003.02:urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.001.02'HIKAZS:19:5:4+1+1+90:N:N'HIKAZS:20:6:4+1+1+0+90:N:N'HISALS:21:5:4+1+1'HISALS:22:6:4+1+1+0'HIEKAS:23:3:4+1+1+0+J:N:N:3'HIKAUS:24:1:4+1+1+0'HICCSS:25:1:4+1+1+0'HICSES:26:1:4+1+1+0+0:180'HICSBS:27:1:4+1+1+1+N:J'HICSLS:28:1:4+1+1+1+J'HICDES:29:1:4+1+1+1+4:1:180:00:00:00:12345'HICDBS:30:1:4+1+1+1+N'HICDNS:31:1:4+1+1+1+0:1:180:J:J:J:J:J:J:J:J:J:00:00:00:12345'HICDLS:32:1:4+1+1+1+1:1:N:J'HICCMS:33:1:4+1+1+0+1000:J:J'HICMES:34:1:4+1+1+0+1:180:1000:J:J'HICMBS:35:1:4+1+1+1+N:J'HICMLS:36:1:4+1+1+1'HIDMES:37:1:4+1+1+0+1:30:1:30:1000:J:J'HIBMES:38:1:4+1+1+0+1:30:1:30:1000:J:J'HIDSCS:39:1:4+1+1+1+1:30:1:30::urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.003.02'HIDMCS:40:1:4+1+1+1+1000:J:J:1:30:1:30::urn?:iso?:std?:iso?:20022?:tech?:xsd?:pain.008.003.02'", BPD.Value);
        }

        [Fact]
        public void Test_Parse_Message_1()
        {
            var message =
@"HNHBK:1:3+000000000593+300+578550834952=734365254762CQCJ=+3+578550834952=734365254762CQCJ=:3'
HNVSK:998:3+PIN:2+998+1+2::3pbWX5WFOHoBAABV8lpvtusWrAQA+1:20210623:132245+2:2:13:@8@00000000:5:1+280:10050000:XXXXXXXXX:V:0:0+0'
HNVSD:999:1+@342@HNSHK:2:4+PIN:2+922+210042763957027+1+1+2::3pbWX5WFOHoBAABV8lpvtusWrAQA+1+1:20210623:132245+1:999:1+6:10:16+280:10050000:XXXXXXXXX:S:0:0'
HIRMG:3:2+3060::Bitte beachten Sie die enthaltenen Warnungen/Hinweise.'
HIRMS:4:2:3+3956::Starke Kundenauthentifizierung noch ausstehend.'
HITAN:5:7:3+S++8578-06-23-13.22.43.709351'
HNSHA:6:2+210042763957027''
HNHBS:7:1+3'".Replace(Environment.NewLine, string.Empty);

            var client = new FinTsClient(new ConnectionDetails());
            Helper.Parse_Message(client, message);

            Assert.Equal(4, client.HNHBS);
            Assert.Equal("8578-06-23-13.22.43.709351", client.HITAN);
        }

        [Fact]
        public void Test_Parse_Message_2()
        {
            var message =
@"HNHBK:1:3+000000000717+300+IA1111613545886+2+IA1111613545886:2'
HNVSK:998:3+PIN:2+998+1+2::VsiR2m3NKH0BAABfF?+evhW?+owAQA+1:20211116:135459+2:2:13:@8@00000000:5:1+280:60090100:XXXXXXXXX:V:0:0+0'
HNVSD:999:1+@494@
HNSHK:2:4+PIN:2+944+1766933014065220+1+1+2::VsiR2m3NKH0BAABfF?+evhW?+owAQA+1+1:20211116:135459+1:999:1+6:10:16+280:60090100:XXXXXXXXX:S:0:0'
HIRMG:3:2+0010::Nachricht entgegengenommen.'
HIRMS:4:2:4+0030::Auftrag empfangen - Sicherheitsfreigabe erforderlich'
HITAN:5:6:4+4++76ma3j/MKH0BAABsRcJNhG?+owAQA+Eine neue TAN steht zur Abholung bereit.  Die TAN wurde reserviert am  16.11.2021 um 13?:54?:59 Uhr. Eine Push-Nachricht wurde versandt.  Bitte geben Sie die TAN ein.'
HNSHA:6:2+1766933014065220''
HNHBS:7:1+2'".Replace(Environment.NewLine, string.Empty);

            var client = new FinTsClient(new ConnectionDetails());
            Helper.Parse_Message(client, message);

            Assert.Equal(3, client.HNHBS);
            Assert.Equal("76ma3j/MKH0BAABsRcJNhG?+owAQA", client.HITAN);
        }

        [Fact]
        public void Test_Parse_Segments_HITAN_1()
        {
            var message =
@"HNHBK:1:3+000000000715+300+KB1090613095412+2+KB1090613095412:2'
HNVSK:998:3+PIN:2+998+1+2::3Wekj53GunsBAABbA19vhW?+owAQA+1:20210906:130955+2:2:13:@8@00000000:5:1+280:60090100:XXXXXXXXX:V:0:0+0'
HNVSD:999:1+@493@HNSHK:2:4+PIN:2+944+1876690780307344+1+1+2::3Wekj53GunsBAABbA19vhW?+owAQA+1+1:20210906:130955+1:999:1+6:10:16+280:60090100:XXXXXXXXX:S:0:0'
HIRMG:3:2+0010::Nachricht entgegengenommen.'
HIRMS:4:2:4+0030::Auftrag empfangen - Sicherheitsfreigabe erforderlich'
HITAN:5:6:4+4++eBYcsEe0unsBAAAXXkX5hG?+owAQA+Eine neue TAN steht zur Abholung bereit.  Die TAN wurde reserviert am  06.09.2021 um 13?:09?:55 Uhr. Eine Push-Nachricht wurde versandt.  Bitte geben Sie die TAN ein.'
HNSHA:6:2+1876690780307344''
HNHBS:7:1+2'".Replace(Environment.NewLine, string.Empty);

            FinTsClient client = new FinTsClient(null);
            Helper.Parse_Segments(client, message);

            Assert.Equal("eBYcsEe0unsBAAAXXkX5hG?+owAQA", client.HITAN);
        }

        [Fact]
        public void Test_Parse_Segments_HITAN_2()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\BankMessage_3.txt");
            var message = File.ReadAllText(path);
            var conn = new ConnectionDetails();
            conn.Blz = 1234567;
            FinTsClient client = new FinTsClient(conn);
            Helper.Parse_Segments(client, message);

            Assert.Equal("eNmo9/2dEocBAACRO?+gjhW?+owAQA", client.HITAN);
        }

        [Fact]
        public void Test_Parse_Segments_HISALS()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\BankMessage_1.txt");
            var message = File.ReadAllText(path);
            var conn = new ConnectionDetails();
            conn.Blz = 1234567;
            FinTsClient client = new FinTsClient(conn);
            Helper.Parse_Segments(client, message);

            Assert.Equal(7, client.HISALS);
        }

        [Fact]
        public void Test_Parse_Segments_HNHBK()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\BankMessage_2.txt");
            var message = File.ReadAllText(path);
            var conn = new ConnectionDetails();
            conn.Blz = 1234567;
            FinTsClient client = new FinTsClient(conn);
            Helper.Parse_Segments(client, message);

            Assert.Equal("#3xufqdASFyM12120221127131048%", client.HNHBK);
        }

        [Fact]
        public void Test_Parse_Transactions_Startpoint()
        {
            var message = @"HIRMS:4:2:3+3040::*Es liegen noch weitere CAMT Umsätze vor:0_=2#=20210727#=12080533#=0#=0#=0+0900::Freigabe erfolgreich'";
            var startpoint = Helper.Parse_Transactions_Startpoint(message);

            Assert.Equal("0_=2#=20210727#=12080533#=0#=0#=0", startpoint);

            message = @"HIRMS:4:2:3+0020::Der Auftrag wurde ausgeführt.+0020::Die gebuchten Umsätze wurden übermittelt.+3040::Es liegen weitere Informationen vor.:7587-01-13-11.32.26.675878'";
            startpoint = Helper.Parse_Transactions_Startpoint(message);

            Assert.Equal("7587-01-13-11.32.26.675878", startpoint);
        }
    }
}
