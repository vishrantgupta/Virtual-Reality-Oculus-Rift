#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
class EditorContinuousLoop
{
    static EditorContinuousLoop()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
      
        if (PlayerPrefs.GetInt("MessageRefresh") == 1)
        {
            Debug.Log("ContinuousLoop: Refresh! ");
            AssetDatabase.Refresh();
            PlayerPrefs.SetInt("MessageRefresh", 0);
        }
        //}
    }
}
#endif

