using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class ItemBase :MonoBehaviour, IItemBase
{
    public enum State { Lock, CanTake, Disable }
    public State currentState = State.Lock;

    public BoxCollider boxCollider;
    public Rigidbody thisRigidbody;

    private UnityAction takeCalback;

    private void Awake()
    {
        boxCollider.enabled = false;
    }

    public void SetTakeItemCallback(UnityAction callback)
    {
        takeCalback = callback;
    }

    public void ChangeStateCanTake()
    {
        if (currentState == State.CanTake) return;

        currentState = State.CanTake;
        boxCollider.enabled = true;
        thisRigidbody.isKinematic = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (currentState == State.CanTake && other.CompareTag("Player"))
        {
            if(takeCalback != null)
            {
                takeCalback();
                takeCalback = null;
            }

            Destroy(this.gameObject);

        }
    }
}