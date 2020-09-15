using AJL_ChessEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace AJL_ChessProgram.Classes
{
    static class Hasher
    {
        public static ulong HashComposition(State state)
        {
            var bArr = new byte[64];
            for (int i = 0; i < Constants.xwidth; i++)
            {
                for (int j = 0; j < Constants.ywidth; j++)
                {
                    bArr[i * Constants.xwidth + j] = (byte)state[i, j];
                }
            }
            return ByteConversion.ByteArrayPrime(bArr);
        }



    }

    static class ByteConversion
    {
        public static ulong[] primes = new ulong[]
        {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
            31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
            73, 79, 83, 89, 97, 101, 103, 107, 109, 113,
            127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
            179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
            233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
            283, 293, 307, 311, 313, 317, 331, 337, 347, 349,
            353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
            419, 421, 431, 433, 439, 443, 449, 457, 461, 463,
            467, 479, 487, 491, 499, 503, 509, 521, 523, 541,
            547, 557, 563, 569, 571, 577, 587, 593, 599, 601,
            607, 613, 617, 619, 631, 641, 643, 647, 653, 659
        };

        public static ulong ByteArrayToULong(byte[] bArray)
        {
            ulong temp = 1;
            for (int i = 0; i < bArray.Length; i++)
            {
                temp = temp * (ulong)(bArray[i] + (ulong)i) - (ulong)i;
            }
            return temp - (ulong)bArray.Length;
        }

        //Is collisionfree for game start and 6 plies.
        public static ulong ByteArrayPrime(byte[] bArray)
        {
            ulong retLong = 0;
            for (int i = 0; i < bArray.Length; i++)
            {
                retLong = retLong * primes[i] + bArray[i];
            }
            return retLong;
        }
    }
}
