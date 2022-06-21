using System.Collections;
using System.Collections.Generic;
using LeTai.Asset.TranslucentImage;
using UnityEngine;

public class SetTransparentImageBlurSourceAtAwake : MonoBehaviour
{
    void Awake()
    {
        gameObject.GetComponentRequired<TranslucentImage>().source = Camera.main.GetComponentRequired<TranslucentImageSource>();
    }
}
