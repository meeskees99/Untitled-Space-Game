using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TESTSCRIPTVOORPRESENTATIE : MonoBehaviour
{
    [SerializeField] string _sceneToLoad;

    public void BackButton()
    {
        DataPersistenceManager.instance.SaveGame();

        SceneManager.LoadSceneAsync(_sceneToLoad);
    }
}
