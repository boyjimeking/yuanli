//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: PlayerListResp.proto
// Note: requires additional types generated from: PlayerLoginSimpleVO.proto
namespace com.pureland.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PlayerListResp")]
  public partial class PlayerListResp : global::ProtoBuf.IExtensible
  {
    public PlayerListResp() {}
    
    private readonly global::System.Collections.Generic.List<PlayerLoginSimpleVO> _loginSimpleVOs = new global::System.Collections.Generic.List<PlayerLoginSimpleVO>();
    [global::ProtoBuf.ProtoMember(1, Name=@"loginSimpleVOs", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<PlayerLoginSimpleVO> loginSimpleVOs
    {
      get { return _loginSimpleVOs; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}