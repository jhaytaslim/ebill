using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace ebill.Utils;

public partial class Cryptography
{

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    // Decode
    public static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public string EncryptSHA256(string source)
    {
        try
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                Console.WriteLine("The SHA256 hash of " + source + " is: " + hash);

                return hash;
            }
        }
        catch (IOException e)
        {
            Console.WriteLine($"I/O Exception: {e.Message}");
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine($"Access Exception: {e.Message}");
        }

        return string.Empty;
    }
}
