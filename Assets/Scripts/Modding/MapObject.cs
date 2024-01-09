using ModIO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapObject : MonoBehaviour
{
    public Button Button { get { return button; } }

    [SerializeField] private Sprite loadingSprite;
    [SerializeField] private Button button;
    [SerializeField] private Image mapImage;
    [SerializeField] private TextMeshProUGUI mapText;

    public void SetMapLoading()
    {
        mapText.text = "";
        mapImage.sprite = loadingSprite;
    }

    public void SetMap(ModProfile mod)
    {
        mapText.text = mod.name;
        ModIOUnity.DownloadTexture(mod.logoImage_320x180, SetIcon);
    }

    void SetIcon(ResultAnd<Texture2D> resultAndTexture)
    {
        if (resultAndTexture.result.Succeeded() && resultAndTexture != null)
        {

            mapImage.sprite = TextureToSprite(resultAndTexture.value);
            mapImage.color = Color.white;
        }
        else
        {
            mapImage.sprite = loadingSprite;
        }
    }

    private static Sprite TextureToSprite(Texture2D texture)
    {
        var rect = new Rect(Vector2.zero, new Vector2(texture.width, texture.height));
        var ppi = 100;
        var spritemeshType = SpriteMeshType.FullRect;
        var sprite = Sprite.Create(texture, rect, Vector2.zero, ppi, 0, spritemeshType);
        return sprite;
    }
}
