using System.Collections;
using UnityEngine;

public class FlashlightFader : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Renderer flashlightRend; // flashlight 
    [SerializeField] private Renderer coneRend;

    public void FadeAndDisable()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float time = 0f;
        Material[] flashlightMats = flashlightRend.materials;
        Material coneMat = coneRend.material;

        float initialOpacity = coneMat.GetFloat("_Base_Opacity");

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            foreach (Material mat in flashlightMats)
            {
                if (mat.name.Contains("FlashlightBulbMaterial"))
                {
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0f);
                }
                else
                {
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
                }
            }

            flashlightRend.materials = flashlightMats;

            // fade cone material
            float coneOpacity = Mathf.Lerp(initialOpacity, 0f, time / fadeDuration);
            coneMat.SetFloat("_Base_Opacity", coneOpacity);
            coneRend.material = coneMat;

            time += Time.deltaTime;
            yield return null;
        }

        flashlightRend.gameObject.SetActive(false);
        coneRend.gameObject.SetActive(false);
    }
}