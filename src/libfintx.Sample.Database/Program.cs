using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libfintx.Database;

namespace libfintx.Sample.Database
{
    class Program
    {
        private static readonly string TestPath = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            // Einfache Datenbank erstellen
            var odbf = new DbfFile(Encoding.GetEncoding(1252));
            odbf.Open(Path.Combine(TestPath, "libfintx.dbf"), FileMode.Create);

            // Header erstellen
            odbf.Header.AddColumn(new DbfColumn("StrCol", DbfColumn.DbfColumnType.Character, 20, 0));
            odbf.Header.AddColumn(new DbfColumn("DecCol1", DbfColumn.DbfColumnType.Number, 5, 1));
            odbf.Header.AddColumn(new DbfColumn("DecCol2", DbfColumn.DbfColumnType.Number, 5, 2));
            odbf.Header.AddColumn(new DbfColumn("DecCol3", DbfColumn.DbfColumnType.Number, 5, 3));
            odbf.Header.AddColumn(new DbfColumn("DecCol4", DbfColumn.DbfColumnType.Number, 15, 5));
            odbf.Header.AddColumn(new DbfColumn("NumCol1", DbfColumn.DbfColumnType.Number, 5, 0));
            odbf.Header.AddColumn(new DbfColumn("NumCol2", DbfColumn.DbfColumnType.Number, 10, 0));
            odbf.Header.AddColumn(new DbfColumn("DateCol1", DbfColumn.DbfColumnType.Date));
            odbf.Header.AddColumn(new DbfColumn("BoolCol1", DbfColumn.DbfColumnType.Boolean));

            // Datensatz hinzufügen
            var orec = new DbfRecord(odbf.Header) { AllowDecimalTruncate = true };
            orec[0] = "Torsten Klinger";
            orec[1] = "123.5";
            orec[2] = "12.35";
            orec[3] = "1.235";
            orec[4] = "1235.123456";
            orec[5] = "1235";
            orec[6] = "123567890";
            orec[7] = "11/07/2020";
            orec[8] = "f";
            odbf.Write(orec, true);

            // Verbindung schließen
            odbf.Close();
        }
    }
}
