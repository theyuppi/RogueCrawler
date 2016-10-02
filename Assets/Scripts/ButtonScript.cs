using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class ButtonScript : MonoBehaviour {

        public void PlayBtn()
        {
            if (PlayerPrefs.GetInt("PD") == 1)
            {
                PlayerPrefs.SetInt("gameStarted", 1);
                SceneManager.LoadScene("MainScene");

            }
            else
            {
                PlayerPrefs.SetInt("gameStarted", 0);
                PlayerPrefs.SetInt("PlayerShouldLoadItems", 1);
                SceneManager.LoadScene("MainScene");
            }
        }

        public void ExitBtn()
        {
            Application.Quit();
        }
    }
}
