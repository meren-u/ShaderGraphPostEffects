using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Meren.PostEffects
{
    public class ColorFilterRendererFeature : ScriptableRendererFeature
    {
        private ColorFilterRenderPass m_pass = null;

        public override void Create()
        {
            m_pass = new ColorFilterRenderPass();
        }
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
        {
            // Volumeコンポーネントを取得
            var volumeStack = VolumeManager.instance.stack;
            var volume = volumeStack.GetComponent<ColorFilter>();
            if (volume.IsActive())
            {
                m_pass.Setup(renderer.cameraColorTarget, volume);
                renderer.EnqueuePass(m_pass);
            }
        }
    }

    public class ColorFilterRenderPass : ScriptableRenderPass
    {
        private const string RenderPassName = nameof(ColorFilterRenderPass);
        private const string ProfilingSamplerName = "SrcToDest";

        private readonly int m_mainTexPropertyId = Shader.PropertyToID("_MainTex");
        private readonly Material m_material;
        private readonly ProfilingSampler m_profilingSampler;

        private RenderTargetHandle m_afterPostProcessTexture;
        private RenderTargetIdentifier m_source;
        private RenderTargetHandle m_tempRenderTargetHandle;
        private ColorFilter m_volume;

        public ColorFilterRenderPass()
        {
            var shader = Shader.Find("Shader Graphs/ColorFilter");
            if (shader == null)
                return;

            m_tempRenderTargetHandle.Init("_TempRT");

            m_profilingSampler = new ProfilingSampler(ProfilingSamplerName);

            m_material = CoreUtils.CreateEngineMaterial(shader);

            m_afterPostProcessTexture.Init("_AfterPostProcessTexture");
        }
        public void Setup(RenderTargetIdentifier source, ColorFilter volume)
        {
            m_source = source;
            m_volume = volume;
            renderPassEvent = RenderPassEvent.AfterRendering;
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!CanExecuted(ref renderingData))
            {
                return;
            }

            var source = m_afterPostProcessTexture.Identifier();
            var cmd = CommandBufferPool.Get(RenderPassName);
            cmd.Clear();

            var tempTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            tempTargetDescriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(m_tempRenderTargetHandle.id, tempTargetDescriptor);

            using (new ProfilingScope(cmd, m_profilingSampler))
            {
                m_material.SetColor("_FilterColor", m_volume.m_filterColor.value);
                cmd.SetGlobalTexture(m_mainTexPropertyId, source);
                Blit(cmd, source, m_tempRenderTargetHandle.Identifier(), m_material);
            }

            Blit(cmd, m_tempRenderTargetHandle.Identifier(), source);

            cmd.ReleaseTemporaryRT(m_tempRenderTargetHandle.id);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        private bool CanExecuted(ref RenderingData renderingData)
        {
            if (m_material == null)
                return false;

            if (!m_volume.IsActive())
                return false;

            // カメラのポストプロセス設定が無効になっていたら何もしない
            if (!renderingData.cameraData.postProcessEnabled)
                return false;

            return true;
        }
    }
}