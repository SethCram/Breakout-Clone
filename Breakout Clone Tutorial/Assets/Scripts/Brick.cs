using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public int hits;
    public int points;
    public Vector3 rotator;
    public Material hitMaterial;

    private Material _ogMaterial;
    private Renderer _renderer;

    private LocalGameManager localGameManager;

    // Start is called before the first frame update
    void Start()
    {
        localGameManager = transform.parent.parent.GetComponentInChildren<LocalGameManager>();

        _renderer = GetComponent<Renderer>();

        //original material set:
        _ogMaterial = _renderer.sharedMaterial;

        //offsets each individual brick's rotation:
        transform.Rotate(rotator * (transform.position.x + transform.position.y) * 0.1f); //each brick has dif position (acts like waves when scaled down
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotator * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {

        hits--;

        //brick destroyed:
        if (hits <= 0 )
        {
            localGameManager.Score += points; 
            Destroy(gameObject);
        }

        _renderer.sharedMaterial = hitMaterial; //change hit brick's material

        Invoke("RestoreMaterial", 0.05f); //calls specified method 0.05s after invoke
    }

    //curr material -> og material
    private void RestoreMaterial()
    {
        _renderer.sharedMaterial = _ogMaterial;
    }
}
