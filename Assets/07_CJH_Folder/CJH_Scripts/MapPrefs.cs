using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPrefs : MonoBehaviour
{
    private const string SelectedMapIndexKey = "SelectedMapIndex";

    public static void SaveSelectedMapIndex(int index)
    {
        PlayerPrefs.SetInt(SelectedMapIndexKey, index);
        PlayerPrefs.Save();
    }

    public static int LoadSelectedMapIndex()
    {
        return PlayerPrefs.GetInt(SelectedMapIndexKey, -1); // -1: 저장된 값 없음
    }

    public static bool HasSavedSelection()
    {
        return PlayerPrefs.HasKey(SelectedMapIndexKey);
    }

    public static void ResetSavedSelection()
    {
        PlayerPrefs.DeleteKey(SelectedMapIndexKey);
    }
}
