using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is an interface, only describes methods implementing scripts need
public interface IDataPersistence
{
    void LoadData(GameData data);

    // Want implementing script to modify the data
    void SaveData(GameData data);
}