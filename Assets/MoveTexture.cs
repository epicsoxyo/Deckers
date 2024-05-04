using UnityEngine;



public class MoveTexture : MonoBehaviour
{

    private Material _material;

    [SerializeField] private float speedX;
    [SerializeField] private float speedY;



    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }



    private void Update()
    {
        _material.mainTextureOffset = new Vector2
        (
            Time.time * speedX,
            Time.time * speedY
        );
    }

}