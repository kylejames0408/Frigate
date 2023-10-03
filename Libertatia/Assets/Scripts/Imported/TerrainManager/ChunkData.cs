using UnityEngine;

public class ChunkData
{
    /// <summary>
    /// Размер объекта tarrain’а по его оси X.
    /// </summary>
    public float Width {
        get {
            return width;
        }

        set {
            width = value;
        }
    }
    private float width = 500;
    /// <summary>
    /// Размер объекта tarrain’а по его оси Z.
    /// </summary>
    public float Length {
        get {
            return length;
        }

        set {
            length = value;
        }
    }
    private float length = 500;
    /// <summary>
    /// Разница по оси Y между значениями самой нижней и самой верхней точек карты высот.
    /// </summary>
    public float Height {
        get {
            return height;
        }

        set {
            height = value;
        }

    }
    private float height = 600;
    /// <summary>
    /// Разрешение карты высот terrain’а в пикселях
    /// </summary>
    public int HeigthmapResolution {
        get {
            return heigthmapResolution;
        }

        set {           
            heigthmapResolution = Mathf.Clamp(value + 1, 33, 4097);
        }
    }
    private int heigthmapResolution = 512;
    /// <summary>
    /// Разрешение карты, которая определяет отдельные островки деталей/травы. Чем выше разрешение, тем меньше и более детальны будут островки.
    /// </summary>
    public int DetailResolution {
        get {
            return detailResolution;
        }

        set {
            detailResolution = Mathf.Clamp(value, 0, 4096);
        }
    }
    private int detailResolution = 1024;
    /// <summary>
    /// Длина/ширина квадрата островков, отрисовываемого за один draw call.
    /// </summary>
    public int DetailResolutioinPerPatch {
        get {
            return detailResolutioinPerPatch;
        }

        set {
            detailResolutioinPerPatch = Mathf.Clamp(value, 8, 128);
        }
    }
    private int detailResolutioinPerPatch = 8;
    /// <summary>
    /// Разрешение карты “splatmap”, которая контролирует смешивание разных текстур terrain’а.
    /// </summary>
    public int ControlTextureResolution {
        get {
            return controlTextureResolution;
        }

        set {
            controlTextureResolution = Mathf.Clamp(value, 16, 1024);
        }
    }
    private int controlTextureResolution = 512;
    /// <summary>
    /// Разрешение композитной текстуры, используемой на terrain’е когда вы смотрите на него с расстояния, большего чем значение Basemap Distance.
    /// </summary>
    public int BaseTextureResolution {
        get {
            return baseTextureResolution;
        }

        set {
            baseTextureResolution = Mathf.Clamp(value, 16, 2048);
        }
    }
    private int baseTextureResolution = 1024;

    public string Path
    {
        get {
            return path;
        }

        set {
            path = value;
        }
    }
    private string path = "";

    public ChunkData(Vector3 size, int heightmap, int detail, int perPatch, int controlTexture, int baseTexture, string path)
    {
        Width  = size.x;
        Length = size.z;
        Height = size.y;
        HeigthmapResolution = heightmap;
        DetailResolution = detail;
        DetailResolutioinPerPatch = perPatch;
        ControlTextureResolution  = controlTexture;
        BaseTextureResolution = baseTexture;
        Path = path;









    }
}
