using UnityEngine;
using System.Collections;
[RequireComponent(typeof(GUIText))]
public class FadingMessage : MonoBehaviour {
    const float DURATION = 2.5f;
    GUIText gt;
	// Use this for initialization
	void Awake () {
        gt = GetComponent<GUIText>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > DURATION)
        { Destroy(gameObject); }
        Color newColor = gt.material.color;
        float proportion = (Time.time / DURATION);
        newColor.a = Mathf.Lerp(1, 0, proportion);
        gt.material.color = newColor;
    }
}
