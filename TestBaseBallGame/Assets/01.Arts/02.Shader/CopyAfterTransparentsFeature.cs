// Assets/BashTest/CopyAfterTransparentsFeature.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;




public class CopyAfterTransparentsFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingTransparents; // UI까지면 AfterRendering
        public string globalTextureName = "_SceneColorAfterTransparents";
        public bool onlyBaseCamera = true;
    }

    class CopyRGPass : ScriptableRenderPass
    {
        readonly Settings settings;
        readonly int globalId;

        // 우리가 소유하는 외부 RTHandle (카메라 크기에 맞게 재할당)
        RTHandle _exposedRT;

        class PassData
        {
            public TextureHandle src;
            public TextureHandle dst;
            public int globalId;
            public RTHandle exposed; // 전역 바인딩용 실제 RT
        }

        public CopyRGPass(Settings s)
        {
            settings = s;
            renderPassEvent = s.injectionPoint;
            globalId = Shader.PropertyToID(string.IsNullOrEmpty(s.globalTextureName)
                ? settings.globalTextureName
                : s.globalTextureName);
        }

        public override void RecordRenderGraph(RenderGraph rg, ContextContainer frameData)
        {

            var camData = frameData.Get<UniversalCameraData>();
            if (settings.onlyBaseCamera && camData.renderType == CameraRenderType.Overlay)
                return;

            var res = frameData.Get<UniversalResourceData>();
            var src = res.activeColorTexture;

            // 카메라 디스크립터로 외부 RTHandle을 확보/리사이즈
            var desc = camData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateHandleIfNeeded(ref _exposedRT, desc, name: settings.globalTextureName);

            // 외부 RTHandle을 RG 리소스로 임포트 → 이 핸들을 렌더 타깃으로 사용
            var dstImported = rg.ImportTexture(_exposedRT);

            using var builder = rg.AddRasterRenderPass<PassData>("Copy After Transparents (RG)", out var passData);

            passData.src = src;            // 입력은 활성 컬러
            passData.dst = dstImported;    // 출력은 우리가 임포트한 외부 RT
            passData.globalId = globalId;
            passData.exposed = _exposedRT;

            // 컬러 0번 첨부
            builder.SetRenderAttachment(passData.dst, 0, AccessFlags.ReadWrite);

            // (버전에 따라 있으면) 읽기 선언 — 없어도 실행엔 문제 없음
            //builder.UseTexture(passData.src);
            builder.UseTexture(passData.src, AccessFlags.Read);   // 있으면 사용

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                // src → exposedRT 로 복사
                //Blitter.BlitTexture(ctx.cmd, data.src, Vector4.one, 0, false);
                Blitter.BlitTexture(ctx.cmd, data.src, new Vector4(1f, 1f, 0f, 0f), 0, false);

                // 우리가 가진 실제 RTHandle을 전역에 바인딩
                Shader.SetGlobalTexture(settings.globalTextureName, data.exposed);


            });
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // 필요 시 유지 — 프레임마다 재할당 비용을 줄이고 싶으면 Release 생략 가능
            //_exposedRT?.Release();
            //_exposedRT = null;
        }
    }

    public Settings settings = new Settings();
    CopyRGPass _pass;

    public override void Create() => _pass = new CopyRGPass(settings);

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        renderer.EnqueuePass(_pass);
    }
}
