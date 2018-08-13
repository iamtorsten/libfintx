using System;
using System.IO;
using System.Xml.Serialization;
using libfintx;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace libfintx_test
{
    [TestClass]
    public class Test_pain_001_001_03
    {
        [TestMethod]
        public void Test_BankersOrders_1()
        {
            libfintx.pain_001_001_03.Document xml;
            XmlSerializer ser = new XmlSerializer(typeof(libfintx.pain_001_001_03.Document), new XmlRootAttribute
            {
                ElementName = "Document",
                Namespace = "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03",
            });

            using (var fileStream = new FileStream("Resources/get-bankers-orders-1.xml", FileMode.Open))
            {
                xml = (libfintx.pain_001_001_03.Document)ser.Deserialize(fileStream);
            }

            libfintx.pain00100103_ct_data data = libfintx.pain00100103_ct_data.Create(xml);

            Assert.AreEqual(1, data.NumberOfTransactions);
            Assert.AreEqual(100.00m, data.ControlSum);
        }
    }
}
