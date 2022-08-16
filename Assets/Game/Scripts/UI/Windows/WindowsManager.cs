
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Game.UI.Windows
{
    public class WindowsManager
    {
        private List<IWindow> windows = new List<IWindow>();

        public void Register(IWindow window)
        {
            if (!windows.Contains(window))
            {
                windows.Add(window);
            }
        }
        public void UnRegister(IWindow window)
        {
            if (windows.Contains(window))
            {
                windows.Remove(window);
            }
        }

        public bool IsAnyWindowShowing()
        {
            return windows.Any((x) => x.IsShowing);
        }
        public bool IsAllHided()
        {
            return windows.All((x) => !x.IsShowing);
        }

        public bool IsContains<T>() where T : IWindow
        {
            return windows.OfType<T>().Any();
        }

        public void Show<T>() where T : class, IWindow
        {
            GetAs<T>().Show();
        }
        public void Hide<T>() where T : class, IWindow
        {
            GetAs<T>().Hide();
        }

        public void HideAll()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].Hide();
            }
        }

        public T GetAs<T>() where T : class, IWindow
        {
            return Get<T>() as T;
        }
        public IWindow Get<T>() where T : IWindow
        {
            if (IsContains<T>())
            {
                return windows.Where((window) => window is T).FirstOrDefault();
            }

            throw new System.Exception("UIWindowsManager DOESN'T CONTAINS WINDOW ERROR");
        }
    }
}