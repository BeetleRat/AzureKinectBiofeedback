using UnityEngine;


namespace LightBuzz.Kinect4Azure
{
    public class Demo_Kinect4Azure_Alignment : MonoBehaviour
    {
        [SerializeField] private Configuration _configuration;
        [SerializeField] private UniformImage _colorOriginal;
        [SerializeField] private UniformImage _depthOriginal;
        [SerializeField] private UniformImage _colorAligned;
        [SerializeField] private UniformImage _depthAligned;

        [SerializeField] private StickmanManager _stickmanColorOriginal;
        [SerializeField] private StickmanManager _stickmanDepthOriginal;
        [SerializeField] private StickmanManager _stickmanColorAligned;
        [SerializeField] private StickmanManager _stickmanDepthAligned;
        [SerializeField] [Range(1000, 10000)] private ushort _maxDepth = 8000;
        [SerializeField] private DepthVisualization _visualization = DepthVisualization.Jet;

        private KinectSensor _sensor;

        private void Start()
        {
            _sensor = KinectSensor.Create(_configuration);

            if (_sensor == null)
            {
                Debug.LogError("Sensor not connected!");
                return;
            }

            if (_sensor.Configuration.ColorFormat != ColorFormat.BGRA32)
            {
                Debug.LogError("You need to set the ColorFormat to BGRA32.");
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
                if (frame.ColorFrameSource != null && frame.DepthFrameSource != null)
                {
                    byte[] colorDataOriginal = frame.ColorFrameSource.Data;
                    byte[] colorDataAligned = _sensor.CoordinateMapper.ColorToDepth();

                    ushort[] depthDataOriginal = frame.DepthFrameSource.Data;
                    ushort[] depthDataAligned = _sensor.CoordinateMapper.DepthToColor();

                    _colorOriginal.Load(colorDataOriginal, frame.ColorFrameSource.Width, frame.ColorFrameSource.Height, TextureFormat.BGRA32);
                    _colorAligned.Load(colorDataAligned, frame.DepthFrameSource.Width, frame.DepthFrameSource.Height, TextureFormat.BGRA32);

                    _depthOriginal.Load(depthDataOriginal, frame.DepthFrameSource.Width, frame.DepthFrameSource.Height, _maxDepth, _visualization);
                    _depthAligned.Load(depthDataAligned, frame.ColorFrameSource.Width, frame.ColorFrameSource.Height, _maxDepth, _visualization);
                }

                if (frame.BodyFrameSource != null)
                {
                    _stickmanColorOriginal.Load(frame.BodyFrameSource.Bodies);
                    _stickmanDepthOriginal.Load(frame.BodyFrameSource.Bodies);
                    _stickmanColorAligned.Load(frame.BodyFrameSource.Bodies);
                    _stickmanDepthAligned.Load(frame.BodyFrameSource.Bodies);
                }
            }
        }

        private void OnDestroy()
        {
            _sensor?.Close();
        }
    }
}
