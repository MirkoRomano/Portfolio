using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Portfolio.Shared
{
    [DefaultExecutionOrder(-1)]
    public class TypableGroup : UIBehaviour
    {
        /// <summary>
        /// Typables list
        /// </summary>
        protected readonly List<ITypable> typables = new List<ITypable>();

        protected TypableGroup()
        { }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterKeyboardInput();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnregisterKeyboardInput();
        }

        /// <summary>
        /// Register the typable to the keyboard
        /// </summary>
        private void RegisterKeyboardInput()
        {
            Keyboard.current.onTextInput += CatchInput;
        }

        /// <summary>
        /// Unregister the typable from the keyboard
        /// </summary>
        private void UnregisterKeyboardInput()
        {
            Keyboard.current.onTextInput -= CatchInput;
        }

        /// <summary>
        /// Catch an input from the keyboard
        /// </summary>
        /// <param name="input">Keyboard input</param>
        private void CatchInput(char input)
        {
            if(typables.Count <= 0)
            {
                return;
            }

            int inputLengthTreshold = typables.Max(x => x.TypedLength);
            foreach (ITypable typable in typables)
            {
                bool canCatchInput = !typable.IsTextCompletelyTyped && typable.CharToType.Equals(input);
                if (!canCatchInput)
                {
                    typable.ResetText();
                    continue;
                }

                bool isOneOfTheMostTyped = typable.TypedLength >= inputLengthTreshold;
                if (isOneOfTheMostTyped)
                {
                    ITextInputHandler inputHandler = typable as ITextInputHandler;

                    if(inputHandler == null)
                    {
                        continue;
                    }

                    inputHandler.CatchInput(input);
                }
                else
                {
                    typable.ResetText();
                }
            }
        }

        /// <summary>
        /// Return a list of active typables
        /// </summary>
        public IEnumerable<ITypable> ActiveTypables()
        {
            return typables.Where(x => x.Enabled && !x.IsTextCompletelyTyped);
        }

        /// <summary>
        /// Get the first active typable of registered in the group
        /// </summary>
        public ITypable GetFirstActiveTypable()
        {
            IEnumerable<ITypable> activeToggles = ActiveTypables();
            return activeToggles.Count() > 0 
                   ? activeToggles.First() 
                   : null;
        }


        /// <summary>
        /// Unregister a typable from the group.
        /// </summary>
        /// <param name="typable">The typable to remove.</param>
        public void UnregisterToggle(ITypable typable)
        {
            if (typables.Contains(typable))
            {
                typables.Remove(typable);
            }
        }

        /// <summary>
        /// Register a typable with the group
        /// </summary>
        /// <param name="typable">The typable to register with the group.</param>
        public void RegisterToggle(ITypable typable)
        {
            if (!typables.Contains(typable))
            {
                typables.Add(typable);
            }
        }

        /// <summary>
        /// Check if a typable is registered in the group
        /// </summary>
        /// <param name="typable">Typable to check</param>
        public bool Exist(ITypable typable) 
        {
            return typables.Contains(typable);
        }
    }
}