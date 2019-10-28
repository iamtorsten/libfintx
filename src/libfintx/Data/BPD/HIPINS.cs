using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace libfintx
{
    public class HIPINS
    {
        public Dictionary<string, bool> GV_PIN_TAN { get; set; }

        public HIPINS()
        {
            GV_PIN_TAN = new Dictionary<string, bool>();
        }

        public bool IsTANRequired(string gvName)
        {
            return GV_PIN_TAN.ContainsKey(gvName) ? GV_PIN_TAN[gvName] : false;
        }

        public static HIPINS Parse_HIPINS(string hipins)
        {
            HIPINS result = new HIPINS();

            if (hipins == null)
                return result;

            // HIPINS:170:1:4+1+1+0+5:5:6:USERID:CUSTID:HKAUB:J:
            // HIPINS:78:1:4+1+1+0+5:20:6:VR-NetKey oder Alias::HKTAN:N:
            var match = Regex.Match(hipins, @"HIPINS:(\d*):(\d*):(\d*)\+(\d*)\+(\d*)\+(\d*)\+(\d*):(\d*):(\d*):(?<belegungbenutzerkennung>.*?):(?<belegungkundenid>.*?):(?<gvlist>.*)");
            if (match.Success)
            {
                var gvList = match.Groups["gvlist"].Value;
                foreach (Match gvMatch in Regex.Matches(gvList, @"(?<gv>[A-Z]+):(?<tanrequired>J|N)"))
                {
                    var gv = gvMatch.Groups["gv"].Value;
                    var tanRequired = gvMatch.Groups["tanrequired"].Value;

                    result.GV_PIN_TAN[gv] = (tanRequired == "J");
                }
            }

            return result;
        }
    }
}
