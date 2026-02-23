using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadGame : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(waitOpen());
    }

    IEnumerator waitOpen()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Arena");
    }
}
