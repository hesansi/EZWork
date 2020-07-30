using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

class EZUVAnimation : MonoBehaviour
{
    public int Columns = 5;
    public int Rows = 5;
    public float FramesPerSecond = 10f;
    public bool RunOnce = true;
    private Renderer renderer;
    private float randomDelay;

    public float RunTimeInSeconds
    {
        get { return ((1f / FramesPerSecond) * (Columns * Rows)); }
    }

    private Material materialCopy = null;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        // Copy its material to itself in order to create an instance not connected to any other
        materialCopy = new Material(renderer.sharedMaterial);
        renderer.sharedMaterial = materialCopy;

        Vector2 size = new Vector2(1f / Columns, 1f / Rows);
        renderer.sharedMaterial.SetTextureScale("_MainTex", size);

        randomDelay = Random.Range(0, 0.5f);
    }

    void OnEnable()
    {
        StartCoroutine(UpdateTiling());
    }

    private IEnumerator UpdateTiling()
    {
        float x = 0f;
        float y = 0f;
        Vector2 offset = Vector2.zero;

        randomDelay -= Time.deltaTime;
        if (randomDelay > 0)
            yield return null;
        while (true) {
            for (int i = Rows - 1; i >= 0; i--) // y
            {
                y = (float) i / Rows;

                for (int j = 0; j <= Columns - 1; j++) // x
                {
                    x = (float) j / Columns;

                    offset.Set(x, y);

                    renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
                    yield return new WaitForSeconds(1f / FramesPerSecond);
                }
            }

            if (RunOnce) {
                yield break;
            }
        }
    }
}