using System;
using UGUIDots.Render;
using UGUIDots.Transforms;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace UGUIDots.Conversions.Systems {

    [UpdateAfter(typeof(RectTransformConversionSystem))]
    [UpdateAfter(typeof(ImageConversionSystem))]
    [UpdateAfter(typeof(TMPTextConversionSystem))]
    public class CanvasConversionSystem : GameObjectConversionSystem {

        protected override void OnUpdate() {
            Entities.ForEach((Canvas canvas) => {

                var parent = canvas.transform.parent;
                if (parent != null) {
#if UNITY_EDITOR
                UnityEditor.EditorGUIUtility.PingObject(canvas);
#endif
                    throw new NotSupportedException($"{canvas.name} is child of {parent.name}, this is not supported!");
                }

                var entity       = GetPrimaryEntity(canvas);
                var canvasScaler = canvas.GetComponent<CanvasScaler>();

                DstEntityManager.RemoveComponent<Anchor>(entity);
                DstEntityManager.AddComponentData(entity, new DirtyTag { });
                DstEntityManager.AddSharedComponentData(entity, new CanvasSortOrder { Value = canvas.sortingOrder });

                // Add the root mesh renderering data to the canvas as the root primary renderer
                DstEntityManager.AddBuffer<CanvasVertexData>(entity);
                DstEntityManager.AddBuffer<CanvasIndexElement>(entity);

                // Add a mesh to the canvas so treat it as a renderer.
                DstEntityManager.AddComponentObject(entity, new Mesh());
                
                // Add a collection SubmeshMaterialIdx 
                DstEntityManager.AddBuffer<SubmeshMaterialIdxElement>(entity);

                switch (canvasScaler.uiScaleMode) {
                    case CanvasScaler.ScaleMode.ScaleWithScreenSize:
                        DstEntityManager.AddComponentData(entity, new ReferenceResolution {
                            Value = canvasScaler.referenceResolution
                        });

                        // TODO: Should figure out if I want to support shrinking and expanding only...
                        if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight) {
                            DstEntityManager.AddComponentData(entity, new WidthHeightRatio {
                                Value =  canvasScaler.matchWidthOrHeight
                            });
                        } else {
                            throw new NotSupportedException($"{canvasScaler.screenMatchMode} is not supported!");
                        }
                        break;
                    default:
                        throw new NotSupportedException($"{canvasScaler.uiScaleMode} is not supported!");
                }
            });
        }
    }
}
