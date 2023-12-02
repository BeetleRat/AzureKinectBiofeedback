using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;


namespace LightBuzz.Kinect4Azure
{
    public class Demo_Kinect4Azure_BackgroundRemoval : MonoBehaviour
    {
        [SerializeField] private Configuration _configuration;
        [SerializeField] private UniformImage _image;
        [SerializeField] private StickmanManager _stickmanManager;
        [SerializeField] private Color _background = Color.green;
        
        private KinectSensor _sensor;
        private Color32[] _colors;

        private void Start()
        {
            _sensor = KinectSensor.Create(_configuration);

            if (_sensor == null)
            {
                Debug.LogError("Sensor not connected!");
                return;
            }

            _sensor.Open();
        }

        private void Update()
        {
            if (_sensor == null || !_sensor.IsOpen) return;

            Frame frame = _sensor.Update();

            if (frame != null)
            {
                if (frame.ColorFrameSource != null && 
                    frame.UserFrameSource != null &&
                    frame.UserFrameSource.Data != null)
                {
                    byte[] colorData = _sensor.CoordinateMapper.ColorToDepth();
                    byte[] userData = frame.UserFrameSource.Data;

                    int width = frame.UserFrameSource.Width;
                    int height = frame.UserFrameSource.Height;
                    int size = width * height;

                    byte[] colors = new byte[colorData.Length];

                    for (int i = 0, j = 0; i < size; i++, j += 4)
                    {
                        if (userData[i] != UserFrameSource.Background)
                        {
                            colors[j + 0] = colorData[j + 0];
                            colors[j + 1] = colorData[j + 1];
                            colors[j + 2] = colorData[j + 2];
                            colors[j + 3] = colorData[j + 3];
                        }
                        else
                        {
                            colors[j + 0] = (byte)(_background.b * 255.0f);
                            colors[j + 1] = (byte)(_background.g * 255.0f);
                            colors[j + 2] = (byte)(_background.r * 255.0f);
                            colors[j + 3] = (byte)(_background.a * 255.0f);
                        }
                    }

                    _image.Load(colors, width, height, TextureFormat.BGRA32);
                }

                if (frame.BodyFrameSource != null)
                {
                    _stickmanManager.Load(frame.BodyFrameSource.Bodies);
                }
            }
        }

        private void OnDestroy()
        {
            _sensor?.Close();
        }
    }
}
