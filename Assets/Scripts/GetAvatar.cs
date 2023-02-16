using UnityEngine;
using Unity.Netcode;
using NuitrackSDK;
using ReadyPlayerMe;
using System.Collections.Generic;
using System.Linq;

public class GetAvatar : NetworkBehaviour
{
    [SerializeField] NuitrackSetup setup;
    private string avatarUrl;

    [SerializeField] GameObject avatar;
    private readonly NetworkVariable<NetworkString> avatarUrl_nw = new();
    //private readonly NetworkVariable<NetworkString> playerName_nw = new();
    private bool avatarLoaded;
    [SerializeField] private GameObject parentRef;

    public Transform[] bones;

    RiggedAvatar rig;

    void Start()
    {
        avatarLoaded = false;
    }

    private void FixedUpdate()
    {
        if (avatarLoaded) { return; }
        else
        {
            if (avatarUrl == null) { return; }

            if (!GetAvatarUrl().Equals(avatarUrl))
            {
                avatarUrl = GetAvatarUrl();
            }

            StartLoadAvatar(GetAvatarUrl());
            avatarLoaded = true;
        }
    }

    public override void OnDestroy()
    {
        if (avatar != null) Destroy(avatar);
        base.OnDestroy();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SetAvatarUrl(avatarUrl);
        }
        base.OnNetworkSpawn();
    }

    public void SetAvatarUrl(string _avatarUrl)
    {
        avatarUrl = _avatarUrl;
        avatarUrl_nw.Value = _avatarUrl;
    }
    private string GetAvatarUrl()
    {
        return avatarUrl_nw.Value;
    }

    private void StartLoadAvatar(string loadAvatarUrl)
    {
        ApplicationData.Log();

        var avatarLoader = new AvatarLoader();
        avatarLoader.OnCompleted += (_, args) =>
        {
            avatar = args.Avatar;

            if (parentRef.name == loadAvatarUrl)
            {
                avatar.transform.parent = transform;
                avatar.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }

            parentRef.name = "ParentRef";
            Debug.Log("DFW2DDGWFHGWFEGDFGWDFG");
            ImportBones();
        };
        avatarLoader.LoadAvatar(loadAvatarUrl);

        parentRef.name = loadAvatarUrl;
    }

    private void ImportBones()
    {
        bones[0] = GetTheChild("Spine2");
        bones[1] = GetTheChild("LeftShoulder");
        bones[2] = GetTheChild("RightShoulder");
        bones[3] = GetTheChild("LeftArm");
        bones[4] = GetTheChild("RightArm");
        bones[5] = GetTheChild("LeftForeArm");
        bones[6] = GetTheChild("RightForeArm");
        bones[7] = GetTheChild("LeftUpLeg");
        bones[8] = GetTheChild("RightUpLeg");
        bones[9] = GetTheChild("LeftLeg");
        bones[10] = GetTheChild("RightLeg");

        SetJoints();
    }

    private void SetJoints()
    {
        rig = gameObject.AddComponent<RiggedAvatar>();

        rig.modelJoints = new List<ModelJoint>();

        for (int i = 0; i < bones.Length; i++)
        {
            ModelJoint mj = new ModelJoint();
            mj.bone = bones[i];

            rig.modelJoints.Add(mj);
        }

        rig.modelJoints[0].jointType = nuitrack.JointType.Torso;
        rig.modelJoints[1].jointType = nuitrack.JointType.LeftCollar;
        rig.modelJoints[2].jointType = nuitrack.JointType.RightCollar;
        rig.modelJoints[3].jointType = nuitrack.JointType.LeftShoulder;
        rig.modelJoints[4].jointType = nuitrack.JointType.RightShoulder;
        rig.modelJoints[5].jointType = nuitrack.JointType.LeftElbow;
        rig.modelJoints[6].jointType = nuitrack.JointType.RightElbow;
        rig.modelJoints[7].jointType = nuitrack.JointType.LeftHip;
        rig.modelJoints[8].jointType = nuitrack.JointType.RightHip;
        rig.modelJoints[9].jointType = nuitrack.JointType.LeftKnee;
        rig.modelJoints[10].jointType = nuitrack.JointType.RightKnee;

        for (int i = 0; i < rig.modelJoints.Count; i++)
        {
            rig.modelJoints[i].baseRotOffset = rig.modelJoints[i].bone.rotation;
            rig.jointsRigged.Add(rig.modelJoints[i].jointType.TryGetMirrored(), rig.modelJoints[i]);
        }

        rig.ready = true;
    }

    private Transform GetTheChild(string n)
    {
        return gameObject.transform.GetComponentsInChildren<Transform>()
            .FirstOrDefault(c => c.gameObject.name == n)?.transform;
    }
}