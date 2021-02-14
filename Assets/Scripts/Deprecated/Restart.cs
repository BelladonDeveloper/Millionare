using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void OnRestart()
    {
        // Deprecated після перезавантаження сцени отримую старі дані :(
        SceneManager.LoadScene(0); 

        Debug.Log("Restart");
    }
}
