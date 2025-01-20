using UnityEngine;
using UnityEngine.UI;

public class DynamicTexture : MonoBehaviour
{
    public Texture2D texture { get; private set; } // 动态生成的纹理

    public int Width => texture.width;
    public int Height => texture.height;

    private RawImage rawImage; // 用于显示纹理的 RawImage 组件

    /// <summary>
    /// 初始化 Texture2D，动态设置大小，并创建 RawImage 组件显示纹理。
    /// </summary>
    /// <param name="width">纹理的宽度</param>
    /// <param name="height">纹理的高度</param>
    public void InitializeTexture(int width, int height)
    {
        // 初始化纹理
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // 默认填充为黑色
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        texture.wrapMode = TextureWrapMode.Clamp;
        SetFilterMode(FilterMode.Point);

        // 初始化 RawImage 并显示纹理
        InitializeRawImage();
    }

    public void SetFilterMode(FilterMode filterMode)
    {
        texture.filterMode = filterMode;
    }

    /// <summary>
    /// 初始化 RawImage 组件，并将纹理赋值给它。
    /// </summary>
    private void InitializeRawImage()
    {
        // 确保挂载了 RectTransform 组件（RawImage 依赖于它）
        if (gameObject.GetComponent<RectTransform>() == null)
        {
            gameObject.AddComponent<RectTransform>();
        }

        // 自动添加 RawImage 组件
        rawImage = gameObject.GetComponent<RawImage>();
        if (rawImage == null)
        {
            rawImage = gameObject.AddComponent<RawImage>();
        }

        // 设置 RawImage 的纹理
        rawImage.texture = texture;

        // 调整 RawImage 的尺寸以适应纹理大小
        RectTransform rectTransform = rawImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
    }

    public void Clear()
    {
        Clear(Color.black);
    }

    public void Clear(Color color)
    {
        Color[] pixels = new Color[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    public void CopyFrom(Texture2D source)
    {
        if (source.width != texture.width || source.height != texture.height)
        {
            Debug.LogWarning(
                $"Texture size mismatch: source width={source.width}, source height={source.height}"
            );
            Debug.LogError("Source texture is not the same size as the target texture.");
            return;
        }
        texture.SetPixels(source.GetPixels());
        texture.Apply();
    }

    public void CopyFrom(Texture2D source, int x, int y)
    {
        // 检查目标位置是否有效
        if (x < 0 || y < 0 || x >= texture.width || y >= texture.height)
        {
            Debug.LogError("Invalid position for source texture in target texture.");
            return;
        }

        // 确保目标纹理区域有足够的空间容纳源纹理
        if (x + source.width > texture.width || y + source.height > texture.height)
        {
            Debug.LogWarning(
                "Source texture will exceed target texture boundaries. Clipping source texture."
            );
        }

        // 获取源纹理的像素数据
        Color[] sourcePixels = source.GetPixels();

        // 将源纹理的像素数据复制到目标纹理的指定位置
        texture.SetPixels(x, y, source.width, source.height, sourcePixels);

        // 应用修改
        texture.Apply();
    }

    public void CopyFrom(RenderTexture source)
    {
        // 如果源 RenderTexture 的大小与目标 Texture2D 不匹配，返回
        if (source.width != texture.width || source.height != texture.height)
        {
            Debug.LogWarning(
                $"Texture size mismatch: source width={source.width}, source height={source.height}"
            );
            Debug.LogError("Source RenderTexture is not the same size as the target texture.");
            return;
        }

        // 创建一个临时的 Texture2D 对象来存储 RenderTexture 的像素
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = source;

        // 使用 ReadPixels 将 RenderTexture 的内容读取到 Texture2D 中
        texture.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        texture.Apply();

        // 恢复原来的 active RenderTexture
        RenderTexture.active = currentActiveRT;
    }

    /// <summary>
    /// 设置以左上角为原点的像素颜色。
    /// </summary>
    /// <param name="x">左上角坐标系的 X 坐标</param>
    /// <param name="y">左上角坐标系的 Y 坐标</param>
    /// <param name="color">要设置的颜色</param>
    public void SetPixel(int x, int y, Color color)
    {
        if (texture == null)
        {
            Debug.LogError("Texture has not been initialized.");
            return;
        }

        // 将左上角坐标系的 (x, y) 映射到 Unity 的左下角坐标系
        int unityY = texture.height - 1 - y;

        // Debug.Log($"texture width: {texture.width}, texture height: {texture.height}");
        // Debug.Log($"x: {x}, y: {y}, unityY: {unityY}");
        // 检查是否在纹理范围内
        if (x < 0 || x >= texture.width || unityY < 0 || unityY >= texture.height)
        {
            Debug.LogError("Coordinates out of bounds!");
            return;
        }

        // 设置像素颜色
        texture.SetPixel(x, unityY, color);
        texture.Apply();
    }

    /// <summary>
    /// 获取以左上角为原点的像素颜色。
    /// </summary>
    /// <param name="x">左上角坐标系的 X 坐标</param>
    /// <param name="y">左上角坐标系的 Y 坐标</param>
    /// <returns>指定像素的颜色</returns>
    public Color GetPixel(int x, int y)
    {
        if (texture == null)
        {
            Debug.LogError("Texture has not been initialized.");
            return Color.clear;
        }

        // 将左上角坐标系的 (x, y) 映射到 Unity 的左下角坐标系
        int unityY = texture.height - 1 - y;

        // 检查是否在纹理范围内
        if (x < 0 || x >= texture.width || unityY < 0 || unityY >= texture.height)
        {
            Debug.LogError("Coordinates out of bounds!");
            return Color.clear;
        }

        // 获取像素颜色
        return texture.GetPixel(x, unityY);
    }

    /// <summary>
    /// 填充整个纹理为指定颜色。
    /// </summary>
    /// <param name="color">要填充的颜色</param>
    public void FillTexture(Color color)
    {
        if (texture == null)
        {
            Debug.LogError("Texture has not been initialized.");
            return;
        }

        Color[] pixels = new Color[texture.width * texture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    /// <summary>
    /// 填充以左上角为原点的矩形区域。
    /// </summary>
    /// <param name="startX">矩形起始的 X 坐标</param>
    /// <param name="startY">矩形起始的 Y 坐标</param>
    /// <param name="width">矩形的宽度</param>
    /// <param name="height">矩形的高度</param>
    /// <param name="color">要填充的颜色</param>
    public void FillRectangle(int startX, int startY, int width, int height, Color color)
    {
        if (texture == null)
        {
            Debug.LogError("Texture has not been initialized.");
            return;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 将左上角坐标映射到 Unity 的左下角坐标系
                int unityY = texture.height - 1 - (startY + y);

                // 检查边界
                if (
                    startX + x < 0
                    || startX + x >= texture.width
                    || unityY < 0
                    || unityY >= texture.height
                )
                {
                    continue;
                }

                // 设置像素颜色
                texture.SetPixel(startX + x, unityY, color);
            }
        }
        texture.Apply();
    }
}
