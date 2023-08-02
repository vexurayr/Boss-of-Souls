using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    // Directory where the data will be saved
    private string dataDirPath = "";

    // Name of the file it will save as
    private string dataFileName = "";

    private bool isUsingEncryption;

    private readonly string encryptionCodeWord = "LookAway";

    public FileDataHandler(string dataDirPath, string dataFileName, bool isUsingEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.isUsingEncryption = isUsingEncryption;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                // Load serialized data from file
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Optionally decrypt the data
                if (isUsingEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // Deserialize data from Json back into C#
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured attempting to load data from file: " + fullPath + "\n" + e);
            }
        }
        
        return loadedData;
    }

    public void Save(GameData data)
    {
        // This way works with more OS having different path separators
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            // Create the directory the file will be written to if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize game object data into Json, true allows for Json formatting
            // Can't handle complex data structures like dictionaries
            string dataToStore = JsonUtility.ToJson(data, true);

            // Optionally encrypt the data
            if (isUsingEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // Write serialized data to file, using blocks ensure connection to that file is closed when finished reading/writing
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured attempting to save data to file: " + fullPath + "\n" + e);
        }
    }

    // Basic implementation of XOR encryption
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";

        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return modifiedData;
    }
}