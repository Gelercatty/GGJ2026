using UnityEngine;

public class PE_Brightness : PostEffectBase
{
    public Shader BscShader;
    public Material BscMat;

    [Range(0.0f, 3.0f)]
    public float Brightness = 1;
    [Range(0.0f, 3.0f)]
    public float Saturation = 0.5f;
    [Range(0.0f, 3.0f)]
    public float Contrast = 0.5f;
    
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Material _mat = CheckShaderAndMaterial(BscShader, BscMat);
        if (null == _mat) Graphics.Blit(src, dest);
        else
        {
            _mat.SetFloat("_Brightness", Brightness);
            _mat.SetFloat("_Saturation", Saturation);
            _mat.SetFloat("_Contrast", Contrast);
            
            Graphics.Blit(src, dest, _mat);
        }


    }
}

