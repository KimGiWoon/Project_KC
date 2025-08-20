using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreviewRenderer : MonoBehaviour
{
    public Camera previewCamera;
    public RenderTexture renderTexture;

    public Sprite CapturePreview()
    {
        previewCamera.targetTexture = renderTexture;
        previewCamera.Render();

        RenderTexture.active = renderTexture;

        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        previewCamera.targetTexture = null;

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
    }
}
