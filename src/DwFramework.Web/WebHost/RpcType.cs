using DwFramework.Extensions;
using ProtoBuf;

namespace DwFramework.Web;

public record class RpcType
{
    [ProtoContract]
    public record class Object<T>
    {
        [ProtoMember(1)]
        public Type Type { get; }
        [ProtoMember(2)]
        public byte[] Bytes { get; }
        public object Value => Bytes.FromJsonBytes(Type);

        public Object(T value)
        {
            Type = typeof(T);
            Bytes = value.ToJsonBytes();
        }
    }

    [ProtoContract]
    public record class Bool
    {
        [ProtoMember(1)]
        public bool Value { get; }

        public Bool(bool value)
        {
            Value = value;
        }
    }

    [ProtoContract]
    public record class String
    {
        [ProtoMember(1)]
        public string Value { get; }

        public String(string value)
        {
            Value = value;
        }
    }

    [ProtoContract]
    public record class Long
    {
        [ProtoMember(1)]
        public long Value { get; }

        public Long(long value)
        {
            Value = value;
        }
    }

    [ProtoContract]
    public record class Int
    {
        [ProtoMember(1)]
        public int Value { get; }

        public Int(int value)
        {
            Value = value;
        }
    }

    [ProtoContract]
    public record class Float
    {
        [ProtoMember(1)]
        public float Value { get; }

        public Float(float value)
        {
            Value = value;
        }
    }
}