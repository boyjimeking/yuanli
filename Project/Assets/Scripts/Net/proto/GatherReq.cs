//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: GatherReq.proto
// Note: requires additional types generated from: ResourceVO.proto
namespace com.pureland.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GatherReq")]
  public partial class GatherReq : global::ProtoBuf.IExtensible
  {
    public GatherReq() {}
    
    private long _sid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"sid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public long sid
    {
      get { return _sid; }
      set { _sid = value; }
    }
    private ResourceVO _resourceVO;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"resourceVO", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public ResourceVO resourceVO
    {
      get { return _resourceVO; }
      set { _resourceVO = value; }
    }
    private long _gatherTime;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"gatherTime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public long gatherTime
    {
      get { return _gatherTime; }
      set { _gatherTime = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}