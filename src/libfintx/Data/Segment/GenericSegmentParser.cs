using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx.Data.Segment
{
    public class GenericSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            var match = Regex.Match(segment.Value, @"^(?<Name>[A-Z]+):(?<Number>\d+):(?<Version>\d+)(:(?<Ref>\d+))?\+(?<Payload>.*?)$");
            if (!match.Success)
                throw new ArgumentException($"Segment has invalid format.{Environment.NewLine}{segment.Value}");

            segment.Name = match.Groups["Name"].Value;
            segment.Number = Convert.ToInt32(match.Groups["Number"].Value);
            segment.Version = Convert.ToInt32(match.Groups["Version"].Value);
            if (match.Groups["Ref"].Success)
                segment.Ref = Convert.ToInt32(match.Groups["Ref"].Value);
            segment.Payload = match.Groups["Payload"].Value;

            return segment;
        }
    }
}
