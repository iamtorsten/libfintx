using System;
using System.Text.RegularExpressions;

namespace libfintx
{
    public class HITANS
    {
        public int Version { get; set; }

        public int Process { get; set; }

        public static HITANS Parse_HITANS(string hitans)
        {
            HITANS result = new HITANS();

            // HITANS:13:4:4+1+1+0+N:N:0:901:2:mobileTAN:mobileTAN::mobileTAN:6:2:mobileTAN:999:0:N:1:0:N:N:N:N:J:00:2:9:910:2:HHD1.3.2OPT:HHDOPT1:1.3.2:chipTAN optisch HHD1.3.2:6:1:Challenge:999:0:N:1:0:N:N:N:N:J:00:2:9:911:2:HHD1.3.2:HHD:1.3.2:chipTAN manuell HHD1.3.2:6:1:Challenge:999:0:N:1:0:N:N:N:N:J:00:2:9:920:2:BestSign:BestSign::BestSign:6:2:BestSign:999:0:N:1:0:N:N:N:N:J:00:2:9:930:2:mobileTAN:mobileTAN::mobileTAN:6:2:mobileTAN:999:0:N:1:0:N:N:N:N:J:00:2:9
            var match = Regex.Match(hitans, @"HITANS:\d+:(?<version>\d+):(?<process>\d+)");
            if (match.Success)
            {
                try
                {
                    result.Version = Convert.ToInt32(match.Groups["version"].Value);
                }
                catch (Exception ex)
                {
                    Log.Write($"Couldn't read HITANS version: {ex.Message}");
                }
                try
                {
                    result.Process = Convert.ToInt32(match.Groups["process"].Value);
                }
                catch (Exception ex)
                {
                    Log.Write($"Couldn't read HITANS process: {ex.Message}");
                }
            }

            return result;
        }
    }
}
