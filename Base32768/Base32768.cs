﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
/*
Base32768 is a binary-to-text encoding optimised for UTF-16-encoded text.
(e.g. Windows, Java, JavaScript)

original is https://github.com/qntm/base32768

MIT License

Copyright (c) 2020 naminodarie

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
namespace Kzrnm.Convert.Base32768
{
    /// <summary>
    /// Base32768 is a binary-to-text encoding optimised for UTF-16-encoded text.
    /// </summary>
    public static partial class Base32768
    {
        internal const int BITS_PER_CHAR = 15;// Base32768 is a 15-bit encoding
        internal const int BITS_PER_BYTE = 8;

        internal const int LookupUpperBound = 32768;
        internal const char ZBits15Start = '\u04a0';

        internal static readonly ushort[] lookupD = FilledArray(new ushort[0xA860], ushort.MaxValue);
        internal static readonly char[] lookupE7 = BuildLookupE(
            "\u0180\u01a0" + "\u0240\u02a0"
        , 128);
        internal static readonly char[] lookupE15 = BuildLookupE(
            "\u04a0\u04c0" + "\u0500\u0520" + "\u0680\u06c0" + "\u0760\u07a0" +
            "\u07c0\u07e0" + "\u1000\u1020" + "\u10a0\u10c0" + "\u1100\u1160" +
            "\u1180\u11a0" + "\u11e0\u1240" + "\u1260\u1280" + "\u12e0\u1300" +
            "\u1320\u1340" + "\u13a0\u13e0" + "\u1420\u1660" + "\u16a0\u16e0" +
            "\u1780\u17a0" + "\u1820\u1860" + "\u18c0\u18e0" + "\u1980\u19a0" +
            "\u19e0\u1a00" + "\u1a20\u1a40" + "\u1bc0\u1be0" + "\u1c00\u1c20" +
            "\u1d00\u1d20" + "\u21e0\u2200" + "\u22c0\u22e0" + "\u2340\u23e0" +
            "\u2400\u2420" + "\u2500\u2760" + "\u2780\u27c0" + "\u2800\u2980" +
            "\u29a0\u29c0" + "\u2a20\u2a60" + "\u2a80\u2ac0" + "\u2ae0\u2b60" +
            "\u2c00\u2c20" + "\u2c80\u2ce0" + "\u2d00\u2d20" + "\u2d40\u2d60" +
            "\u2ea0\u2ee0" + "\u31c0\u31e0" + "\u3400\u4da0" + "\u4dc0\u9fc0" +
            "\ua000\ua480" + "\ua4a0\ua4c0" + "\ua500\ua600" + "\ua640\ua660" +
            "\ua6a0\ua6e0" + "\ua700\ua760" + "\ua780\ua7a0" + "\ua840\ua860"
        , 32768);

#if NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static ushort Validate15BitCharAndLookupD(char c)
        {
            if (c >= ZBits15Start)
            {
                if (c < (uint)lookupD.Length)
                {
                    ushort z = lookupD[c];
                    if (z < LookupUpperBound)
                    {
                        return z;
                    }
                }
            }
            ThrowFormatException(c);
            return default;
        }

#if NETSTANDARD1_0_OR_GREATER || NET45_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static ushort ValidateCharAndLookupD(char c, out bool is7BitChar)
        {
            is7BitChar = c < ZBits15Start;
            if (c < (uint)lookupD.Length)
            {
                ushort z = lookupD[c];
                if (z < LookupUpperBound)
                {
                    return z;
                }
            }
            ThrowFormatException(c);
            return default;
        }

        private static T[] FilledArray<T>(T[] array, T value)
        {
#if NETSTANDARD2_1_OR_GREATER
            Array.Fill(array, value);
#else
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
#endif
            return array;
        }


        private static char[] BuildLookupE(string pairString, int size)
        {
            var encodeRepertoire = new char[size];
            ushort ix = 0;
            for (int p = 0; p < pairString.Length; p += 2)
            {
                var from = pairString[p];
                var to = pairString[p + 1];
                for (char i = from; i < to; i++)
                {
                    lookupD[i] = ix;
                    encodeRepertoire[ix++] = i;
                }
            }
            Debug.Assert(size == ix);
            return encodeRepertoire;
        }

        static void ThrowFormatException(char c)
           => throw new FormatException($"Unrecognised Base32768 character: {c}");
    }
}
