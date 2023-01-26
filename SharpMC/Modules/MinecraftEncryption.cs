using System;
using System.Text;
using System.Security.Cryptography;
using System.Numerics;

namespace SharpMC.Modules;

public static class MinecraftEncryption
{
    public static string MinecraftShaDigest(string input) 
    {
        var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
        // Reverse the bytes since BigInteger uses little endian
        Array.Reverse(hash);
        
        BigInteger b = new BigInteger(hash);
        // very annoyingly, BigInteger in C# tries to be smart and puts in
        // a leading 0 when formatting as a hex number to allow roundtripping 
        // of negative numbers, thus we have to trim it off.
        if (b < 0)
        {
            // toss in a negative sign if the interpreted number is negative
            return "-" + (-b).ToString("x").TrimStart('0');
        }
        else
        {
            return b.ToString("x").TrimStart('0');
        }
    }

    public static string ToPublicKey(byte[] key)
    {
        string final = "-----BEGIN PUBLIC KEY-----";
        final += Convert.ToBase64String(key);
        final += "-----END PUBLIC KEY-----";
        return final;
    }
}