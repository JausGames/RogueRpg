using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PP_SRF_CellShading : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RenderTargetIdentifier source;
        private Material material;
        private RenderTargetHandle tempRenderTargetHandler;

        public CustomRenderPass(Material material)
        {
            this.material = material;
            tempRenderTargetHandler.Init("_TemporaryColorTexture");
        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }
        

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get("CustomBlitRenderPass");
            commandBuffer.GetTemporaryRT(tempRenderTargetHandler.id, renderingData.cameraData.cameraTargetDescriptor);
            Blit(commandBuffer, source, tempRenderTargetHandler.Identifier(), material);
            Blit(commandBuffer, tempRenderTargetHandler.Identifier(), source);
            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }
    [System.Serializable]
    public class Settings
    {
        public Material material = null;
    }

    public Settings settings = new Settings();

    CustomRenderPass m_ScriptablePass;
    public bool IsEnabled = false;
    public float PosterizeAmount = 4f;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings.material);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.source = renderer.cameraColorTarget;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


