using System.Collections;
using System.Collections.Generic;
using EZWork;
using UnityEngine;
using UnityEngine.EventSystems;

public class EZOutline : MonoBehaviour
{    
    private SpriteRenderer spRender;
    private MaterialPropertyBlock spMatBlock;
    private int toggleID, colorID;
    private bool isOutlineShowing = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Material mat = new Material(Shader.Find("EZShader/SpriteOutline"));
        spRender = GetComponent<SpriteRenderer>();
        spRender.material = mat;
        
        spMatBlock = new MaterialPropertyBlock();
        toggleID = Shader.PropertyToID("_OutlineToggle");
        colorID = Shader.PropertyToID("_OutlineColor");
    }

    public void SetOutlineColor(Color color)
    {
        spRender.GetPropertyBlock(spMatBlock);
        spMatBlock.SetVector(colorID, color);
        spRender.SetPropertyBlock(spMatBlock);
    }

    public void Show()
    {
        spRender.GetPropertyBlock(spMatBlock);
        spMatBlock.SetInt(toggleID,1);
        spRender.SetPropertyBlock(spMatBlock);
    }
    
    public void Hide()
    {
        spRender.GetPropertyBlock(spMatBlock);
        spMatBlock.SetInt(toggleID,0);
        spRender.SetPropertyBlock(spMatBlock);
    }

}
