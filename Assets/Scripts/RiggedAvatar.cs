using System;
using System.Collections.Generic;
using UnityEngine;
using NuitrackSDK;
using NuitrackSDK.Calibration;
using Unity.Netcode;

public class RiggedAvatar : NetworkBehaviour
{
    [Header("Rigged model")]
    [SerializeField]
    public List<ModelJoint> modelJoints;
    [SerializeField]
    nuitrack.JointType rootJoint = nuitrack.JointType.LeftCollar;
    /// <summary> Model bones </summary>
    public Dictionary<nuitrack.JointType, ModelJoint> jointsRigged = new Dictionary<nuitrack.JointType, ModelJoint>();
    NuitrackSetup setup;

    public bool ready = false;

    void Start()
    {
        // setup = GetComponent<NuitrackSetup>();

        // modelJoints = new List<ModelJoint>();

        // for (int i = 0; i < setup.bones.Length; i++)
        // {
        // ModelJoint mj = new ModelJoint();

        // mj.bone = setup.bones[i];
        // modelJoints.Add(mj);
        // }

        // modelJoints[0].jointType = nuitrack.JointType.Torso;
        // modelJoints[1].jointType = nuitrack.JointType.LeftCollar;
        // modelJoints[2].jointType = nuitrack.JointType.RightCollar;
        // modelJoints[3].jointType = nuitrack.JointType.LeftShoulder;
        // modelJoints[4].jointType = nuitrack.JointType.RightShoulder;
        // modelJoints[5].jointType = nuitrack.JointType.LeftElbow;
        // modelJoints[6].jointType = nuitrack.JointType.RightElbow;
        // modelJoints[7].jointType = nuitrack.JointType.LeftHip;
        // modelJoints[8].jointType = nuitrack.JointType.RightHip;
        // modelJoints[9].jointType = nuitrack.JointType.LeftKnee;
        // modelJoints[10].jointType = nuitrack.JointType.RightKnee;

        // foreach (ModelJoint mj in modelJoints)
        // {
        // mj.baseRotOffset = mj.bone.rotation;
        // jointsRigged.Add(mj.jointType.TryGetMirrored(), mj);
        // }
    }

    void Update()
    {
        if (NuitrackManager.Users.Current != null && NuitrackManager.Users.Current.Skeleton != null && ready)
            ProcessSkeleton(NuitrackManager.Users.Current.Skeleton);
    }

    void ProcessSkeleton(UserData.SkeletonData skeleton)
    {
        //Calculate the model position: take the root position and invert movement along the Z axis
        //Vector3 rootPos = Quaternion.Euler(0f, 180f, 0f) * skeleton.GetJoint(rootJoint).Position;
        //transform.position = rootPos;

        foreach (var riggedJoint in jointsRigged)
        {
            //Get joint from the Nuitrack
            UserData.SkeletonData.Joint joint = skeleton.GetJoint(riggedJoint.Key);

            ModelJoint modelJoint = riggedJoint.Value;

            //Calculate the model bone rotation: take the mirrored joint orientation, add a basic rotation of the model bone, invert movement along the Z axis
            Quaternion jointOrient = Quaternion.Inverse(CalibrationInfo.SensorOrientation) * joint.RotationMirrored * modelJoint.baseRotOffset;

            modelJoint.bone.rotation = jointOrient;
        }
    }
}