using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Player",menuName = "Character Data",order = 2)]
public class CharacterData : ScriptableObject
{ 
        [Header("Data")] 
        public GameObject characterPrefab;
        public Image characterProfile;

        [Header("Client Info")] 
        public ulong clientId;
        public int playerId;
        public bool isSelected;

        private void OnEnable()
        {
                EmptyData();
        }

        public void EmptyData()
        { 
                isSelected = false;
                clientId = 0;
                playerId = -1;
        }
}
