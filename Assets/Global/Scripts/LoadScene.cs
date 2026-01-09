using UnityEngine;
using UnityEngine.SceneManagement;

namespace Global.Scripts
{
    public class LoadScene:MonoBehaviour
    {
        void Start()
        {
            // 遍历Build Settings里的所有场景，全部加载（叠加）
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive);
            }
        }
    }
}