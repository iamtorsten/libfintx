using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using libfintx.FinTS.Camt.Camt052;
using Xunit;

namespace libfintx.Tests.FinTS.Camt.Camt052
{
    public class Camt052ParserTest
    {
        [Fact]
        public void Test_Hypovereinsbank_052_001_08()
        {
            Camt052Parser parser = new Camt052Parser();
            string xml = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Camt052_Hypo_Vereinsbank.txt"));
            xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + xml;
            parser.ProcessDocument(xml, Encoding.GetEncoding("ISO-8859-1"));
            Assert.NotEmpty(parser.statements);
        }

        [Fact]
        public void Test_Spardabank_052_001_02()
        {
            Camt052Parser parser = new Camt052Parser();
            string xml = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Camt052_Spardabank.txt"));
            xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + xml;
            parser.ProcessDocument(xml, Encoding.GetEncoding("ISO-8859-1"));
            Assert.NotEmpty(parser.statements);
        }
    }
}
