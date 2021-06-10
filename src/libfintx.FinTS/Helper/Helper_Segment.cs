/*	
 * 	
 *  This file is part of libfintx.
 *  
 *  Copyright (C) 2016 - 2021 Torsten Klinger
 * 	E-Mail: torsten.klinger@googlemail.com
 *  
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 3 of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software Foundation,
 *  Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 * 	
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace libfintx.FinTS
{
    /// <summary>
    /// Segment processing.
    /// </summary>
    public static partial class Helper
    {
        private static string ProcessSegmentBegin(string message, StringBuilder currentSegment)
        {
            var match = Regex.Match(message, @"^[A-Z]+:\d+:\d+(:\d+)?\+");
            if (!match.Success)
                throw new ArgumentException($"Invalid segment. Expected segment begin. Message is: {Truncate(message)}");

            currentSegment.Append(match.Value);
            return message.Substring(match.Index + match.Length);
        }

        private static DelimiterMatch FindNextDelimiter(string message)
        {
            var escapeMatch = Regex.Match(message, @"\?");
            var binaryMatch = Regex.Match(message, @"@\d+@");
            var segmentEndMatch = Regex.Match(message, @"'");

            var result = new Match[] { escapeMatch, binaryMatch, segmentEndMatch }.Where(m => m.Success).OrderBy(m => m.Index).FirstOrDefault();

            if (result == escapeMatch)
                return new DelimiterMatch(Delimiter.Escape, result.Index, result.Value);
            if (result == binaryMatch)
                return new DelimiterMatch(Delimiter.Binary, result.Index, result.Value);
            if (result == segmentEndMatch)
                return new DelimiterMatch(Delimiter.Segment, result.Index, result.Value);

            return null;
        }

        /// <summary>
        /// Splits segments into strings. Inspired from https://github.com/nemiah/phpFinTS/blob/fce921b5311ee3c15eb075c25627a0584c6bee99/lib/Fhp/Syntax/Parser.php
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal static List<string> SplitSegments(string message)
        {
            List<string> segments = new List<string>();

            StringBuilder currentSegment = new StringBuilder();
            message = ProcessSegmentBegin(message, currentSegment);

            while (message.Length > 0)
            {
                var delimiter = FindNextDelimiter(message);
                if (delimiter == null)
                    throw new ArgumentException($"Invalid segment. Didn't find any delimiter. Message is: {Truncate(message)}");

                switch (delimiter.Delimiter)
                {
                    case Delimiter.Escape:
                        {
                            var length = delimiter.Index + delimiter.Value.Length + 1;
                            if (message.Length < length)
                                throw new ArgumentException($"Invalid segment. There are no more characters after escape character. Message is: {Truncate(message)}");

                            // Escape character must be followed by +,:,',?,@
                            var allowedChars = new[] { '+', ':', '\'', '?', '@' };
                            if (!allowedChars.Contains(message[length - 1]))
                                throw new ArgumentException($"Invalid segment. Escape character must be followed by +,:,',?,@. Message is: {Truncate(message)}");

                            currentSegment.Append(message.Substring(0, length));
                            message = message.Substring(length);

                            break;
                        }
                    case Delimiter.Binary:
                        {
                            var lengthStr = delimiter.Value.Substring(1, delimiter.Value.Length - 2);
                            var binaryLength = Convert.ToInt32(lengthStr);
                            var length = delimiter.Index + delimiter.Value.Length + binaryLength;
                            if (message.Length < length)
                                throw new ArgumentException($"Invalid segment. Given binary length {binaryLength} exceeds actual message length. Message is: {Truncate(message)}");

                            currentSegment.Append(message.Substring(0, length));
                            message = message.Substring(length);

                            // binary data must be followed by +,:,'
                            var allowedChars = new[] { '+', ':', '\'' };
                            if (!allowedChars.Contains(message[0]))
                                throw new ArgumentException($"Invalid segment. Binary data must be followed by +,:,'. Message is: {Truncate(message)}");

                            break;
                        }
                    case Delimiter.Segment:
                        {
                            currentSegment.Append(message.Substring(0, delimiter.Index));
                            segments.Add(currentSegment.ToString());
                            currentSegment = new StringBuilder();

                            message = message.Substring(delimiter.Index + delimiter.Value.Length);
                            if (message.Length > 0)
                                message = ProcessSegmentBegin(message, currentSegment);

                            break;
                        }
                    default:
                        break;
                }

            }

            return segments;
        }


        /// <summary>
        /// Decrypts segments. This means that encrypted data is decrypted and signature header (HNVSK) and signature end (HNVSD) are omitted.
        /// </summary>
        /// <param name="encryptedSegments"></param>
        /// <returns></returns>
        private static List<string> DecryptSegments(List<string> encryptedSegments)
        {
            var sequence = new[] { "HNHBK", "HNVSK", "HNVSD", "HNHBS" };
            for (int i = 0; i < sequence.Length; i++)
            {
                if (encryptedSegments.Count - 1 < i || !encryptedSegments[i].StartsWith(sequence[i]))
                    throw new ArgumentException("Expected exactly 4 segments (HNHBK, HNVSK, HNVSD, HNHBS) to decode.");
            }

            List<string> segments = new List<string>();
            segments.Add(encryptedSegments.First()); // HNHBK

            // Extract segments from HNVSD
            var hnvsd = encryptedSegments[2];
            var match = Regex.Match(hnvsd, @"HNVSD:999:1\+@\d+@");
            if (!match.Success)
                throw new ArgumentException("Invalid HNVSD segment.");
            var binaryData = hnvsd.Substring(match.Length);
            var decodedSegments = SplitSegments(binaryData);
            segments.AddRange(decodedSegments);

            segments.Add(encryptedSegments.Last()); // HNHBS

            return segments;
        }

        internal static List<string> SplitEncryptedSegments(string message)
        {
            var encodedSegments = SplitSegments(message);
            var decodedSegments = DecryptSegments(encodedSegments);

            return decodedSegments;
        }

        private static string Truncate(string s, int length = 15)
        {
            if (s == null)
                return null;

            if (s.Length < length)
                return s;

            return s.Substring(0, length) + "...";
        }
    }

    internal enum Delimiter
    {
        Escape,
        Binary,
        Segment
    }

    internal class DelimiterMatch
    {
        internal Delimiter Delimiter { get; }

        internal int Index { get; }

        internal string Value { get; }

        internal bool IsEscape => Delimiter == Delimiter.Escape;

        internal bool IsBinary => Delimiter == Delimiter.Binary;

        internal bool IsSegmentEnd => Delimiter == Delimiter.Segment;

        internal DelimiterMatch(Delimiter delimiter, int index, string value)
        {
            Delimiter = delimiter;
            Index = index;
            Value = value;
        }

        public override string ToString()
        {
            return $"Delimiter: {Delimiter}, Index: {Index}, Value: {Value}";
        }
    }
}
