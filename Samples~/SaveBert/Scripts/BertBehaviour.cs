using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace Fab.Save.Samples
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BertBehaviour : MonoBehaviour
    {
        private NavMeshAgent navAgent;
        private UIDocument ui;

        public Transform hatSlot;
        public GameObject hat;

        private void OnEnable()
        {
            SaveObject saveObject = GetComponent<SaveObject>();
            if (saveObject)
            {
                saveObject.OnSave += OnSave;
                saveObject.OnLoad += OnLoad;
            }
        }

        private void OnDisable()
        {
            SaveObject saveObject = GetComponent<SaveObject>();
            if (saveObject)
            {
                saveObject.OnSave -= OnSave;
                saveObject.OnLoad -= OnLoad;
            }
        }

        private void Start()
        {
            navAgent = GetComponent<NavMeshAgent>();
            ui = FindObjectOfType<UIDocument>();


        }


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // ensure that pointer is not over UI
                if(ui && ui.rootVisualElement.panel.Pick(RuntimePanelUtils.ScreenToPanel(ui.rootVisualElement.panel, Input.mousePosition)) == null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if(Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if(hit.collider.gameObject.tag == "Hat")
                        {
                            if (hat)
                                UnEquipHat();
                            else
                            {
                                if((transform.position - hit.collider.gameObject.transform.position).magnitude < 2f)
                                    EquipHat(hit.collider.gameObject);
                            }
                        }

                        navAgent.SetDestination(hit.point);
                    } 
                }
            }
        }

        public void EquipHat(GameObject hat)
        {
            hat.GetComponent<Rigidbody>().isKinematic = true;
            hat.transform.SetParent(hatSlot, false);
            hat.transform.localPosition = Vector3.zero;
            hat.transform.localRotation = Quaternion.identity;
            this.hat = hat;
        }

        public void UnEquipHat()
        {
            if (hat)
            {
                hat.GetComponent<Rigidbody>().isKinematic = false;
                hat.transform.SetParent(SaveUtils.GetRootSaveContainer().transform);
                hat = null;
            }
        }

        private void OnSave(ObjectState objectState)
        {
            objectState.Add("hat", SaveUtils.GetGuid(hat));
        }

        private void OnLoad(ObjectState objectState)
        {
            string hatGuid = objectState.GetString("hat");
            if (hatGuid == null)
            {
                hat = null;
            }
            else
            {
                GameObject hat = SaveUtils.FindSaveObjectByGuid(hatGuid).gameObject;
                EquipHat(hat);
            }
        }
    }
}
