//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: ResourceVO.proto
// Note: requires additional types generated from: ResourceType.proto
namespace com.pureland.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ResourceVO")]
  public partial class ResourceVO : global::ProtoBuf.IExtensible
  {
    public ResourceVO() {}
    
    private ResourceType _resourceType;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"resourceType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public ResourceType resourceType
    {
      get { return _resourceType; }
      set { _resourceType = value; }
    }
    private int _resourceCount;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"resourceCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int resourceCount
    {
      get { return _resourceCount; }
      set { _resourceCount = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}