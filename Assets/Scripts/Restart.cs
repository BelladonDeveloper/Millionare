using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void OnRestart()
    {
        SceneManager.LoadScene(0);
        Debug.Log("Restart");
    }
}
