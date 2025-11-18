using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VHSRenderPass : ScriptableRenderPass
{
    private Material vhsMaterial;
    private int tempTextureID = Shader.PropertyToID("_TemporaryColorTexture");

    public VHSRenderPass(Material mat)
    {
        vhsMaterial = mat;
    }

    // Убираем Setup(), camera target получаем внутри Execute()
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (vhsMaterial == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("VHSEffect");

        // Берем RTHandle камеры и приводим к RenderTargetIdentifier
        RTHandle cameraColorHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RenderTargetIdentifier cameraRT = cameraColorHandle; // явное преобразование

        // Создаем временный RT
        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        cmd.GetTemporaryRT(tempTextureID, opaqueDesc);

        // Блитим эффект
        cmd.Blit(cameraRT, tempTextureID, vhsMaterial);
        cmd.Blit(tempTextureID, cameraRT);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }


}
