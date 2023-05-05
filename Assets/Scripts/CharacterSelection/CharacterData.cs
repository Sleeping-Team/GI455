using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player",menuName = "Character Data",order = 2)]
public class CharacterData : ScriptableObject
{
        [Header("Data")] 
        public GameObject characterPrefab;
        public Sprite characterSprite;

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
