using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fab.Save.Samples
{
    [RequireComponent(typeof(UIDocument))]
    public class SaveLoadUI : MonoBehaviour
    {
        private UIDocument document;

        [SerializeField]
        private SaveSystem saveSystem;

        private TextField filenameInput;

        private void Start()
        {
            // make sure the save directory exists.
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Saved"));

            document = GetComponent<UIDocument>();

            filenameInput = document.rootVisualElement.Q<TextField>(name: "filename-input");

            Button saveBtn = document.rootVisualElement.Q<Button>(name: "save-btn");
            Button loadBtn = document.rootVisualElement.Q<Button>(name: "load-btn");

            if (string.IsNullOrEmpty(filenameInput.text))
            {
                saveBtn.SetEnabled(false);
                loadBtn.SetEnabled(false);
            }

            saveBtn.clicked += () => {
                string fullPath = GetSavePath(filenameInput.text);
                Debug.Log("Saving game to " + fullPath);
                saveSystem.Save(fullPath);
            };
            loadBtn.clicked += () => {
                string fullPath = GetSavePath(filenameInput.text);
                Debug.Log("Loading game from " + fullPath);
                saveSystem.Load(fullPath);
            };

            filenameInput.RegisterValueChangedCallback(evt =>
            {
                bool isValid = !string.IsNullOrEmpty(evt.newValue);

                saveBtn.SetEnabled(isValid);
                loadBtn.SetEnabled(isValid);
            });
        }

        private string GetSavePath(string filename)
        {
            return Path.Combine(Application.persistentDataPath, "Saved", filename + ".save");
        }
    }
}
