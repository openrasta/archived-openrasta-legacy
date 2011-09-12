#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;

namespace OpenRasta.Text
{
    /// <summary>
    /// Provides partial implementation for decoding strings according to RFC2047.
    /// </summary>
    /// <remarks>
    /// This implementation is not yet conformant to rfc2047.
    /// </remarks>
    public static class Rfc2047Encoding
    {
        public static string DecodeTextToken(string textToDecode)
        {
            StringBuilder decoded = new StringBuilder();
            StringBuilder charsetBuilder = null;
            StringBuilder encodedText = null;

            for (int i = 0; i < textToDecode.Length; i++)
            {
                char ch = textToDecode[i];

                if (ch == '=' && i < textToDecode.Length - 1 && textToDecode[i + 1] == '?')
                {
                    i += 2;
                    charsetBuilder = new StringBuilder();
                    while (i < textToDecode.Length && textToDecode[i] != '?')
                    {
                        charsetBuilder.Append(textToDecode[i]);
                        i++;
                    }
                    i++;
                    string charset = charsetBuilder.ToString();
                    Encoding textEncoder = null;
                    try
                    {
                        textEncoder = Encoding.GetEncoding(charset);
                    }
                    catch { }
                    char encoding = textToDecode[i];
                    Func<string, Encoding, string> decoder = null;
                    if ((encoding == 'Q' || encoding == 'q') && i + 1 < textToDecode.Length && textToDecode[i + 1] == '?')
                    { decoder = DecodeQuotedPrintable; }
                    else if ((encoding == 'B' || encoding == 'b') && i + 1 < textToDecode.Length && textToDecode[i + 1] == '?')
                    { decoder = DecodeBase64; }

                    if (textEncoder != null && decoder != null)
                    {
                        i += 2;
                        encodedText = new StringBuilder();
                        byte[] encodedBuffer = new byte[4];
                        for (; i + 1 < textToDecode.Length && !(textToDecode[i] == '?' && textToDecode[i + 1] == '='); i++)
                        {
                            encodedText.Append(textToDecode[i]);
                        }
                        decoded.Append(decoder(encodedText.ToString(),textEncoder));
                        i += 1;
                    }
                    else
                    {
                        decoded.Append("=?").Append(charset).Append("?").Append(encoding);
                        continue;
                    }
                }
                else
                    decoded.Append(ch);

            }
            return decoded.ToString();
        }
        private static string DecodeQuotedPrintable(string textToDecode, Encoding textEncoder)
        {
            MemoryStream toDecode = new MemoryStream(textToDecode.Length);

            for (int i = 0; i < textToDecode.Length; i++)
            {
                byte byteToAdd = 0;
                if (textToDecode[i] == '_')
                {
                    byteToAdd = (byte)' ';
                }
                else if (textToDecode[i] == '=' && i + 2 < textToDecode.Length)
                {
                    byteToAdd = byte.Parse(textToDecode[i+1] + "" + textToDecode[i + 2], NumberStyles.HexNumber,CultureInfo.InvariantCulture);
                    i += 2;
                }
                else
                {
                    byteToAdd = (byte)textToDecode[i];
                }
                toDecode.WriteByte(byteToAdd);
            }

            return textEncoder.GetString(toDecode.GetBuffer(), 0, (int)toDecode.Length);
        }
        private static string DecodeBase64(string text, Encoding textEncoder)
        {
            var bytes = Convert.FromBase64String(text);
            return textEncoder.GetString(bytes);
        }
    }
}

#region Full license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
