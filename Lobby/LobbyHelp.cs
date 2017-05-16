using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHelp : MonoBehaviour {


    GameObject helpFrame;

    void Start() {
        helpFrame = transform.Find("HelpFrame").gameObject;
    }


    public void OnCloseButtonPressed() {
        helpFrame.SetActive(false);
    }

    public void OnHelpButtonPressed() {
        helpFrame.SetActive(true);
    }
}
