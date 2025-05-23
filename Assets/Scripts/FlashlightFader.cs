using System.Collections;
using UnityEngine;

public class FlashlightFader : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Renderer rend; // the flashlight model

    public void FadeAndDisable()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float time = 0f;
        Material[] mats = rend.materials;

        while (time < fadeDuration)
        {   
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            foreach (Material mat in mats)
            {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
            }
            rend.materials = mats;
            time += Time.deltaTime;
            yield return null;
        }

        rend.gameObject.SetActive(false);
    }
}