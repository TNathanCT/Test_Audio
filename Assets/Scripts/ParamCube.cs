﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{

    public int _band;
    public float _startScale, _scaleMultiplier;
    public bool _useBuffer;
    
    /* 
    void Update()
    {
        if(_useBuffer){
            transform.localScale = new Vector3(transform.localScale.x, (AudioPeer._bandbuffer[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
        }

        else if (!_useBuffer){
            transform.localScale = new Vector3(transform.localScale.x, (AudioPeer._freqBand[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);

        }
    
    }*/

}
