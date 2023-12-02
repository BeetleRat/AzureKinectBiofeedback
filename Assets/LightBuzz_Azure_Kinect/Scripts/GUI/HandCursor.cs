using UnityEngine;
using UnityEngine.UI;

namespace LightBuzz.Kinect4Azure
{
    public enum CursorHand
    {
        Automatic,
        Left,
        Right
    }

    public class HandCursor : MonoBehaviour
    {
        [SerializeField] private CursorHand _hand = CursorHand.Automatic;
        [SerializeField][Range(0, 1)] private float _smoothing = 0.25f;
        [SerializeField][Range(0, 10)] private float _scaleX = 1.0f;
        [SerializeField][Range(0, 10)] private float _scaleY = 1.0f;

        private Image _cursorImage;

        private Vector2 _previous = Vector2.zero;

        public CursorHand Hand
        {
            get => _hand;
            set => _hand = value;
        }

        private void Start()
        {
            _cursorImage = GetComponent<Image>();
        }

        public void Load(Body body, UniformImage image)
        {
            if (body == null) return;
            if (image == null) return;

            int width = image.Width;
            int height = image.Height;

            Joint elbowLeft = body.Joints[JointType.ElbowLeft];
            Joint elbowRight = body.Joints[JointType.ElbowRight];
            Joint wristLeft = body.Joints[JointType.WristLeft];
            Joint wristRight = body.Joints[JointType.WristRight];

            Vector2D shoulderLeft = body.Joints[JointType.ShoulderLeft].PositionColor;
            Vector2D shoulderRight = body.Joints[JointType.ShoulderRight].PositionColor;

            Vector2D shoulder = Vector2D.Zero;
            Vector2D elbow = Vector2D.Zero;
            Vector2D wrist = Vector2D.Zero;

            switch (_hand)
            {
                case CursorHand.Left:
                    {
                        shoulder = shoulderLeft;
                        elbow = elbowLeft.PositionColor;
                        wrist = wristLeft.PositionColor;
                    }
                    break;
                case CursorHand.Right:
                    {
                        shoulder = shoulderRight;
                        elbow = elbowRight.PositionColor;
                        wrist = wristRight.PositionColor;
                    }
                    break;
                case CursorHand.Automatic:
                    {
                        shoulder = wristLeft.PositionColor.Y < wristRight.PositionColor.Y ? shoulderLeft : shoulderRight;
                        elbow = wristLeft.PositionColor.Y < wristRight.PositionColor.Y ? elbowLeft.PositionColor : elbowRight.PositionColor;
                        wrist = wristLeft.PositionColor.Y < wristRight.PositionColor.Y ? wristLeft.PositionColor : wristRight.PositionColor;
                    }
                    break;
                default:
                    break;
            }

            float distance = Calculations.Distance(shoulderLeft, shoulderRight);

            float minX = shoulder.X - distance;
            if (minX < 0.0f) minX = 0.0f;

            float maxX = shoulder.X + distance;
            if (maxX > width) maxX = width;

            float minY = shoulder.Y - distance;
            if (minY < 0.0f) minY = 0.0f;

            float maxY = shoulder.Y + distance;
            if (maxY > height) maxY = height;

            float factorX = minX != maxX ? (wrist.X - minX) / (maxX - minX) : 0.0f;
            float factorY = minY != maxY ? (wrist.Y - minY) / (maxY - minY) : 0.0f;

            float x = width * factorX * _scaleX;
            float y = height * factorY * _scaleY;

            float angle = 90.0f - Calculations.Rotation(elbow, wrist, Axis.Z);
            if (wrist.X > elbow.X) angle = -angle;

            Vector3 position = Vector2.Lerp(_previous, new Vector2(x, y), _smoothing);
            Vector3 rotation = new Vector3(0.0f, 0.0f, angle);
            Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

            _previous = position;

            _cursorImage.transform.localPosition = image.GetPosition(position);
            _cursorImage.transform.localRotation = UnityEngine.Quaternion.Euler(rotation);
            _cursorImage.transform.localScale = scale;
        }
    }
}