using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class NuitrackSetup : NetworkBehaviour
{
    [SerializeField] public Transform[] bones;

    void Start()
    {
        bones = new Transform[11];
        GetBones();
    }

    public void GetBones()
    {
        Transform head = GetTheChild("Head");

        Camera.main.transform.parent = head;
        Camera.main.transform.localPosition = new Vector3(0, 0, .2f);
        Camera.main.transform.localRotation = Quaternion.identity;

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

        bones[3].localRotation = Quaternion.Euler(new Vector3(0, bones[3].rotation.y, bones[3].rotation.z));
        bones[4].localRotation = Quaternion.Euler(new Vector3(0, bones[4].rotation.y, bones[4].rotation.z));

        this.gameObject.AddComponent<RiggedAvatar>();

        foreach (Transform bone in bones)
        {
            bone.gameObject.AddComponent<ClientNetworkTransform>();
            bone.GetComponent<ClientNetworkTransform>().SyncPositionX = false;
            bone.GetComponent<ClientNetworkTransform>().SyncPositionY = false;
            bone.GetComponent<ClientNetworkTransform>().SyncPositionZ = false;

            bone.GetComponent<ClientNetworkTransform>().SyncScaleX = false;
            bone.GetComponent<ClientNetworkTransform>().SyncScaleY = false;
            bone.GetComponent<ClientNetworkTransform>().SyncScaleZ = false;

            bone.GetComponent<ClientNetworkTransform>().SyncRotAngleX = true;
            bone.GetComponent<ClientNetworkTransform>().SyncRotAngleY = true;
            bone.GetComponent<ClientNetworkTransform>().SyncRotAngleZ = true;
        }
    }

    private Transform GetTheChild(string _name)
    {
        return transform.GetComponentsInChildren<Transform>()
                             .FirstOrDefault(c => c.gameObject.name.Equals(_name));
    }
}
