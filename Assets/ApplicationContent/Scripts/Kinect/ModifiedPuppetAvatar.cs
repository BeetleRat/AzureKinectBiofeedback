using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

[Serializable]
struct BoolVector
{
    public bool X;
    public bool Y;
    public bool Z;
}

[Serializable]
struct FloatVector
{
    public float X;
    public float Y;
    public float Z;
}

/// <summary>
/// <para>Modifications to the avatar position, compared to the tracked body</para>
/// </summary>
[Serializable]
struct PositionModification
{
    public BoolVector FreezePosition;
    public FloatVector PositionOffset;
    [Header("Debug")] public FloatVector PositionScale;
}

/// <summary>
/// <para>Script used to control an avatar using the Azure Kinect.</para>
/// <para>This script requires the main camera in the scene. But it must not be a child object of the object using this script</para>
/// </summary>
public class ModifiedPuppetAvatar : MonoBehaviour
{
    [Tooltip("TrackerHandler TrackerHandler located in the Kinect4AzureTrackerAllInOne prefab")] 
    [SerializeField] private TrackerHandler _kinectDevice;

    [Tooltip("pointBody object which is a child of Kinect4AzureTrackerAllInOne prefab")] 
    [SerializeField] private Transform _rootPosition;

    [Tooltip("The root transform object of the avatar armature")] 
    [SerializeField] private Transform _characterRootTransform;

    [SerializeField] private PositionModification _positionModification;

    private Dictionary<JointId, Quaternion> absoluteOffsetMap;
    private Animator puppetAnimator;
    private Quaternion modelRotation;
    private new GameObject camera;

    private static HumanBodyBones MapKinectJoint(JointId joint)
    {
        // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
        switch (joint)
        {
            case JointId.Pelvis: return HumanBodyBones.Hips;
            case JointId.SpineNavel: return HumanBodyBones.Spine;
            case JointId.SpineChest: return HumanBodyBones.Chest;
            case JointId.Neck: return HumanBodyBones.Neck;
            case JointId.Head: return HumanBodyBones.Head;
            case JointId.HipLeft: return HumanBodyBones.LeftUpperLeg;
            case JointId.KneeLeft: return HumanBodyBones.LeftLowerLeg;
            case JointId.AnkleLeft: return HumanBodyBones.LeftFoot;
            case JointId.FootLeft: return HumanBodyBones.LeftToes;
            case JointId.HipRight: return HumanBodyBones.RightUpperLeg;
            case JointId.KneeRight: return HumanBodyBones.RightLowerLeg;
            case JointId.AnkleRight: return HumanBodyBones.RightFoot;
            case JointId.FootRight: return HumanBodyBones.RightToes;
            case JointId.ClavicleLeft: return HumanBodyBones.LeftShoulder;
            case JointId.ShoulderLeft: return HumanBodyBones.LeftUpperArm;
            case JointId.ElbowLeft: return HumanBodyBones.LeftLowerArm;
            case JointId.WristLeft: return HumanBodyBones.LeftHand;
            case JointId.ClavicleRight: return HumanBodyBones.RightShoulder;
            case JointId.ShoulderRight: return HumanBodyBones.RightUpperArm;
            case JointId.ElbowRight: return HumanBodyBones.RightLowerArm;
            case JointId.WristRight: return HumanBodyBones.RightHand;
            default: return HumanBodyBones.LastBone;
        }
    }

    private void Start()
    {
        modelRotation = transform.rotation;
        camera = GetMainCamera();

        puppetAnimator = GetComponent<Animator>();
        Transform _rootJointTransform = _characterRootTransform;

        absoluteOffsetMap = new Dictionary<JointId, Quaternion>();
        for (int i = 0; i < (int)JointId.Count; i++)
        {
            HumanBodyBones hbb = MapKinectJoint((JointId)i);
            if (hbb != HumanBodyBones.LastBone)
            {
                Transform transform = puppetAnimator.GetBoneTransform(hbb);
                Quaternion absOffset = GetSkeletonBone(puppetAnimator, transform.name).rotation;
                // find the absolute offset for the tpose
                while (!ReferenceEquals(transform, _rootJointTransform))
                {
                    transform = transform.parent;
                    absOffset = GetSkeletonBone(puppetAnimator, transform.name).rotation * absOffset;
                }

                absoluteOffsetMap[(JointId)i] = absOffset;
            }
        }
    }

