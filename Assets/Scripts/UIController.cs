using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{

    public TMP_Text f3;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            f3.gameObject.SetActive(!f3.gameObject.activeSelf);
        }
        if(f3.gameObject.activeSelf == true)
        {
            f3.text = "XYZ: " + player.transform.position.x + " / " + player.transform.position.y + " / " + player.transform.position.z;
        }
    }
}
