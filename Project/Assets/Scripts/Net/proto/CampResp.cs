//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: CampResp.proto
// Note: requires additional types generated from: PlayerVO.proto
// Note: requires additional types generated from: CampVO.proto
namespace com.pureland.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"CampResp")]
  public partial class CampResp : global::ProtoBuf.IExtensible
  {
    public CampResp() {}
    
    private CampVO _campVO;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"campVO", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public CampVO campVO
    {
      get { return _campVO; }
      set { _campVO = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}