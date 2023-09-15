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
        [SerializeField, Min(Constants.Zero), Delayed] private int _currentSceneIndex;
        
        [Header(Constants.Headers.ExternalDependencies)] 
        [SerializeField] private RigidbodyModifierZone2D _rigidbodyModifierZone2D;
        [SerializeField] private Cannon2D _leftCannon;
        [SerializeField] private Cannon2D _rightCannon;
        
        private readonly List<IUpdatable> _updatables = new(Constants.DefaultCollectionCapacity);
        private readonly KeyboardInputListener _keyboardInputListener = new();
        private readonly ISceneLoader _sceneLoader = new DefaultSceneLoader();

#if UNITY_EDITOR
        private void OnValidate()
        {
            _currentSceneIndex = 
                Mathf.Clamp(_currentSceneIndex, Constants.Zero, SceneManager.sceneCountInBuildSettings);
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
            _keyboardInputListener.BindAction(_inputSettings.LeftCannonKey, _leftCannon.PerformShot);
            _keyboardInputListener.BindAction(_inputSettings.RightCannonKey, _rightCannon.PerformShot);
            _keyboardInputListener.BindAction(_inputSettings.ReloadSceneKey, ReloadScene);
            _keyboardInputListener.BindAction(_inputSettings.QuitApplicationKey, Application.Quit);
            _keyboardInputListener.BindAction(_inputSettings.EnableModifierZoneKey, _rigidbodyModifierZone2D.EnableModifier);
            _keyboardInputListener.BindAction(_inputSettings.DisableModifierZoneKey, _rigidbodyModifierZone2D.DisableModifier);
            
            _updatables.Add(_keyboardInputListener);
        }

        private void ReloadScene()
        {
            _sceneLoader.LoadScene(_currentSceneIndex);
        }
    }
}