using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Libraries/Class")]
public class ClassLibrary : ScriptableObject {

    public CharClass[] library;


    public CharClass GetEntry(string id) {
        for (int i = 0; i < library.Length; i++) {
            if (library[i].id == id)
                return library[i];
        }

        Debug.LogWarning("Could not find the id:  " + id);
        return null;
    }
}
