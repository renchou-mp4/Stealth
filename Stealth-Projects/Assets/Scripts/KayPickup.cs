using UnityEngine;
using System.Collections;

public class KayPickup : MonoBehaviour 
{
    public AudioClip KeyGrab;

    private GameObject player;
    private PlayerInventory playerInventory;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerInventory = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<PlayerInventory>();
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject == player)
        {
            AudioSource.PlayClipAtPoint(KeyGrab, transform.position);
            playerInventory.hasKey = true;
            Destroy(gameObject);
        }
    }

}
