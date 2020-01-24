using UnityEngine;
using System;
using Lean.Localization;

namespace AgaQ.UI.Dialogs
{
    /// <summary>
    /// Provides functionality for displaying standard general dialogs.
    /// </summary>
    public static class Dialog
    {
        #region Public functions

        /// <summary>
        /// Show info dialog with translated text
        /// </summary>
        /// <param name="translationLabel">Translation label.</param>
        public static void ShowTranslatedInfo(string translationLabel)
        {
            var translation = LeanLocalization.GetTranslation(translationLabel);
            ShowInfo(translation != null ? translation.Text : translationLabel);
        }

        /// <summary>
        /// Display standard dialog with some text info and button ok.
        /// </summary>
        /// <param name="infoText">text to diaopley in dialog.</param>
        public static void ShowInfo(string infoText)
        {
            ShowInfo(infoText, DialogButtonType.ok);
        }

        /// <summary>
        /// Display standard dialog with text info and one button.
        /// </summary>
        /// <param name="infoText">Text to diaopley in dialog.</param>
        /// <param name="buttonType">Button type to display.</param>
        public static void ShowInfo(string infoText, DialogButtonType buttonType)
        {
            ShowInfo(infoText, GetTranslatedButtonText(buttonType));
        }

        /// <summary>
        /// Display standard dialog with text info and one button.
        /// </summary>
        /// <param name="infoText">Text to diaopley in dialog.</param>
        /// <param name="buttonText">Text to diaply at button.</param>
        public static void ShowInfo(string infoText, string buttonText)
        {
            var dialog = GetDialogBehaviour();
            if (dialog != null)
            {
                dialog.SetOneButtonDialog(infoText, buttonText);
                dialog.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Show standard dialog with text info and two buttons.
        /// </summary>
        /// <param name="questionText">text to diaopley in dialog.</param>
        /// <param name="button1Type">Cancel button type.</param>
        /// <param name="button2Type">Ok button type</param>
        /// <param name="OnButtonClickAction">Action to call when button pressed</param>
        public static void ShowQuesttion(string questionText, DialogButtonType button1Type, DialogButtonType button2Type, Action<bool> OnButtonClickAction)
        {
            ShowQuesttion(questionText, GetTranslatedButtonText(button1Type), GetTranslatedButtonText(button2Type), OnButtonClickAction);
        }

        /// <summary>
        /// Show standard dialog with text info and two buttons.
        /// </summary>
        /// <param name="questionText">text to diaopley in dialog.</param>
        /// <param name="buttonCancelText">Text to dispaly at button cancel.</param>
        /// <param name="buttonOkText">Text to display at button ok.</param>
        /// <param name="OnButtonClickAction">Action to call when button pressed</param>
        public static void ShowQuesttion(string questionText, string buttonCancelText, string buttonOkText, Action<bool> OnButtonClickAction)
        {
            var dialog = GetDialogBehaviour();
            if (dialog != null)
            {
                dialog.SetTwoButtonsDialog(questionText, buttonCancelText, buttonOkText, OnButtonClickAction);
                dialog.gameObject.SetActive(true);
            }
        }

        #endregion

        /// <summary>
        /// Get dialog behaviour connected with GameObject.
        /// If there is no dialog object, load it first form resources.
        /// </summary>
        /// <returns>DialogBehaviour object</returns>
        static DialogBehaviour GetDialogBehaviour()
        {
            var dialogBehaviour = GameObject.FindObjectOfType<DialogBehaviour>();
            if (dialogBehaviour == null)
            {
                //load dialog from resources and instanciate it
                var dialog = Resources.Load("GeneralDialog") as GameObject;
                if (dialog != null)
                {
                    //find canvas
                    var canvas = GameObject.FindObjectOfType<Canvas>();
                    if (canvas != null)
                    {
                        //instansiate dialog
                        dialog = GameObject.Instantiate(dialog, canvas.transform);
                        dialogBehaviour = dialog.GetComponent<DialogBehaviour>();
                    }
                }
            }

            return dialogBehaviour;
        }

        /// <summary>
        /// Gets translation of button text.
        /// </summary>
        /// <returns>The translated buttontext.</returns>
        /// <param name="buttonType">Button type.</param>
        static string GetTranslatedButtonText(DialogButtonType buttonType)
        {
            string translationKey = "";
            if (buttonType == DialogButtonType.no)
                translationKey = "No";
            else if (buttonType == DialogButtonType.yes)
                translationKey = "Yes";
            else if (buttonType == DialogButtonType.ok)
                translationKey = "OK";
            else if (buttonType == DialogButtonType.cancel)
                translationKey = "Cancel";

            var translation = LeanLocalization.GetTranslation(translationKey);

            if (translation == null)
                return translationKey;
            
            return LeanLocalization.GetTranslation(translationKey).Text;
        }
    }
}
