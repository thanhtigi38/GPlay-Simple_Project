using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThanhND
{
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "ScriptableObjects/LevelDatabase", order = 1)]
    public class LevelDatabase : ScriptableObject
    {
        public List<LevelData> levels;
    }


    [Serializable]
    public class LevelData
    {
        public int id;
        public int totalObjects;
        public int totalTypes;
        public float maxPairDistance = 5.0f; 
    }
}