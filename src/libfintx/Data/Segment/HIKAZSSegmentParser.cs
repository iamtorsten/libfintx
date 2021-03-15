using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx.Data.Segment
{
    public class HIKAZSSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            var result = new HIKAZS(segment);

            if (result.Version < 6) // Ignore
                return segment;

            // HIKAZS:23:7:4+20+1+1+90:N:N'
            var match = Regex.Match(segment.Payload, @"^(?<maxanzauftr>\d*)\+(?<minanzsig>\d*)\+(?<sicherheitskl>\d*)\+(?<zeitraum>\d*):(?<maxeintr>[J|N]{0,1}):(?<allekt>[J|N]{0,1}).*");
            if (!match.Success)
                throw new ArgumentException($"Could not parse segment{Environment.NewLine}{segment.Payload}");

            var zeitraum = match.Groups["zeitraum"].Value;
            if (zeitraum != null)
                result.Zeitraum = Convert.ToInt32(zeitraum);

            return result;
        }
    }
}
