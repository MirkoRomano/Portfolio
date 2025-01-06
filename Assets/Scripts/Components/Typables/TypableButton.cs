using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Portfolio.Shared
{
    [RequireComponentInChildren(typeof(TypableText))]
    public class TypableButton : Selectable
    {
        /// <summary>
        /// Function definition for a button click event.
        /// </summary>
        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }

        /// <summary>
        /// Event delegates triggered on click.
        /// </summary>
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent onClick = new ButtonClickedEvent();

        /// <summary>
        /// UnityEvent that is triggered when the button is pressed.
        /// </summary>
        public ButtonClickedEvent OnClick
        {
            get 
            { 
                return onClick; 
            }
            
            set 
            { 
                onClick = value; 
            }
        }

        protected TypableButton()
        { }

        public override void OnPointerDown(PointerEventData eventData)
        {
            return;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            return;
        }

        /// <summary>
        /// Button press logic
        /// </summary>
        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            onClick.Invoke();
        }


        /// <summary>
        /// Execute the click logic from code
        /// </summary>
        public void ForceClick()
        {
            DoStateTransition(SelectionState.Pressed, false);
            Press();
        }

        /// <summary>
        /// Reset the button transition state
        /// </summary>
        public void ForceReset()
        {
            DoStateTransition(SelectionState.Normal, false);
        }
    }
}


