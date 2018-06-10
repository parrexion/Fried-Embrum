using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StarList : ScriptableObject {
    
    public CharacterStats[] characters;

    
    public CharacterStats GetRandom() {
        return characters[Random.Range(0, characters.Length)];
    }
}
