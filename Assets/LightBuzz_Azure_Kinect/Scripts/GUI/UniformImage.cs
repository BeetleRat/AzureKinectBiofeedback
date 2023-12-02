using System;
using UnityEngine;
using UnityEngine.UI;

namespace LightBuzz.Kinect4Azure
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(AspectRatioFitter))]
    [Serializable]
    public class UniformImage : MonoBehaviour
    {
        [SerializeField] protected bool _flipVertically = true;
        [SerializeField] protected bool _flipHorizontally = false;

        public bool FlipVertically
        {
            get => _flipVertically;
            set => _flipVertically = value;
        }

        public bool FlipHorizontally
        {
            get => _flipHorizontally;
            set => _flipHorizontally = value;
        }

        protected RectTransform rectTransform;
        protected Vector3 initialScale;
        protected Canvas canvas;
        protected Image image;
        protected AspectRatioFitter arf;
        protected Sprite sprite;
        protected Texture2D texture;

        protected DepthVisualization depthVisualization;

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public void Load(ColorFrameSource source)
        {
            if (source == null) return;

            Load(source.Data, source.Width, source.Height, source.Format);
        }

        public void Load(DepthFrameSource source, ushort maxDepth = 8000, DepthVisualization visualization = DepthVisualization.Jet)
        {
            if (source == null) return;

            Load(source.Data, source.Width, source.Height, maxDepth, visualization);
        }

        public void Load(byte[] data, int width, int height, TextureFormat format = TextureFormat.RGB24)
        {
            if (data == null || data.Length == 0) return;

            Create(width, height, format);

            if (data != null && data.Length > 0)
            {
                texture.LoadRawTextureData(data);
                texture.Apply();
            }
        }

        public void Load(Color32[] data, int width, int height, TextureFormat format = TextureFormat.RGBA32)
        {
            if (data == null || data.Length == 0) return;

            Create(width, height, format);

            texture.SetPixels32(data);
            texture.Apply();
        }

        public void Load(ushort[] data, int width, int height, ushort maxDepth = 8000, DepthVisualization visualization = DepthVisualization.Jet)
        {
            if (data == null || data.Length == 0) return;
            
            depthVisualization = visualization;

            const int channels = 3;

            float minOld = 0.0f;
            float maxOld = (float)maxDepth;
            float minNew = depthVisualization == DepthVisualization.Jet ? -1.0f : 0.0f;
            float maxNew = depthVisualization == DepthVisualization.Jet ? 1.0f : 255.0f;

            byte[] pixels = new byte[data.Length * channels];

            System.Threading.Tasks.Parallel.For(0, data.Length, i =>
            {
                ushort depth = data[i];
                float value = (float)depth;

                float red = 0.0f;
                float green = 0.0f;
                float blue = 0.0f;

                if (depth != 0)
                {
                    if (depthVisualization == DepthVisualization.Jet)
                    {
                        float t = (((value - minOld) * (maxNew - minNew)) / (maxOld - minOld)) + minNew;

                        red = Mathf.Clamp01(1.5f - Mathf.Abs(2.0f * t - 1.0f)) * 255.0f;
                        green = Mathf.Clamp01(1.5f - Mathf.Abs(2.0f * t)) * 255.0f;
                        blue = Mathf.Clamp01(1.5f - Mathf.Abs(2.0f * t + 1.0f)) * 255.0f;
                    }
                    else
                    {
                        red = green = blue = value / maxOld * maxNew;
                    }
                }

                pixels[i * channels + 0] = (byte)red;
                pixels[i * channels + 1] = (byte)green;
                pixels[i * channels + 2] = (byte)blue;
            });

            Load(pixels, width, height);
        }

        /// <summary>
        /// Calculates the position in the local space of the image.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 GetPosition(Vector2 point)
        {
            // Calculate local position
            Vector3 pos = Vector3.zero;
            Transform t = transform;
            bool hasParent = t.parent != null;
            while (hasParent)
            {
                pos += t.localPosition;
                t = t.parent;
                hasParent = t.parent != null;
            }

            float scaledWidth = image.rectTransform.rect.width * image.transform.localScale.x;
            float scaledHeight = image.rectTransform.rect.height * image.transform.localScale.y;

            return new Vector2(
                (point.x / Width * scaledWidth - scaledWidth * 0.5f) + pos.x,
                (point.y / Height * scaledHeight - scaledHeight * 0.5f) + pos.y);
        }

        private void Create(int width, int height, TextureFormat format)
        {
            if (texture == null || texture.width != width || texture.height != height || texture.format != format)
            {
                texture = new Texture2D(width, height, format, false);

                canvas = FindObjectOfType<Canvas>();
                image = GetComponent<Image>();
                arf = GetComponent<AspectRatioFitter>();
                rectTransform = (RectTransform)transform;
                initialScale = transform.localScale;

                Width = texture.width;
                Height = texture.height;

                sprite = Sprite.Create(texture, new Rect(0, 0, Width, Height), new Vector2(0.5f, 0.5f));

                rectTransform.sizeDelta = new Vector2(Width, Height);

                image.sprite = sprite;
                arf.aspectRatio = Width / (float)Height;
            }

            float flipX = _flipHorizontally ? -initialScale.x : initialScale.x;
            float flipY = _flipVertically ? initialScale.y : -initialScale.y;

            image.transform.localScale = new Vector3(flipX, flipY, initialScale.z);
        }
    }
}
