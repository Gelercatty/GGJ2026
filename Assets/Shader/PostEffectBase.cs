using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PostEffectBase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CheckResource();
    }

    // Update is called once per frame
    void CheckResource()
    {
        bool _supported = CheckDefaultSupported()&&CheckSpecificSupported();
        if(!_supported)OnNotSupported();
    }
    protected virtual bool CheckDefaultSupported()
    {
        return true;
    }
    protected virtual bool CheckSpecificSupported()
    {
        return true;
    }
    void OnNotSupported()
    {
        enabled =false;
    }
    protected Material CheckShaderAndMaterial(Shader _shader,Material _material)
    {
        Debug.Log("run!");
        if(null==_shader||!_shader.isSupported)return null;
        else
        {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }
        if(_material){
            Debug.Log(_material);
            return _material;
        }
        else 
            return null;
    }

}
