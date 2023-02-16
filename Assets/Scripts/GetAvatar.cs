using UnityEngine;
using Unity.Netcode;
using ReadyPlayerMe;

public class GetAvatar : NetworkBehaviour
{
    [SerializeField] NuitrackSetup setup;
    private string avatarUrl;

    [SerializeField] GameObject avatar;
    private readonly NetworkVariable<NetworkString> avatarUrl_nw = new();
    //private readonly NetworkVariable<NetworkString> playerName_nw = new();
    private bool avatarLoaded;
    [SerializeField] private GameObject parentRef;

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
            setup = gameObject.AddComponent<NuitrackSetup>();
        };
        avatarLoader.LoadAvatar(loadAvatarUrl);

        parentRef.name = loadAvatarUrl;
    }

    /*private void StartLoadAvatar(string loadAvatarUrl)
    {
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
            setup = gameObject.AddComponent<NuitrackSetup>();
        };

        avatarLoader.LoadAvatar(loadAvatarUrl);

        parentRef.name = loadAvatarUrl;
*/
}