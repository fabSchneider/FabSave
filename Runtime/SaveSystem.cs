using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Fab.Save
{
    /// <summary>
    /// The save system is responsible for saving and loading of the save objects in the scene.
    /// </summary>
    [AddComponentMenu("Fab/Save/Save System")]
    public class SaveSystem : MonoBehaviour
    {
        [Tooltip("A list of all prefabs that should possibly be instantiated in the load process.")]
        public List<SaveObject> prefabs;

        private Dictionary<string, GameObject> prefabDict;
        private JsonSerializer serializer;

        private void Start()
        {
            prefabDict = new Dictionary<string, GameObject>(prefabs.Count);

            foreach (SaveObject prefab in prefabs)
            {
                prefabDict.Add(prefab.Guid, prefab.gameObject);
            }

            serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Include;
            serializer.Converters.Add(new ObjectStateDataConverter());
        }

        /// <summary>
        /// Saves all objects in the scene to a file.
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            GameObject root = SaveUtils.GetRootSaveContainer();
            if(root == null)
            {
                Debug.LogError($"No root save object found. Add a gameObject with a \"{SaveUtils.RootTagName}\" tag to the scene.");
                return;
            }

            List<ObjectState> states = SaveState(root);

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, states);
                }
            }
        }

        /// <summary>
        /// Loads all objects from a save file.
        /// </summary>
        /// <param name="filePath"></param>
        public void Load(string filePath)
        {
            GameObject root = SaveUtils.GetRootSaveContainer();
            if (root == null)
            {
                Debug.LogError($"No root save object found. Add a gameObject with a \"{SaveUtils.RootTagName}\" tag to the scene.");
                return;
            }

            if (File.Exists(filePath))
            {
                List<ObjectState> states;
                using (StreamReader sr = new StreamReader(filePath))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        states = serializer.Deserialize<List<ObjectState>>(reader);
                    }
                }

                ApplyState(prefabDict, states, root);
            }
            else
            {
                Debug.LogError("File does not exist.");
            }
        }

    protected static List<ObjectState> SaveState(GameObject rootObject)
        {
            List<ObjectState> states = new List<ObjectState>();

            // Initialize saving of all dynamic objects that are direct children of the root object
            foreach (Transform child in rootObject.transform)
            {
                SaveObject dynamicObject = child.GetComponent<SaveObject>();

                if (dynamicObject == null || !dynamicObject.isActiveAndEnabled)
                    continue;

                dynamicObject.Save(states);
            }
            return states;
        }

        protected static void ApplyState(Dictionary<string, GameObject> prefabs, List<ObjectState> objectStates, GameObject rootObject)
        {
            ClearChildren(rootObject);

            foreach (ObjectState objectState in objectStates)
            {
                GameObject createdObject;
                SaveObject dynamicObject;

                // Do we have a prefab with the required guid?
                if (!prefabs.ContainsKey(objectState.prefabGuid))
                    throw new InvalidOperationException($"Prefab with guid {objectState.prefabGuid} not found.");

                // Instantiate the prefab at the specified position
                createdObject = UnityEngine.Object.Instantiate(prefabs[objectState.prefabGuid]);
                // Find the DynamicObject component and set the object state
                dynamicObject = createdObject.GetComponent<SaveObject>();

                dynamicObject.Load(objectState);

                // Attach the object to the root (it can get attached to a different parent later)
                createdObject.transform.SetParent(rootObject.transform);
            }
        }
        protected static void ClearChildren(GameObject root)
        {
            foreach (Transform child in root.transform)
                Destroy(child.gameObject);
        }

        protected static string SaveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UnitySaveLoad", "Saved");

        protected static string GetSaveFullPath(string filename)
        {
            return Path.Combine(SaveDir, filename + ".save");
        }
    }
}
