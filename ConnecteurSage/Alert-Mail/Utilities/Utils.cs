using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Alert_Mail.Utilities
{
    public static class Utils
    {
        /// <summary>
        /// Mot-clé de chiffrement utilisé pour le cryptage
        /// </summary>
        private static string CryptKey = "@SDK$";

        /// <summary>
        /// Crypte une chaine de caractère en utilisant une clé de cryptage
        /// </summary>
        /// <param name="original">la chaine à crypter</param>
        /// <returns>La chaine cryptée</returns>
        public static string Encrypt(string original)
        {
            try
            {
                MD5CryptoServiceProvider hashMd5 = new MD5CryptoServiceProvider();
                byte[] passwordHash = hashMd5.ComputeHash(
                Encoding.Default.GetBytes(CryptKey));
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = passwordHash;
                des.Mode = CipherMode.ECB;
                byte[] buffer = Encoding.Default.GetBytes(original);
                return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (CryptographicException e)
            {
                //Survient lorsque le fournisseur de cryptographie n'est pas disponible
                Console.WriteLine(e.Message);
                return string.Empty;
            }
            catch (EncoderFallbackException e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Décrypte une chaine en utilisant une clé de cryptage
        /// </summary>
        /// <param name="encrypted">la chaîne à décrypter</param>
        /// <returns>la chaine décryptée</returns>
        public static string Decrypt(string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted))
                return string.Empty;
            try
            {
                encrypted = Encoding.Default.GetString(Convert.FromBase64String(encrypted));
                MD5CryptoServiceProvider hashMd5 = new MD5CryptoServiceProvider();
                byte[] passwordHash = hashMd5.ComputeHash(Encoding.Default.GetBytes(CryptKey));
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = passwordHash;
                des.Mode = CipherMode.ECB;
                byte[] buffer = Encoding.Default.GetBytes(encrypted);
                return Encoding.Default.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (CryptographicException e)
            {
                //Survient lorsque le fournisseur de cryptographie n'est pas disponible
                Console.WriteLine(e.Message);
                return string.Empty;
            }
            catch (DecoderFallbackException e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
                return string.Empty;
            }
        }

        public static String[,] getAllPrioritiesTable()
        {
            String[,] table = new String[6, 3];
            table[0,0] = "P0"; table[0,1] = "#000000"; table[0,2] = "Votre ticket sera traité dans 24 heurs ouvrables";
            table[1,0] = "P1"; table[1,1] = "#FF0000"; table[1,2] = "Votre ticket sera traité dans 24 heurs ouvrables";
            table[2,0] = "P2"; table[2,1] = "#FF4400"; table[2,2] = "Votre ticket sera traité dans 1-2 jours ouvrables";
            table[3,0] = "P3"; table[3,1] = "##E8700C"; table[3,2] = "Votre ticket sera traité dans 2-3 jours ouvrables";
            table[4,0] = "P4"; table[4,1] = "#FFFF00"; table[4,2] = "Votre ticket sera traité dans 2-3 jours ouvrables";
            table[5,0] = "P0-P4"; table[5,1] = "#0C83E8"; table[5,2] = "Votre ticket sera traité au plus vite";
            return table;
        }
    }
}
