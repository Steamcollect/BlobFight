using System;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

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
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key).Take(32).ToArray();
            aes.GenerateIV();

            using MemoryStream ms = new();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using StreamWriter sw = new(cs);
            sw.Write(plainText);

            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        private static string Decrypt(string cipherText, string key)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key).Take(32).ToArray();
            aes.IV = buffer.Take(aes.BlockSize / 8).ToArray();

            using MemoryStream ms = new(buffer.Skip(aes.IV.Length).ToArray());
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new(cs);
            return sr.ReadToEnd();
        }

        private void SaveToJson()
        {
            string infoData = JsonUtility.ToJson(rsoContentSave.Value);
            string encryptedJson = Encrypt(infoData, EncryptionKey);
            File.WriteAllText(filePath, encryptedJson);
        }
        private void LoadFromJson()
        {
            string infoData = File.ReadAllText(filePath);
            string decryptedJson = Decrypt(infoData, EncryptionKey);
            rsoContentSave.Value = JsonUtility.FromJson<ContentSaved>(decryptedJson);
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
