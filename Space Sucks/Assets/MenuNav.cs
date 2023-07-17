using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNav : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

}
