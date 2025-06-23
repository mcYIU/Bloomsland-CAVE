using UnityEngine;

public class Transformer : MonoBehaviour
{
    public Transform parentObject; 

    private void Start()
    {
        transform.parent = parentObject; 
        gameObject.transform.position = Vector3.zero; 
    }
}
