using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UGUIDots {

    public class OrthographicRenderPass : ScriptableRenderPass {

        public Queue<(Mesh, Material, Matrix4x4, MaterialPropertyBlock)> InstructionQueue { get; private set; }
        private string profilerTag;

        public OrthographicRenderPass(OrthographicRenderSettings settings) {
            profilerTag          = settings.ProfilerTag;
            base.renderPassEvent = settings.RenderPassEvt;
            InstructionQueue     = new Queue<(Mesh, Material, Matrix4x4, MaterialPropertyBlock)>();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            var cmd = CommandBufferPool.Get(profilerTag);
            using (new ProfilingSample(cmd, profilerTag)) {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                // Set the projection view matrix
                var proj = Matrix4x4.Ortho(0, Screen.width, 0, Screen.height, -100f, 100f);
                var view = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                cmd.SetViewProjectionMatrices(view, proj);

                while (InstructionQueue.Count > 0) {
                    var tuple = InstructionQueue.Dequeue();
                    cmd.DrawMesh(tuple.Item1, tuple.Item3, tuple.Item2, 0, 0, tuple.Item4);
                }

            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [System.Serializable]
    public class OrthographicRenderSettings {
        public string ProfilerTag;
        public RenderPassEvent RenderPassEvt;

        public OrthographicRenderSettings() {
            ProfilerTag = "Orthographic Render Pass";
            RenderPassEvt = RenderPassEvent.AfterRenderingPostProcessing;
        }
    }

    public class OrthographicRenderFeature : ScriptableRendererFeature {

        public OrthographicRenderPass Pass { get; private set; }
        public OrthographicRenderSettings Settings = new OrthographicRenderSettings();

        public override void Create() {
            Pass = new OrthographicRenderPass(Settings);
            Pass.renderPassEvent = Settings.RenderPassEvt;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            renderer.EnqueuePass(Pass);
        }
    }
}