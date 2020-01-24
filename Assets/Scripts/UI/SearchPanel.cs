using UnityEngine;
using UnityEngine.UI;

namespace AgaQ.UI
{
    /// <summary>
    /// Controller for search panel used as text filter for brick list.
    /// </summary>
    public class SearchPanel : MonoBehaviour
    {
        [SerializeField] BricksList list;
        [SerializeField] InputField inputField;
        [SerializeField] Button cancelButton;

        void Start()
        {
            inputField.text = "";
            cancelButton.gameObject.SetActive(false);

            inputField.onValueChanged.AddListener(delegate { OnTextChange(); });
            cancelButton.onClick.AddListener(delegate { OnCancelButton(); });
        }

        void OnTextChange()
        {
            string text = inputField.text;
            cancelButton.gameObject.SetActive(text != "");            
            list.SetNameFilter(text);
        }

        void OnCancelButton()
        {
            inputField.text = "";
            cancelButton.gameObject.SetActive(false);
        }
    }
}
