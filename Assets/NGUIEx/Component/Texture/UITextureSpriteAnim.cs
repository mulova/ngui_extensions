using UnityEngine;

[RequireComponent(typeof(UITexture)), ExecuteInEditMode]
public class UITextureSpriteAnim : MonoBehaviour
{
    public int columns = 5;
    public int rows = 5;
    public float speed = 30f;
    public float delay = 1f;
    private float time;
    private float delayTime;
    private int r = 0;
    private int c = -1;

    private UITexture tex;

    void Start()
    {
        tex = GetComponent<UITexture>();
        NextSprite();
    }

    public void Play()
    {
        enabled = true;
    }

    public void Stop()
    {
        enabled = false;
    }

    void Update()
    {
        if (delayTime < delay)
        {
            delayTime += Time.deltaTime;
            return;
        }
        time += Time.deltaTime * speed;
        if (time >= 1f)
        {
            time -= 1;
            NextSprite();
        }
    }

    [ContextMenu("Next")]
    private void NextSprite()
    {
        c++;
        if (c >= columns)
        {
            c = 0;
            r++;
            if (r >= rows)
            {
                delayTime = 0;
                r = 0;
            }
        }
        float width = 1f / columns;
        float height = 1f / rows;
        tex.uvRect = new Rect(c*width, 1-(r+1)*height, width, height);
    }
}