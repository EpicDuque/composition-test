using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool loadScenes;
    private void Start()
    {
        if (loadScenes)
        {
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
            SceneManager.LoadScene("Level", LoadSceneMode.Additive);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level"));
    }
}
