using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Start()
    {
        UIMgr.Instance.ShowPanel<PanelMain>();
        MusicMgr.Instance.PlayBKMusic("Magical Brittlethorn Cut 60");
    }
}
