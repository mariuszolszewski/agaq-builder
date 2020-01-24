using UnityEngine;

namespace AgaQ.UI
{
    public class  PreferencesDialog : MonoBehaviour
    {
        [SerializeField] ColorButton backgroundCloreButton;
        [SerializeField] ColorButton highlightColorButton;
        [SerializeField] ColorButton selectionColorButton;

        void OnEnable()
        {
            var prefs = Preferences.instance;

            backgroundCloreButton.SetColor(prefs.editorBackgroundColor);
            backgroundCloreButton.ConfirmColor();

            highlightColorButton.SetColor(prefs.brickHighlightColor);
            highlightColorButton.ConfirmColor();

            selectionColorButton.SetColor(prefs.brickSelectionColor);
            selectionColorButton.ConfirmColor();
        }

        public void Reset()
        {
            var prefs = Preferences.instance;

            backgroundCloreButton.SetColor(prefs.defaultEditorBackgroundColor);
            backgroundCloreButton.ConfirmColor();

            highlightColorButton.SetColor(prefs.defaultBrickHighlightColor);
            highlightColorButton.ConfirmColor();

            selectionColorButton.SetColor(prefs.defaultSelectionColor);
            selectionColorButton.ConfirmColor();

        }

        public void Save()
        {
            var prefs = Preferences.instance;

            prefs.editorBackgroundColor = backgroundCloreButton.selectedColor;
            prefs.brickHighlightColor = highlightColorButton.selectedColor;
            prefs.brickSelectionColor = selectionColorButton.selectedColor;

            prefs.Save();

            UnityEngine.Camera.main.backgroundColor = backgroundCloreButton.selectedColor;

            transform.parent.gameObject.SetActive(false);
        }
    }
}
