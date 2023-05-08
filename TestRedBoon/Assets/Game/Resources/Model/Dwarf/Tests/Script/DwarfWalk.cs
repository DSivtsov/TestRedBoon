using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfWalk : MonoBehaviour
{
    [Range(-1f,5f)]
    [SerializeField] private float Zspeed;

    CharacterController _dwarf;

    private void Awake()
    {
        _dwarf = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (System.Math.Abs(Zspeed) < 0.1f) Zspeed = 0;

        //Debug.Log((transform.rotation * new Vector3(0, 0, 1f / 50f)).ToString("F3"));
        //transform.position += transform.rotation * new Vector3(0, 0, Zspeed / 50f) ;
        _dwarf.Move(transform.rotation * new Vector3(0, 0, Zspeed ) * Time.deltaTime);
    }
}
