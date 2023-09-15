using System.Collections.Generic;
using Scripts.Cannons;
using Scripts.Core.Input;
using Scripts.Core.Input.Data;
using Scripts.Core.Interfaces;
using Scripts.Core.Services;
using Scripts.Zones.Base;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Core.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header(Constants.Headers.Input)] 
        [SerializeField] private InputSettings _inputSettings;

        [Header(Constants.Headers.Other)]
        [SerializeField, Min(Constants.Zero)] private int currentSceneIndex;
        
        [Header(Constants.Headers.ExternalDependencies)] 
        [SerializeField] private RigidbodyAffectionZone2D _affectionZone2D;
        [SerializeField] private Cannon2D leftCannon;
        [SerializeField] private Cannon2D rightCannon;
        
        private readonly List<IUpdatable> _updatables = new(Constants.DefaultCollectionCapacity);
        private readonly KeyboardInputListener _keyboardInputListener = new();
        private readonly ISceneLoader _sceneLoader = new DefaultSceneLoader();

#if UNITY_EDITOR
        private void OnValidate()
        {
            currentSceneIndex = Mathf.Clamp(currentSceneIndex, Constants.Zero, SceneManager.sceneCountInBuildSettings);
        }
#endif
        
        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            for (var i = 0; i < _updatables.Count; i++)
            {
                _updatables[i].OnUpdate();
            }
        }

        private void Initialize()
        {
            InitializeInput();
        }

        private void InitializeInput()
        {
            _keyboardInputListener.BindAction(_inputSettings.leftCannonKey, leftCannon.PerformShot);
            _keyboardInputListener.BindAction(_inputSettings.rightCannonKey, rightCannon.PerformShot);
            _keyboardInputListener.BindAction(_inputSettings.reloadSceneKey, ReloadScene);
            _keyboardInputListener.BindAction(_inputSettings.quitApplicationKey, Application.Quit);
            _keyboardInputListener.BindAction(_inputSettings.enableAffectionZoneKey, _affectionZone2D.EnableAffection);
            _keyboardInputListener.BindAction(_inputSettings.disableAffectionZoneKey, _affectionZone2D.DisableAffection);
            
            _updatables.Add(_keyboardInputListener);
        }

        private void ReloadScene()
        {
            _sceneLoader.LoadScene(currentSceneIndex);
        }
    }
}