using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICursor : MonoBehaviour
{
    #region singleton
    public static UICursor I;

    void Awake()
    {
        if (I != null && I != this)
            Destroy(gameObject);
        else
            I = this;
    }
    #endregion

    public Sprite sprite;

    void Start()
    {
        sprite = GetComponent<Sprite>();
    }

    void Update()
    {
        
    }
}
