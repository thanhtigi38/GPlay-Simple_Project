using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PianoTileData", fileName = "PianoTileData.asset")]
public class PianoTileData : ScriptableObject
{
    public AudioClip[] clips;
    public float timer;
    public string name;
}
