using UnityEngine;

public class AuthScreen : MonoBehaviour
{
    public GameObject WelcomeContent;
    public GameObject LoginContent;

    public void GetStarted()
    {
        WelcomeContent.SetActive(false);
        LoginContent.SetActive(true);
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