    private GameObject GetMainCamera()
    {
        GameObject foundMainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        Camera[] childCameras = GetComponentsInChildren<Camera>(true);
        foreach (Camera childCamera in childCameras)
        {
            if (childCamera.gameObject == foundMainCamera)
            {
                return null;
            }
        }

        return foundMainCamera;
    }

    private static SkeletonBone GetSkeletonBone(Animator animator, string boneName)
    {
        int count = 0;
        StringBuilder cloneName = new StringBuilder(boneName);
        cloneName.Append("(Clone)");
        foreach (SkeletonBone sb in animator.avatar.humanDescription.skeleton)
        {
            if (sb.name == boneName || sb.name == cloneName.ToString())
            {
                return animator.avatar.humanDescription.skeleton[count];
            }

            count++;
        }

        return new SkeletonBone();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        for (int j = 0; j < (int)JointId.Count; j++)
        {
            if (MapKinectJoint((JointId)j) != HumanBodyBones.LastBone && absoluteOffsetMap.ContainsKey((JointId)j))
            {
                // get the absolute offset
                Transform finalJoint = puppetAnimator.GetBoneTransform(MapKinectJoint((JointId)j));
                finalJoint.rotation = GetRotationFromDevice(j);
                if (j == 0)
                {
                    finalJoint.position = GetPositionFromDevice(j);
                }
            }
        }

        RotateFullObject();
    }

    private Quaternion GetRotationFromDevice(int j)
    {
        Quaternion absOffset = absoluteOffsetMap[(JointId)j];

        return absOffset * Quaternion.Inverse(absOffset) *
               _kinectDevice.absoluteJointRotations[j] * absOffset;
    }

    private Vector3 GetPositionFromDevice(int j)
    {
        Vector3 positionOffset = _kinectDevice.absoluteJointPosition[j];
        positionOffset = ApplyConstraints(positionOffset);
        positionOffset = ApplyOffsets(positionOffset);
        positionOffset = AddOffsetToRootPosition(positionOffset);
        positionOffset = ApplyScaleToPosition(positionOffset);

        return _characterRootTransform.position + positionOffset;
    }

    private Vector3 ApplyConstraints(Vector3 position)
    {
        return new Vector3(
            _positionModification.FreezePosition.X ? 0 : position.x,
            _positionModification.FreezePosition.Y ? 0 : position.y,
            _positionModification.FreezePosition.Z ? 0 : position.z);
    }

    private Vector3 ApplyOffsets(Vector3 position)
    {
        return new Vector3(
            position.x + _positionModification.PositionOffset.X,
            position.y + _positionModification.PositionOffset.Y,
            position.z + _positionModification.PositionOffset.Z);
    }

    private Vector3 AddOffsetToRootPosition(Vector3 positionOffset)
    {
        Vector3 rootLocalPosition = _rootPosition.localPosition;
        return new Vector3(
            rootLocalPosition.x + positionOffset.x,
            rootLocalPosition.y + positionOffset.y,
            rootLocalPosition.z + positionOffset.z);
    }

    private Vector3 ApplyScaleToPosition(Vector3 position)
    {
        return new Vector3(
            position.x * _positionModification.PositionScale.X,
            position.y * _positionModification.PositionScale.Y,
            position.z * _positionModification.PositionScale.Z);
    }

    private void RotateFullObject()
    {
        if (camera)
        {
            transform.rotation *= modelRotation;
            transform.rotation *= camera.transform.rotation;
        }
    }
}