using System;

namespace Portfolio
{
    public class MasterLoop : SingletonMonobehaviour<MasterLoop>
    {
        public static event Action UpdateLoop;
        public static event Action LateUpdateLoop;

        private void Update()
        {
            UpdateLoop?.Invoke();
        }

        private void LateUpdate()
        {
            LateUpdateLoop?.Invoke();
        }
    }
}

