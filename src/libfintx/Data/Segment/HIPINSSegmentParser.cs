using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx.Data.Segment
{
    public class HIPINSSegmentParser : ISegmentParser
    {
        public Segment ParseSegment(Segment segment)
        {
            HIPINS result = new HIPINS(segment);

            // HIPINS:170:1:4+1+1+0+5:5:6:USERID:CUSTID:HKAUB:J:
            // HIPINS:78:1:4+1+1+0+5:20:6:VR-NetKey oder Alias::HKTAN:N:
            var match = Regex.Match(segment.Payload, @"^(\d*)\+(\d*)\+(\d*)\+(\d*):(\d*):(\d*):(?<belegungbenutzerkennung>.*?):(?<belegungkundenid>.*?):(?<gvlist>.*)$");

            if (match.Success)
            {
                var gvList = match.Groups["gvlist"].Value;
                foreach (Match gvMatch in Regex.Matches(gvList, @"(?<gv>[A-Z]+):(?<tanrequired>J|N)"))
                {
                    var gv = gvMatch.Groups["gv"].Value;
                    var tanRequired = gvMatch.Groups["tanrequired"].Value;

                    result.GvPinTan[gv] = (tanRequired == "J");
                }
            }

            return result;
        }
    }
}
