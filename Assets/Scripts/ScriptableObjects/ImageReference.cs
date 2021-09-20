using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new image reference", menuName = "Scriptables/ImageReference")]
public class ImageReference : ScriptableObject {
    public Sprite image;
}