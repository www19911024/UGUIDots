using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace UGUIDots.Conversions.Systems {

    [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    public class VisualAssetsDeclarationSystem : GameObjectConversionSystem {
        protected override void OnUpdate() {
            Entities.ForEach((Image img) => {
                var mat = img.material != null ? img.material : 
                    Canvas.GetDefaultCanvasMaterial();
                DeclareReferencedAsset(mat);

                var texture = img.sprite != null ? img.sprite.texture : Texture2D.whiteTexture;
                DeclareReferencedAsset(texture);
            });

            Entities.ForEach((TextMeshProUGUI text) => {
                var mat = text.materialForRendering != null ? text.materialForRendering : 
                    Canvas.GetDefaultCanvasMaterial();

                DeclareReferencedAsset(mat);
            });
        }
    }

    public class VisualAssetsConversionSystem : GameObjectConversionSystem {
        protected override void OnUpdate() {
            Entities.ForEach((Image img) => {
                CreateTextureEntity(img);
                CreateMaterialEntity(img);
            });

            Entities.ForEach((TextMeshProUGUI text) => {
                CreateMaterialEntity(text);
            });
        }

        private void CreateTextureEntity(Image img) {
            var texture = img.sprite != null ? img.sprite.texture : Texture2D.whiteTexture;
            var linkedTexture = GetPrimaryEntity(texture);

            DstEntityManager.AddComponentObject(linkedTexture, texture);
        }

        private void CreateMaterialEntity(Image img) {
            var mat = img.material != null ? img.material : Canvas.GetDefaultCanvasMaterial();
            var linkedMaterial = GetPrimaryEntity(mat);

            DstEntityManager.AddComponentObject(linkedMaterial, mat);
        }

        private void CreateMaterialEntity(TextMeshProUGUI text) {
            var mat = text.materialForRendering != null ? text.materialForRendering : 
                Canvas.GetDefaultCanvasMaterial();

            var linkedMaterial = GetPrimaryEntity(mat);
            DstEntityManager.AddComponentObject(linkedMaterial, mat);
        }
    }
}
