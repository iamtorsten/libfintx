using System;
using System.Text.RegularExpressions;

namespace libfintx
{
    public class HICAZS
    {
        public int? Zeitraum { get; set; }

        public string Format { get; set; }

        public static HICAZS Parse_HICAZS(string hicazs)
        {
            HICAZS result = new HICAZS();

            if (hicazs != null)
            {
                // HICAZS:92:1:4+1+1+1+450:N:N:urn?:iso?:std?:iso?:20022?:tech?:xsd?:camt.052.001.02
                var match = Regex.Match(hicazs, @"HICAZS:(?<nr>\d*):(?<vers>\d*):(?<bezug>\d*)\+(?<maxanzauftr>\d*)\+(?<minanzsig>\d*)\+(?<sicherheitskl>\d*)\+(?<zeitraum>\d*):(?<maxeintr>[J|N]{0,1}):(?<allekt>[J|N]{0,1}):(?<formate>.*)");
                if (match.Success)
                {
                    var zeitraum = match.Groups["zeitraum"].Value;
                    if (zeitraum != null)
                        result.Zeitraum = Convert.ToInt32(zeitraum);

                    result.Format = match.Groups["formate"].Value;
                }
            }

            return result;
        }
    }
}
