using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

public class AuthentificationScript
{
    // Will be sent to server in encrypted form
    public struct AuthenticationPacket
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string username;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string password;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string randomHexValue; // Has to be the same to work

        public long timestamp;
    };

    // Sent to client as JSON
    [Serializable]
    public struct ChallengePacket
    {
        public RSAParameters publicKey;
        public string randomHexValue; // Has to be the same to work
        public long timestamp;
    }

    private static RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();

    public static byte[] encryptData (string username, string password, string randomHex, long timestamp, RSAParameters publicKey)
    {
        AuthenticationPacket packet = new AuthenticationPacket();
        packet.username = username;
        packet.password = password;
        packet.randomHexValue = randomHex;
        packet.timestamp = timestamp;

        // Structure in bytes, encrypt with public key
        byte[] copiedStruct = PacketUtils.StructToByteArray(packet);
        RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
        provider.ImportParameters(publicKey);

        return provider.Encrypt(copiedStruct, true);
    }

    // Decrypts data that client sent
    public static AuthenticationPacket decryptData (byte[] byteArray, RSAParameters privateKey)
    {
        RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
        provider.ImportParameters(privateKey);
        byte[] decryptedBytes = provider.Decrypt(byteArray, true);

        AuthenticationPacket packet = new AuthenticationPacket();
        packet = (AuthenticationPacket) PacketUtils.ByteArrayToStruct(decryptedBytes, packet);

        return packet;
    }

    private static Random hexRandom = new Random();
    private const string hexContains = "0123456789ABCDEF";
    private static string RandomHexNumber(int digits)
    {
        string result = "";
        for (int i=0; i<digits; i++)
        {
            result += hexContains[hexRandom.Next(hexContains.Length)];
        }
        return result;
    }

    // Construct a challenge packet for client to read and respond
    public static ChallengePacket ConstructClientChallenge(RSAParameters publicKey)
    {
        ChallengePacket packet = new ChallengePacket();
        packet.publicKey = publicKey;
        packet.randomHexValue = RandomHexNumber(64);
        packet.timestamp = DateTime.Now.ToFileTime();
        return packet;
    }

}
