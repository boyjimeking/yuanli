//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: TrapRefillReq.proto
namespace com.pureland.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"TrapRefillReq")]
  public partial class TrapRefillReq : global::ProtoBuf.IExtensible
  {
    public TrapRefillReq() {}
    
    private TrapRefillReq.RefillType _refillType;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"refillType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public TrapRefillReq.RefillType refillType
    {
      get { return _refillType; }
      set { _refillType = value; }
    }
    private long _sid = default(long);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"sid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long sid
    {
      get { return _sid; }
      set { _sid = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"RefillType")]
    public enum RefillType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"Single", Value=1)]
      Single = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"All", Value=2)]
      All = 2
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}