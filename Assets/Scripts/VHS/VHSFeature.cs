using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VHSFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class VHSSettings
    {
        public Material vhsMaterial = null;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public VHSSettings settings = new VHSSettings();
    private VHSRenderPass vhsPass;

    public override void Create()
    {
        if (settings.vhsMaterial != null)
        {
            vhsPass = new VHSRenderPass(settings.vhsMaterial)
            {
                renderPassEvent = settings.renderPassEvent
            };
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.vhsMaterial != null && vhsPass != null)
        {
            renderer.EnqueuePass(vhsPass);
        }
    }
}
