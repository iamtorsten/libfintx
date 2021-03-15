using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx.Data.Segment
{
    public class Segment
    {
        public string Value { get; set; }

        public string Name { get; set; }

        public int Number { get; set; }

        public int Version { get; set; }

        public int? Ref { get; set; }

        public string Payload { get; set; }

        public Segment(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Segment segment &&
                   Name == segment.Name &&
                   Number == segment.Number &&
                   Version == segment.Version;
        }

        public override int GetHashCode()
        {
            var hashCode = 1663882333;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            hashCode = hashCode * -1521134295 + Version.GetHashCode();
            return hashCode;
        }
    }
}
