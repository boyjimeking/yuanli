//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: BattleOperationPlaceSoldierVO.proto
namespace com.pureland.proto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BattleOperationPlaceSoldierVO")]
  public partial class BattleOperationPlaceSoldierVO : global::ProtoBuf.IExtensible
  {
    public BattleOperationPlaceSoldierVO() {}
    
    private int _cid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"cid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int cid
    {
      get { return _cid; }
      set { _cid = value; }
    }
    private int _x;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int x
    {
      get { return _x; }
      set { _x = value; }
    }
    private int _y;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int y
    {
      get { return _y; }
      set { _y = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}