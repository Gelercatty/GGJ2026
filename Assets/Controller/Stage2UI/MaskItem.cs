
using UnityEngine;
using UnityEngine.UI;
namespace GGJ2026
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UISilhouetteToggle : MonoBehaviour
    {
        public bool silhouette;
        public Shader silhouetteShader; // 指向上面那个 UI/SilhouetteMask

        private Image _img;
        private Material _silMat;
        private Material _origMat;

        private void Awake()
        {
            _img = GetComponent<Image>();
            _origMat = _img.material; // 可能为 null（用默认）
            if (!silhouetteShader) silhouetteShader = Shader.Find("UI/SilhouetteMask");
            _silMat = new Material(silhouetteShader);
            Apply();
        }

        private void OnValidate() => Apply();

        public void SetSilhouette(bool on)
        {
            silhouette = on;
            Apply();
        }

        private void Apply()
        {
            if (!_img) _img = GetComponent<Image>();
            if (!_img) return;

            if (silhouette)
                _img.material = _silMat;
            else
                _img.material = _origMat; // 回到原材质/默认
        }
    }

}


