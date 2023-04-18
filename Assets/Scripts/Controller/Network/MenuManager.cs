using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private CharacterData[] m_characterDatas;

    private IEnumerator Start()
    {
        ClearAllCharacterData();

        yield return new WaitUntil(() => NetworkManager.Singleton.SceneManager != null);
    }
    
    private void ClearAllCharacterData()
    {
        // Clean the all the data of the characters so we can start with a clean slate
        foreach (CharacterData data in m_characterDatas)
        {
            data.EmptyData();
        }
    }
}
