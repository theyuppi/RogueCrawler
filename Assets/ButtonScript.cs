using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	public void PlayBtn()
    {
        Application.LoadLevel("MainScene");
    }

    public void ExitBtn()
    {
        Application.Quit();
    }
}
