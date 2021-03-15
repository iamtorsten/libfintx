using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx.Data.Segment
{
    internal class HICAZSSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            var result = new HICAZS(segment);

            // HICAZS:92:1:4+1+1+1+450:N:N:urn?:iso?:std?:iso?:20022?:tech?:xsd?:camt.052.001.02
            var match = Regex.Match(segment.Payload, @"^(?<maxanzauftr>\d*)\+(?<minanzsig>\d*)\+(?<sicherheitskl>\d*)\+(?<zeitraum>\d*):(?<maxeintr>[J|N]{0,1}):(?<allekt>[J|N]{0,1}):(?<formate>.*)");
            if (!match.Success)
                throw new ArgumentException($"Could not parse segment{Environment.NewLine}{segment.Payload}");

            var zeitraum = match.Groups["zeitraum"].Value;
            if (zeitraum != null)
                result.Zeitraum = Convert.ToInt32(zeitraum);

            result.Format = match.Groups["formate"].Value;

            return result;
        }
    }
}
