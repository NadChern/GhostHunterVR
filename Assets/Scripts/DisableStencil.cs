using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DisableStencil : MonoBehaviour
{
    public UniversalRendererData rendererData;
    private LayerMask layerMask; // current settings with stencil mask enabled

    private void Start()
    {
        layerMask = rendererData.opaqueLayerMask;
    }

    public void disableStencil()
    {
        rendererData.opaqueLayerMask = ~0;
        rendererData.transparentLayerMask = ~0;
    }

    private void OnDestroy()
    {
        rendererData.opaqueLayerMask = layerMask;
        rendererData.transparentLayerMask = layerMask;
    }
}