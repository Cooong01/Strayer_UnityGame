using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletePref : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.DeleteAll();
    }
}
