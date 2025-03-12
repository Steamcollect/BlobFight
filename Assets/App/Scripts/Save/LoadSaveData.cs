using System;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace BT.Save
{
    public class LoadSaveData : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RSE_LoadData rseLoad;
        [SerializeField] private RSE_SaveData rseSave;
        [SerializeField] private RSE_ClearData rseClear;
        [SerializeField] private RSO_ContentSaved rsoContentSave;
        
        private string filePath;

        private static readonly string EncryptionKey = "ajekoBnPxI9jGbnYCOyvE9alNy9mM/Kw";

        private void OnEnable()
        {
            rseLoad.action += LoadFromJson;
            rseSave.action += SaveToJson;
            rseClear.action += ClearContent;
        }

        private void OnDisable()
        {
            rseLoad.action -= LoadFromJson;
            rseSave.action -= SaveToJson;
        }

        private void Start()
        {
            filePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Save", "Save.json");

            string directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (FileAlreadyExist()) LoadFromJson();
            else SaveToJson();
        }

        private static string Encrypt(string plainText, string key)
        {
            byte[] iv;
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV();
                iv = aes.IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            byte[] combinedArray = new byte[iv.Length + array.Length];
            Array.Copy(iv, 0, combinedArray, 0, iv.Length);
            Array.Copy(array, 0, combinedArray, iv.Length, array.Length);

            return Convert.ToBase64String(combinedArray);
        }

        private static string Decrypt(string cipherText, string key)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);

                byte[] iv = new byte[aes.BlockSize / 8];
                byte[] cipherArray = new byte[buffer.Length - iv.Length];

                Array.Copy(buffer, iv, iv.Length);
                Array.Copy(buffer, iv.Length, cipherArray, 0, cipherArray.Length);

                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(cipherArray))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private void SaveToJson()
        {
            string infoData = JsonUtility.ToJson(rsoContentSave.Value);
            //string encryptedJson = Encrypt(infoData, EncryptionKey);
            File.WriteAllText(filePath, infoData);
        }
        private void LoadFromJson()
        {
            string infoData = System.IO.File.ReadAllText(filePath);
            rsoContentSave.Value = JsonUtility.FromJson<ContentSaved>(infoData);
        }
        private void ClearContent()
        {
            rsoContentSave.Value = new();
            SaveToJson();
        }

        private bool FileAlreadyExist()
        {
            return File.Exists(filePath);
        }        
    }   
}
