using Unity.Netcode;
using Unity.Collections;

public struct NetworkString : INetworkSerializable
{
    private ForceNetworkSerializeByMemcpy<FixedString128Bytes> info;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref info);
    }

    public override string ToString()
    {
        return info.Value.ToString();
    }

    public static implicit operator string(NetworkString s) => s.ToString();
    public static implicit operator NetworkString(string s) => new NetworkString() { info = new FixedString128Bytes(s) };
}