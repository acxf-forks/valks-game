using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INoiseFilter
{
    bool Enabled { get; set; }
    float Evaluate(Vector3 point);
}
