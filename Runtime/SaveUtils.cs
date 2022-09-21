using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fab.Save
{
    /// <summary>
    /// Utility class for save and load operations.
    /// </summary>
    public static class SaveUtils
    {

        public static readonly string RootTagName = "DynamicRoot";

        public static GameObject GetRootSaveContainer()
        {
            return GameObject.FindGameObjectWithTag(RootTagName);
        }

        public static SaveObject FindSaveObjectByGuid(string guid)
        {
            SaveObject[] dynamicObjects = GetRootSaveContainer().GetComponentsInChildren<SaveObject>();
            foreach (SaveObject dynamicObject in dynamicObjects)
            {
                if (dynamicObject.Guid.Equals(guid))
                    return dynamicObject;
            }
            return null;
        }

        /// <summary>
        /// Gets the state guid from a gameObject. Returns null if the gameObject has no state.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static string GetGuid(GameObject gameObject)
        {
            if (gameObject && gameObject.TryGetComponent(out SaveObject dynamicObject))
                return dynamicObject.Guid;

            return null;
        }
    }
}
