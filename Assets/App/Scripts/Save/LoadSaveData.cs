using System;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Collections;

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
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.GenerateIV();

            using MemoryStream memoryStream = new();
            memoryStream.Write(aes.IV, 0, aes.IV.Length);

            using CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using StreamWriter writer = new(cryptoStream);
            writer.Write(plainText);

            writer.Flush();
            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private static string Decrypt(string cipherText, string key)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);

            byte[] iv = buffer[..(aes.BlockSize / 8)];
            byte[] cipherArray = buffer[(aes.BlockSize / 8)..];

            aes.IV = iv;

            using MemoryStream memoryStream = new(cipherArray);
            using CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(cryptoStream);

            return reader.ReadToEnd();
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

            SetScreen();
        }

        private void SetScreen()
        {
            if (rsoContentSave.Value.fullScreen)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }

            Screen.fullScreen = rsoContentSave.Value.fullScreen;
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
