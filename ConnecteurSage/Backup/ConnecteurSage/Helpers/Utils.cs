using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Configuration;

namespace ConnecteurSage.Helpers
{
    /// <summary>
    /// Classe statique utilitaire
    /// </summary>
    public static class Utils
    {
        #region Champs privés
        /// <summary>
        /// Mot-clé de chiffrement utilisé pour le cryptage
        /// </summary>
        private static string CryptKey = "@SDK$";
        #endregion

        #region Méthodes de cryptage
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
                //MessageBox.Show(e.Message);
                return string.Empty;
            }
            catch (EncoderFallbackException e)
            {
                //MessageBox.Show(e.Message);
                return string.Empty;
            }
            catch (InvalidOperationException e)
            {
                //MessageBox.Show(e.Message);
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
                //MessageBox.Show(e.Message);
                return string.Empty;
            }
            catch (DecoderFallbackException e)
            {
                //MessageBox.Show(e.Message);
                return string.Empty;
            }
            catch (InvalidOperationException e)
            {
                //MessageBox.Show(e.Message);
                return string.Empty;
            }
        }
        #endregion
       
    }
}
