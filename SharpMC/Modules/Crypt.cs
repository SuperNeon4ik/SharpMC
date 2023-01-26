using System;
using System.Text;
using java.security;
using javax.crypto;

namespace SharpMC.Modules;

public static class Crypt
{
    // private const string SYMMETRIC_ALGORITHM = "AES";
    // private const int SYMMETRIC_BITS = 128;
    // private const string ASYMMETRIC_ALGORITHM = "RSA";
    // private const int ASYMMETRIC_BITS = 1024;
    // private const string BYTE_ENCODING = "ISO_8859_1";
    // private const string HASH_ALGORITHM = "SHA-1";
    //
    // public static SecretKey generateSecretKey() {
    //     try {
    //         KeyGenerator var0 = KeyGenerator.getInstance("AES");
    //         var0.init(128);
    //         return var0.generateKey();
    //     } catch (Exception var1) {
    //         throw new CryptException(var1);
    //     }
    // }
    //
    // public static KeyPair generateKeyPair() {
    //     try {
    //         KeyPairGenerator var0 = KeyPairGenerator.getInstance("RSA");
    //         var0.initialize(1024);
    //         return var0.generateKeyPair();
    //     } catch (Exception var1) {
    //         throw new CryptException(var1);
    //     }
    // }
    //
    // public static byte[] digestData(string var0, PublicKey var1, SecretKey var2) {
    //     try {
    //         return digestData(Encoding.Default.GetBytes(var0), var2.getEncoded(), var1.getEncoded());
    //     } catch (Exception var4) {
    //         throw new CryptException(var4);
    //     }
    // }
    //
    // private static byte[] digestData(byte[] var0) {
    //     MessageDigest var1 = MessageDigest.getInstance("SHA-1");
    //     byte[][] var2 = var0;
    //     int var3 = var0.length;
    //
    //     for(int var4 = 0; var4 < var3; ++var4) {
    //         byte[] var5 = var2[var4];
    //         var1.update(var5);
    //     }
    //
    //     return var1.digest();
    // }
    //
    // public static PublicKey byteToPublicKey(byte[] var0) {
    //     try {
    //         EncodedKeySpec var1 = new X509EncodedKeySpec(var0);
    //         KeyFactory var2 = KeyFactory.getInstance("RSA");
    //         return var2.generatePublic(var1);
    //     } catch (Exception var3) {
    //         throw new CryptException(var3);
    //     }
    // }
    //
    // public static SecretKey decryptByteToSecretKey(PrivateKey var0, byte[] var1) {
    //     byte[] var2 = decryptUsingKey(var0, var1);
    //
    //     try {
    //         return new SecretKeySpec(var2, "AES");
    //     } catch (Exception var4) {
    //         throw new CryptException(var4);
    //     }
    // }
    //
    // public static byte[] encryptUsingKey(Key var0, byte[] var1) {
    //     return cipherData(1, var0, var1);
    // }
    //
    // public static byte[] decryptUsingKey(Key var0, byte[] var1) {
    //     return cipherData(2, var0, var1);
    // }
    //
    // private static byte[] cipherData(int var0, Key var1, byte[] var2) {
    //     try {
    //         return setupCipher(var0, var1.getAlgorithm(), var1).doFinal(var2);
    //     } catch (Exception var4) {
    //         throw new CryptException(var4);
    //     }
    // }
    //
    // private static Cipher setupCipher(int var0, String var1, Key var2) {
    //     Cipher var3 = Cipher.getInstance(var1);
    //     var3.init(var0, var2);
    //     return var3;
    // }
    //
    // public static Cipher getCipher(int var0, Key var1)  {
    //     try {
    //         Cipher var2 = Cipher.getInstance("AES/CFB8/NoPadding");
    //         var2.init(var0, var1, new IvParameterSpec(var1.getEncoded()));
    //         return var2;
    //     } catch (Exception var3) {
    //         throw new CryptException(var3);
    //     }
    // }
}