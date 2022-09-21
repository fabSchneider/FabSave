using System;
using System.Collections.Generic;

namespace Fab.Save
{
    /// <summary>
    /// Holds generic state data identifiable by a key.
    /// </summary>
    [Serializable]
    public class ObjectStateData : Dictionary<string, object> { }

    /// <summary>
    /// Holds the state of a game object
    /// </summary>
    [Serializable]
    public class ObjectState
    {
        public string guid;
        public string prefabGuid;

        public string objectName;
        public string objectTag;
        public int objectLayer;

        public float xPos;
        public float yPos;
        public float zPos;

        public float xRot;
        public float yRot;
        public float zRot;

        public float xScale;
        public float yScale;
        public float zScale;

        public ObjectStateData data;

        public ObjectState()
        {
            if (string.IsNullOrEmpty(guid))
                guid = CreateGuid();

            prefabGuid = guid;

            data = new ObjectStateData();
        }

        /// <summary>
        /// Returns a new unique identifier applicable of an object state.
        /// </summary>
        /// <returns></returns>
        public static string CreateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Returns true if the state contains an entry with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return data.ContainsKey(key);
        }

        /// <summary>
        /// Adds a value with the given key to the state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add<T>(string key, T value)
        {
            data.Add(key, value);
        }

        /// <summary>
        /// Returns the value for the given key as a boolean.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBool(string key)
        {
            return Convert.ToBoolean(data[key]);
        }

        /// <summary>
        /// Returns the value for the given key as an integer.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt(string key)
        {
            return Convert.ToInt32(data[key]);
        }

        /// <summary>
        /// Returns the value for the given key as a float.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float GetFloat(string key)
        {
            return Convert.ToSingle(data[key]);
        }

        /// <summary>
        /// Returns the value for the given key as a string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            object val = data[key];

            if (val == null)
                return null;

            return Convert.ToString(val);
        }

        /// <summary>
        /// Returns the value for the given key as a boolean array.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool[] GetBoolArray(string key)
        {
            return Array.ConvertAll((object[])data[key], x => Convert.ToBoolean(x));
        }

        /// <summary>
        /// Returns the value for the given key as an integer array.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int[] GetIntArray(string key)
        {
            return Array.ConvertAll((object[])data[key], x => Convert.ToInt32(x));
        }

        /// <summary>
        /// Returns the value for the given key as a float array.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float[] GetFloatArray(string key)
        {
            return Array.ConvertAll((object[])data[key], x => Convert.ToSingle(x));
        }

        /// <summary>
        /// Returns the value for the given key as a string array.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string[] GetStringArray(string key)
        {
            return Array.ConvertAll((object[])data[key], x =>
            {
                object val = data[key];

                if (val == null)
                    return null;

                return Convert.ToString(val);
            });
        }

    }
}
