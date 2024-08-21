using System;
using System.Collections.Generic;
using _01_Scripts.UI;
using NaughtyAttributes;
using QubeesUtility.Runtime.QubeesUtility;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Managers
{
    public class QUIManager : SingletonPersistent<QUIManager>
    {
        private readonly Stack<QUIWindow> _windowStack = new();
        public List<QUIWindow> windows = new();
        public static Action<QUIWindow> OpenQWindow;
        public static Action CloseQWindow;
        public static Action CloseAllQWindows;

        [SerializeField] private Button closerButton;

        private void OnEnable()
        {
            OpenQWindow += OpenWindow;
            CloseQWindow += CloseTopWindow;
            CloseAllQWindows += CloseAllWindows;
            closerButton.onClick.AddListener(CloseTopWindow);
        }

        private void OnDisable()
        {
            OpenQWindow -= OpenWindow;
            CloseQWindow -= CloseTopWindow;
            CloseAllQWindows -= CloseAllWindows;
            closerButton.onClick.RemoveListener(CloseTopWindow);
        }

        private void OpenWindow(QUIWindow window)
        {
            _windowStack.Push(window);
        }

        [Button]
        private void CloseTopWindow()
        {
            if (_windowStack.Count <= 0) return;
            var topWindow = _windowStack.Pop();
            topWindow.Close();
        }

        private void CloseAllWindows()
        {
            while (_windowStack.Count > 0)
            {
                CloseTopWindow();
            }
        }
    }
}