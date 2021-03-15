using System.Collections.Generic;
using System.Text.RegularExpressions;
using libfintx.Data.Segment;

namespace libfintx
{
    public class HIPINS : SegmentBase
    {
        public Dictionary<string, bool> GvPinTan { get; set; }

        public HIPINS(Segment segment) : base(segment)
        {
            GvPinTan = new Dictionary<string, bool>();
        }

        public bool IsTanRequired(string gvName)
        {
            return GvPinTan.ContainsKey(gvName) ? GvPinTan[gvName] : false;
        }
    }
}
