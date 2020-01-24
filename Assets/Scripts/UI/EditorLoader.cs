using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace AgaQ.UI
{
    /// <summary>
    /// Class provide functionality ro load editor scene asychronusly
    /// </summary>
    public class EditorLoader : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(LoadCoroutine());
        }

        IEnumerator LoadCoroutine()
        {
            yield return null;
            SceneManager.LoadSceneAsync("Editor");
        }
    }
}
