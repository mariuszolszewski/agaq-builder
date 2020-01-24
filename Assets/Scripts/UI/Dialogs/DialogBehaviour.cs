using UnityEngine;
using UnityEngine.UI;
using System;

namespace AgaQ.UI.Dialogs
{
    /// <summary>
    /// Behaviour that cotrols dialog.
    /// </summary>
    public class DialogBehaviour : MonoBehaviour
    {
        [SerializeField] Text text;      // placeholder for dialog text
        [SerializeField] Button button1; // first button
        [SerializeField] Button button2; // second button

        Action<bool> OnButtonClick;

        #region Public functions

        /// <summary>
        /// Configure dialog to layoutwith one button.
        /// </summary>
        /// <param name="dialogText">Dialog text.</param>
        /// <param name="buttonText">Text to show at button.</param>
        public void SetOneButtonDialog(string dialogText, string buttonText)
        {
            text.text = dialogText;
            SetButton(button2, buttonText);
            button1.gameObject.SetActive(false);
            OnButtonClick = null;
        }

        /// <summary>
        /// Configure dialog to layout with two buttons.
        /// </summary>
        /// <param name="dialogText">DIalog text.</param>
        /// <param name="cancelButtonText">First button text.</param>
        /// <param name="okButtonText">Second button text.</param>
        /// <param name="onClickAction">Action to call on button click.</param>
        public void SetTwoButtonsDialog(string dialogText, string cancelButtonText, string okButtonText, Action<bool> onClickAction)
        {
            text.text = dialogText;
            SetButton(button1, cancelButtonText);
            SetButton(button2, okButtonText);
            OnButtonClick = onClickAction;
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Event handler for button 1.
        /// </summary>
        public void OnButton1Pressed()
        {
            //close dialog
            gameObject.SetActive(false);

            if (OnButtonClick != null)
                OnButtonClick(false);
        }

        /// <summary>
        /// Event handler for button 2.
        /// </summary>
        public void OnButton2Pressed()
        {
            //close dialog
            gameObject.SetActive(false);

            if (OnButtonClick != null)
                OnButtonClick(true);
        }

        #endregion

        /// <summary>
        /// Prepares button to use in dialog.
        /// </summary>
        /// <param name="button">Button to prepare.</param>
        /// <param name="buttonText">Text to set at button.</param>
        void SetButton(Button button, string buttonText)
        {
            var buttonTextComponent = button.GetComponentInChildren<Text>();
            if (buttonTextComponent != null)
                buttonTextComponent.text = buttonText;
            button.gameObject.SetActive(true);
        }
    }
}
