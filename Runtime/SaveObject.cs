using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fab.Save
{
    /// <summary>
    /// Component that saves the state of the owning <see cref="GameObject"/> 
    /// </summary>
    [AddComponentMenu("Fab/Save System/Save Object")]
    [DisallowMultipleComponent]
    public class SaveObject : MonoBehaviour
    {
        public delegate void LoadEventHandler(ObjectState objectState);
        public event LoadEventHandler OnLoad;
        public delegate void SaveEventHandler(ObjectState objectState);
        public event SaveEventHandler OnSave;

        [SerializeField]
        private ObjectState objectState;

        /// <summary>
        /// Returns the guid of this object's state.
        /// </summary>
        public string Guid => objectState.guid;

        private void Start()
        {
            if ((objectState != null) && objectState.guid.Equals(objectState.prefabGuid))
            {
                // Create a new guid for each prefab instantiation
                objectState.guid = ObjectState.CreateGuid();
            }
        }

        /// <summary>
        /// Loads an object state for the owning gameObject.
        /// </summary>
        /// <param name="objectState"></param>
        public void Load(ObjectState objectState)
        {
            this.objectState = objectState;

            gameObject.name = objectState.objectName;
            gameObject.tag = objectState.objectTag;
            gameObject.layer = objectState.objectLayer;

            Vector3 pos = new Vector3(
                Convert.ToSingle(objectState.xPos),
                Convert.ToSingle(objectState.yPos),
                Convert.ToSingle(objectState.zPos));

            Quaternion rot = Quaternion.Euler(
                Convert.ToSingle(objectState.xRot),
                Convert.ToSingle(objectState.yRot),
                Convert.ToSingle(objectState.zRot));

            Vector3 scale = new Vector3(
                Convert.ToSingle(objectState.xScale),
                Convert.ToSingle(objectState.yScale),
                Convert.ToSingle(objectState.zScale));

            transform.SetPositionAndRotation(pos, rot);
            transform.localScale = scale;

            StartCoroutine(LoadAfterFrame(objectState));
        }

        protected IEnumerator LoadAfterFrame(ObjectState objectState)
        {
            // Wait for the next frame so that all objects have been
            // created from objectStates
            yield return null; 
            OnLoad?.Invoke(objectState);
        }

        /// <summary>
        /// Saves the state of this gameObject and all its children.
        /// </summary>
        /// <param name="states"></param>
        public void Save(List<ObjectState> states)
        {
            PrepareToSave();

            // Loop through all the children and save them one after the other
            foreach (SaveObject child in gameObject.GetComponentsInChildren<SaveObject>(false))
            {
                if (child.gameObject != gameObject)
                    child.Save(states);
            }

            // Save this objects state into the list as well
            states.Add(objectState);
        }

        protected void PrepareToSave()
        {

            // add basic information to the objects state 
            objectState.objectName = gameObject.name;
            objectState.objectTag = gameObject.tag;
            objectState.objectLayer = gameObject.layer;

            // add transform information 
            Vector3 position = gameObject.transform.position;
            objectState.xPos = position.x;
            objectState.yPos = position.y;
            objectState.zPos = position.z;

            Vector3 euler = gameObject.transform.eulerAngles;
            objectState.xRot = euler.x;
            objectState.yRot = euler.y;
            objectState.zRot = euler.z;

            Vector3 scale = gameObject.transform.localScale;
            objectState.xScale = scale.x;
            objectState.yScale = scale.y;
            objectState.zScale = scale.z;

            // clear all generic values
            objectState.data.Clear();

            // invoke the delegate to let each subscriber 
            // add their data to the state
            OnSave?.Invoke(objectState);
        }
    }
}
