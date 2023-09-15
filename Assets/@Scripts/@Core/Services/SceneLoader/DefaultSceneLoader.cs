using System;
using UnityEngine.SceneManagement;

namespace Scripts.Core.Services
{
    public class DefaultSceneLoader : ISceneLoader
    {
        public void LoadScene(int sceneBuildIndex)
        {
#if DEBUG
            if (SceneManager.sceneCountInBuildSettings == 0)
                throw new Exception("You haven't added any scene to Build Settings!");

            if (sceneBuildIndex >= SceneManager.sceneCountInBuildSettings)
                throw new ArgumentOutOfRangeException(nameof(sceneBuildIndex));
#endif
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }
}