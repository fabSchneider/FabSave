# Fab Save 

A simple, GameObject based save system for Unity.

<img src="Documentation~/SaveBert.gif"/>

Current features of version 0.1.0 include:
- SaveGameObject data to json.
- Load json data and instantiate objects with the loaded data.
- Callbacks for saving and loading custom data
- Save references to other scene objects

# Usage

You can define how a MonoBehaviour's state is saved and loaded by subscribing to the `OnLoad` and `OnSave` events of a `SaveObject` component.

````csharp
public class Item : MonoBehaviour
{
    public int cost;
    public GameObject owner;

    private void OnEnable()
    {
        // Subscribe to OnLoad and OnSave callbacks 
        // when the behaviour is enabled.
        SaveObject saveObject = GetComponent<SaveObject>();
        saveObject.OnSave += OnSave;
        saveObject.OnLoad += OnLoad;
    }

    private void OnDisable()
    {
        // Unsubscribe to OnLoad and OnSave callbacks 
        // when the behaviour is disabled so it won't 
        // be saved if it is inactive.
        SaveObject saveObject = GetComponent<SaveObject>();
        saveObject.OnSave -= OnSave;
        saveObject.OnLoad -= OnLoad;
    }

    // OnSave is called before all saved states are written to a file. 
    private void OnSave(ObjectState objectState)
    {
        // Add the data worth saving to the object state here.
        objectState.Add("cost", cost);
        objectState.Add("owner", SaveUtils.GetGuid(owner));
    }

    // On Load is called after the all states have been loaded
    // and saved objects have been instantiated in the scene.
    private void OnLoad(ObjectState objectState)
    {
        // Retrieve and apply the loaded state here.
        cost = objectState.GetInt("cost");
        string ownerGuid = objectState.GetString("owner");
        owner = SaveUtils.FindSaveObjectByGuid(ownerGuid).gameObject
    }
}
````
