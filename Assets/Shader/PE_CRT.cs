using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PE_CRT : PostEffectBase
{
    public Shader BscShader;
    public Material BscMat;

    public float Curvature;

    private void OnRenderImage(RenderTexture src,RenderTexture dest)
    {
        Debug.Log("run2");
        Material _mat =CheckShaderAndMaterial(BscShader,BscMat);
        if(_mat==null)Graphics.Blit(src,dest);
        else{
            _mat.SetFloat("_Curvature",Curvature);

            Graphics.Blit(src,dest,_mat);
        }
    }
}
