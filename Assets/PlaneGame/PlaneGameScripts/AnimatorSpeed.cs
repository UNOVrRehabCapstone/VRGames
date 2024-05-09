﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanesGame{
public class AnimatorSpeed : MonoBehaviour {
    public float speed;
	// Use this for initialization
	void Start () {
       gameObject.GetComponent<Animator>().speed = speed;
    }
	
}}
