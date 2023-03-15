using module.counter.model;
using module.fabao.packet.impl;
namespace GameEvent
{
	
	public struct UIShowEvent
	{
		public string name;
	}
	public struct UIHideEvent
	{
		public string name;
	}
	public struct UIDestroyEvent
	{
		public string name;
	}
	public struct ResourceChangedEvent
	{
		public ResourceType resType;
		public long resValue;
	}
	public struct ResourceAllChangedEvent
	{
	}
	public struct LevelUpEvent
	{
		public bool success;
	}
	public struct ExpAutoAddEvent
	{
		public bool isMax;
	}
	public struct ItemChangeEvent {
		public PackType type;
	}
    public struct ItemOneChangeEvent
    {
        public PackType type;
        public int index;
    }
    public struct CounterUpdateEvent { }
    public struct EmailUpdateEvent {}
	//----------------------法宝-----------------------
	public struct FaBaoChangeEvent { }		//法宝内容变化
	public struct FaBaoCountChangeEvent { }//法宝个数发生了变化
	public struct FaBaoWearSucEvent { }
	public struct FaBaoFuLingSelectEvent {
		public FaBaoInfo info;
	}

	//----------------------域外-----------------------
	public struct YWPosInfoChangeEvent
	{
		public YWPosInfo info;
	}
    public struct YWMapChangeEvent { }
	public struct YWPosInfoPathChangeEvent { }
	public struct YWLingLiChangeEvent { }
	public struct YWPacketChangeEvent { }
	public struct YWNoticeInfoChangeEvent { }
	public struct YWMiJingChangeEvent {
		public int x;
		public int y;
	}
    //-------------------章节--------------------------
    public struct ChapterLevelChangeEvent { }

    //-------------------pet---------------------------
    public struct PetUpdateEvent { public long uid; };
    public struct PetDeleteEvent { public long uid; };
    public struct PetFormationSucEvent {};
    public struct PetEvolutionSucEvent { public long uid; };
    //-----------------task----------------------------
    public struct TaskUpdateEvent { };
    public struct TaskOneUpdateEvent { public int taskId; };

    public struct EquipChangeEvent
    {
    }

}

