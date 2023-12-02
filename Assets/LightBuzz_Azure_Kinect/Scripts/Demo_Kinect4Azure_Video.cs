using LightBuzz.Kinect4Azure.Video;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LightBuzz.Kinect4Azure
{
    public class Demo_Kinect4Azure_Video : MonoBehaviour
    {
        [Header("Sensor Configuration")]
        [SerializeField] private Configuration _configuration;
        [SerializeField] private UniformImage _image;
        [SerializeField] private StickmanManager _stickmanManager;

        [Header("Video Configuration")]
        [SerializeField] private bool _recordColor = true;
        [SerializeField] private bool _recordDepth = false;
        [SerializeField] private bool _recordUser = false;
        [SerializeField] private bool _recordBody = true;
        [SerializeField] private bool _recordFloor = false;
        [SerializeField] private bool _recordIMU = false;

        [Header("User Interface")]
        [SerializeField] private Image startStopBtnImg;
        [SerializeField] private Sprite[] startStopSprites;

        [SerializeField] private MediaBarPlayer mediaBarPlayer;
        [SerializeField] private GameObject recordingPanel;
        [SerializeField] private GameObject backBtnGO;

        [SerializeField] private GameObject savingPanelGO;

        private KinectSensor _sensor;
        private VideoRecorder _recorder;

        private bool _isRecording = false;
        private bool _stopPlayback = false;
        private bool _showSavingPanel = false;

        private void Start()
        {
            _sensor = KinectSensor.Create(_configuration);

            if (_sensor == null)
            {
                Debug.LogError("Sensor not connected!");
                return;
            }

            _sensor.Open();

            _recorder = new VideoRecorder(new VideoConfiguration
            {
                Path = Path.Combine(Application.persistentDataPath, "Video"),
                ColorResolution = _sensor.Configuration.ColorResolution.Size(),
                DepthResolution = _sensor.Configuration.DepthMode.Size(),
                RecordColor = _recordColor,
                RecordDepth = _recordDepth,
                RecordUser = _recordUser,
                RecordBody = _recordBody,
                RecordFloor = _recordFloor,
                RecordIMU = _recordIMU
            });

            _recorder.OnRecordingStarted += OnRecordingStarted;
            _recorder.OnRecordingStopped += OnRecordingStopped;
            _recorder.OnRecordingCompleted += OnRecordingCompleted;

            Debug.Log("Video will be saved at " + _recorder.Configuration.Path);
        }

        private void OnRecordingCompleted()
        {
            Debug.Log("Recording completed");

            _showSavingPanel = false;
        }

        private void OnRecordingStopped()
        {
            Debug.Log("Recording stopped");

            _showSavingPanel = true;
        }

        private void OnRecordingStarted()
        {
            Debug.Log("Recording started");

            _stopPlayback = true;
        }

        private void Update()
        {
            if (_stopPlayback)
            {
                mediaBarPlayer.Stop();

                _stopPlayback = false;
            }

            if (savingPanelGO.activeSelf != _showSavingPanel)
            {
                savingPanelGO.SetActive(_showSavingPanel);

                // Playback
                if (!_showSavingPanel)
                {
                    mediaBarPlayer.LoadVideo(_recorder.Configuration.Path);
                    mediaBarPlayer.Play();
                    backBtnGO.SetActive(true);
                    recordingPanel.SetActive(false);
                }
            }

            UpdateFrame();
        }

        private void OnDestroy()
        {
            _sensor?.Close();

            _recorder.OnRecordingStarted -= OnRecordingStarted;
            _recorder.OnRecordingStopped -= OnRecordingStopped;
            _recorder.OnRecordingCompleted -= OnRecordingCompleted;
            _recorder.Dispose();

            mediaBarPlayer.Dispose();
        }

        private void UpdateFrame()
        {
            Frame frame =
                mediaBarPlayer.IsPlaying ? mediaBarPlayer.Update() :
                _sensor != null && _sensor.IsOpen ? _sensor.Update() :
                null;

            if (frame != null)
            {
                if (frame.ColorFrameSource != null)
                {
                    _image.Load(frame.ColorFrameSource);
                }

                if (frame.BodyFrameSource != null)
                {
                    _stickmanManager.Load(frame.BodyFrameSource.Bodies);
                }
            }

            _recorder?.Update(frame);
        }

        public void BackToRecording()
        {
            mediaBarPlayer.Stop();
            recordingPanel.SetActive(true);
            backBtnGO.SetActive(false);
        }

        public void StartStopRecording()
        {
            _isRecording = !_isRecording;
            startStopBtnImg.sprite = startStopSprites[_isRecording ? 1 : 0];

            if (_isRecording)
                _recorder.Start();
            else
                _recorder.Stop();
        }
    }
}
