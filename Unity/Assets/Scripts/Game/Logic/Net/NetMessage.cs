using ET;
using ProtoBuf;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
namespace  module
{
	public enum GameObjId
	{
		GameObjId_none=0,
		enemy=4,
		npcPet=4,
	}
}
namespace  module
{
	public enum GameObjType
	{
		GameObjType_NONE=0,
		PLAYER=11,
		PET=12,
		MONSTER=13,
		NPC=15,
	}
}
namespace  module
{
	[ProtoContract]
	public partial class Pos: Object
	{
		[ProtoMember(1)]
		public int x { get; set; }

		[ProtoMember(2)]
		public int y { get; set; }

	}
}
namespace  module.attri
{
//属性类型
	public enum AttrEnum
	{
		Attr_NONE=0,
		ATK=1,
		DEF=2,
		HP_MAX=3,
		SPEED=4,
		CRITICAL=5,
		PHYSICAL_DEF=6,
		MAGIC_DEF=7,
		PHYSICAL_STRIKE=8,
		MAGIC_STRIKE=9,
		REN_XING=10,
		WU_XING=11,
		EXP_ADD=12,
		EXP_RATE=13,
		ATK_PER=14,
		DEF_PER=15,
		HP_MAX_PER=16,
		CRITICAL_DAMAGE_PER_PHYSICAL=17,
		CRITICAL_MAGIC=18,
		BLOOD=19,
		CURE_EFFECT=20,
		CURE_PER=21,
		REN_XING_MAGIC=22,
	}
}
namespace  module.test
{
	public enum GmCmd
	{
		GmCmd_NONE=0,
		getItem=1,
		addResources=2,
		addExp=3,
		addTime=4,
		addDay=5,
		attr=6,
		charge=7,
		addMail=8,
		miJing=9,
		sendMessage=9999,
	}
}
namespace  module.account.model
{
	public enum AntiAddictionType
	{
		AntiAddictionType_none=0,
		Unlimited=1,
		AGE16TO18=2,
		AGE8TO16=3,
		AGELESS8=4,
		NOREALNAME=5,
	}
}
namespace  module.activity.model
{
	public enum ActivityType
	{
		NONE=0,
	}
}
namespace  module.battle.domain
{
//战报信息
	[ProtoContract]
	public partial class BattleRecord: Object
	{
		[ProtoMember(1)]
		public long battleId { get; set; }

// 谁赢了,=1表示是进攻方,=2表示防守方
		[ProtoMember(2)]
		public int winner { get; set; }

// 战报文件地址
		[ProtoMember(3)]
		public string reportPath { get; set; }

// 战斗服的端口
		[ProtoMember(4)]
		public int battlePort { get; set; }

// 进攻方队伍信息
		[ProtoMember(5)]
		public module.battle.domain.TeamRecord atkTeam { get; set; }

// 防守方队伍信息
		[ProtoMember(6)]
		public module.battle.domain.TeamRecord defTeam { get; set; }

	}
}
namespace  module.battle.domain
{
//战斗单位信息
	[ProtoContract]
	public partial class BattlerInfo: Object
	{
// 攻击输出
		[ProtoMember(1)]
		public long attack { get; set; }

// 治疗量
		[ProtoMember(2)]
		public long heal { get; set; }

// 受到伤害量
		[ProtoMember(3)]
		public long beInjured { get; set; }

// 击杀次数
		[ProtoMember(4)]
		public int killTimes { get; set; }

// 站位
		[ProtoMember(5)]
		public int site { get; set; }

// 剩余血量
		[ProtoMember(6)]
		public long hp { get; set; }

// 最大血量
		[ProtoMember(7)]
		public long maxHp { get; set; }

// 战斗单位的id
		[ProtoMember(8)]
		public long battlerId { get; set; }

// 战斗单位的类型
		[ProtoMember(9)]
		public module.GameObjType type { get; set; }

// 战斗单位的模型id
		[ProtoMember(10)]
		public int modelId { get; set; }

// 战斗单位的等级
		[ProtoMember(11)]
		public int level { get; set; }

// 战斗单位的境界
		[ProtoMember(12)]
		public int state { get; set; }

	}
}
namespace  module.battle.domain
{
//战斗记录的角色信息
	[ProtoContract]
	public partial class PlayerRecord: Object
	{
		[ProtoMember(1)]
		public long playerId { get; set; }

		[ProtoMember(2)]
		public string playerName { get; set; }

// 上阵的战斗单位信息
		[ProtoMember(3)]
		public List<module.battle.domain.BattlerInfo> battlerInfos = new List<module.battle.domain.BattlerInfo>();

	}
}
namespace  module.battle.domain
{
//战斗队伍记录信息
	[ProtoContract]
	public partial class TeamRecord: Object
	{
// 角色列表,方便以后做组队推关,主线关卡,这里size就是1
		[ProtoMember(1)]
		public List<module.battle.domain.PlayerRecord> players = new List<module.battle.domain.PlayerRecord>();

	}
}
namespace  module.battle.enums
{
//战斗类型
	public enum BattleType
	{
		BattleType_NODE=0,
//关卡
		CHECK_POINTS=2,
	}
}
namespace  module.chat.model
{
	public enum ChatType
	{
		ChatType_NONE=0,
		CHAT_CURRENT=1,
		CHAT_NEARBY=2,
		ChAT_TEAM=3,
		CHAT_CLAN=4,
		CHAT_WORLD=5,
		CHAT_SYSTEM=6,
		CHAT_LOOP=7,
		FRIEND=8,
		CHAT_RUMOR=11,
		CHAT_HORN=12,
	}
}
namespace  module.checkpoints.message
{
//请求挑战关卡
	[Message(OuterOpcode.CheckPointsBattleReq)]
	[ProtoContract]
	public partial class CheckPointsBattleReq: Object, IRequest
	{
		public int RpcId{ get; set; }
	}
}
namespace  module.checkpoints.message
{
//关卡战斗结果返回
	[Message(OuterOpcode.CheckPointsBattleResp)]
	[ProtoContract]
	public partial class CheckPointsBattleResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 战报
		[ProtoMember(1)]
		public module.battle.domain.BattleRecord record { get; set; }

// 关卡id,如果赢了,角色的关卡就需要更新为这个
		[ProtoMember(2)]
		public int checkPointsId { get; set; }

// 如果是赢了,获得的奖励
		[ProtoMember(3)]
		public List<module.item.message.ItemInfo> rewards = new List<module.item.message.ItemInfo>();

	}
}
namespace  module.checkpoints.message
{
//上线推送关卡信息
	[Message(OuterOpcode.CheckPointsInitResp)]
	[ProtoContract]
	public partial class CheckPointsInitResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public int currentId { get; set; }

	}
}
namespace  module.common.model
{
//
	[ProtoContract]
	public partial class CommItemInfo: Object
	{
// 参考 MailAttachType 枚举
		[ProtoMember(1)]
		public int type { get; set; }

		[ProtoMember(2)]
		public int index { get; set; }

		[ProtoMember(3)]
		public int modelId { get; set; }

		[ProtoMember(4)]
		public int amount { get; set; }

	}
}
namespace  module.common.model
{
	public enum Decisions
	{
		Decisions_none=0,
		accept=4,
		reject=5,
		reject_all=6,
	}
}
namespace  module.common.model
{
	public enum OperEnums
	{
		OperEnums_NONE=0,
		NO=101,
		GM=102,
		CHECKPOINTS_WIN_FIRST=103,
		DELETE_DROPS=104,
		ITEM_USE=105,
		classifyPack=106,
		ITEM_BREAKDOWN=107,
		ITEM_COMPOUND=108,
		MAIL_RECIVE=109,
		FABAO_ZHUZAO=110,
		FABAO_BLESSING=111,
		FABAO_DECOMPOSE=112,
		PET_CATCH=113,
		QUICK_BUY_ITEM=114,
		PACK_CAP_ADD=115,
		YW_TRANS=116,
		YW_TAN_SUO=117,
		YW_TAN_SUO_MI=118,
		YW_DAILY_REWARD=119,
		YW_BUY_LING=120,
		YW_BUT_PACKET=121,
		DRUGS_LIAN_DAN=122,
		ITEM_SELL=123,
		TASK=124,
	}
}
namespace  module.common.model
{
	public enum TimeLimitType
	{
		TimeLimitType_NONE=0,
		time_day=1,
		time_time=2,
		time_week=3,
		time_date=5,
	}
}
namespace  module.counter.message
{
//计数器更新or上线时推送,更新时只发有变化的
	[Message(OuterOpcode.CounterResp)]
	[ProtoContract]
	public partial class CounterResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 计数器信息：key是枚举CounterType　， val　＝　当前的计数
		[ProtoMember(1)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,int> countMap { get; set; }

	}
}
namespace  module.counter.model
{
	public enum CounterType
	{
		CounterType_NONE=0,
//已使用的灵宠免费抓捕次数
		PET_CATCH=1,
//域外战场已使用的免费传送次数
		YW_FREE=2,
//域外战场已购买灵力次数
		YW_BUY_LING_LI=3,
	}
}
namespace  module.counter4server.model
{
//计数器类型
	public enum ServerCounterType
	{
		ServerCounterType_none=0,
	}
}
namespace  module.equip.message
{
//请求整理装备背包
	[Message(OuterOpcode.ClassifyEquipReq)]
	[ProtoContract]
	public partial class ClassifyEquipReq: Object, IRequest
	{
		public int RpcId{ get; set; }
		[ProtoMember(1)]
		public module.equip.model.EquipPackType packType { get; set; }

// 如果是仓库,第几页,从1开始
		[ProtoMember(2)]
		public int page { get; set; }

	}
}
namespace  module.equip.message
{
//道具信息改变推送
	[Message(OuterOpcode.EquipChangeResp)]
	[ProtoContract]
	public partial class EquipChangeResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 改变的道具列表
		[ProtoMember(1)]
		public List<module.equip.message.vo.EquipInfo> changeItems = new List<module.equip.message.vo.EquipInfo>();

// 道具包裹类型
		[ProtoMember(2)]
		public module.equip.model.EquipPackType packType { get; set; }

// 要不要提示道具信息
		[ProtoMember(3)]
		public bool tip { get; set; }

	}
}
namespace  module.equip.message
{
//上线推送道具背包信息
	[Message(OuterOpcode.EquipListResp)]
	[ProtoContract]
	public partial class EquipListResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 背包大小
		[ProtoMember(1)]
		public int size_pack { get; set; }

// 仓库大小
		[ProtoMember(2)]
		public int size_store { get; set; }

// 背包的道具
		[ProtoMember(3)]
		public List<module.equip.message.vo.EquipInfo> items_pack = new List<module.equip.message.vo.EquipInfo>();

// 仓库的道具
		[ProtoMember(4)]
		public List<module.equip.message.vo.EquipInfo> items_store = new List<module.equip.message.vo.EquipInfo>();

// 装备栏的道具
		[ProtoMember(5)]
		public List<module.equip.message.vo.EquipInfo> items_equip = new List<module.equip.message.vo.EquipInfo>();

	}
}
namespace  module.equip.model
{
//背包类型
	public enum EquipPackType
	{
		Equip_TYPE_NONE=0,
//仓库
		Equip_STORE_TYPE=1,
//装备背包
		Equip_PACK_TYPE=2,
//装备栏
		Equip_EQUIP_TYPE=3,
	}
}
namespace  module.equip.model
{
//装备类型,也是装备的佩戴位
	public enum EquipType
	{
//武器
		TYPE_WEAPON=1,
//衣服
		TYPE_CLOTHES=2,
//披风
		TYPE_CLOAK=3,
//腰带
		TYPE_BELT=4,
//鞋子
		TYPE_SHOES=5,
//帽子
		TYPE_CAP=6,
//戒指
		TYPE_RING=7,
//项链
		TYPE_NECKLACE=8,
	}
}
namespace  module.errorcode.message
{
//
	[Message(OuterOpcode.ErrorCodeResp)]
	[ProtoContract]
	public partial class ErrorCodeResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 错误号
		[ProtoMember(1)]
		public int errorId { get; set; }

// 动态参数
		[ProtoMember(2)]
		public List<string> paramList = new List<string>();

// 发送时间
		[ProtoMember(3)]
		public long createTime { get; set; }

	}
}
namespace  module.fabao.message
{
//法宝附灵,最好是等FaBaoBlessingResp返回了,再发下一次请求,避免连点把洗到的技能给冲掉了
	[Message(OuterOpcode.FaBaoBlessingReq)]
	[ProtoContract]
	public partial class FaBaoBlessingReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 法宝背包类型
		[ProtoMember(1)]
		public module.fabao.packet.impl.FaBaoPackType packType { get; set; }

// 法宝在背包的位置
		[ProtoMember(2)]
		public int packetIndex { get; set; }

	}
}
namespace  module.fabao.message
{
//法宝附灵返回,最好等这个收到了,再发下一次请求,避免连点把洗到的技能给冲掉了
	[Message(OuterOpcode.FaBaoBlessingResp)]
	[ProtoContract]
	public partial class FaBaoBlessingResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 是否成功,失败不用管,服务端会给错误提示,成功的话客户端给个id=27的错误提示
		[ProtoMember(1)]
		public bool success { get; set; }

// 如果成功,获得的技能id
		[ProtoMember(2)]
		public int skillId { get; set; }

	}
}
namespace  module.fabao.message
{
//法宝信息变化推送
	[Message(OuterOpcode.FaBaoChangeResp)]
	[ProtoContract]
	public partial class FaBaoChangeResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 改变的法宝列表
		[ProtoMember(1)]
		public List<module.fabao.message.vo.FaBaoInfo> changeItems = new List<module.fabao.message.vo.FaBaoInfo>();

// 法宝包裹类型
		[ProtoMember(2)]
		public module.fabao.packet.impl.FaBaoPackType packType { get; set; }

// 要不要提示信息
		[ProtoMember(3)]
		public bool tip { get; set; }

	}
}
namespace  module.fabao.message
{
//法宝分解
	[Message(OuterOpcode.FaBaoDecomposeReq)]
	[ProtoContract]
	public partial class FaBaoDecomposeReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 法宝背包类型
		[ProtoMember(1)]
		public module.fabao.packet.impl.FaBaoPackType packType { get; set; }

// 法宝在背包的位置
		[ProtoMember(2)]
		public int packetIndex { get; set; }

	}
}
namespace  module.fabao.message
{
//上线推送法宝背包信息
	[Message(OuterOpcode.FaBaoPacketResp)]
	[ProtoContract]
	public partial class FaBaoPacketResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 背包的法宝
		[ProtoMember(1)]
		public List<module.fabao.message.vo.FaBaoInfo> faBaos_pack = new List<module.fabao.message.vo.FaBaoInfo>();

// 装备栏的法宝
		[ProtoMember(2)]
		public List<module.fabao.message.vo.FaBaoInfo> faBaos_equip = new List<module.fabao.message.vo.FaBaoInfo>();

	}
}
namespace  module.fabao.message
{
//评分提交
	[Message(OuterOpcode.FaBaoScoreCommitReq)]
	[ProtoContract]
	public partial class FaBaoScoreCommitReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 法宝在背包的位置
		[ProtoMember(1)]
		public int packetIndex { get; set; }

// 获得的分数结果
		[ProtoMember(2)]
		public string results { get; set; }

// 第几步,就是FaBaoScoreStartResp里面gotScores的size
		[ProtoMember(3)]
		public int step { get; set; }

	}
}
namespace  module.fabao.message
{
//所有评分步骤已全部完成了,该提示提示,该做表现做表现,这个消息之前已经把最新的法宝信息推送了
	[Message(OuterOpcode.FaBaoScoreOverResp)]
	[ProtoContract]
	public partial class FaBaoScoreOverResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 法宝在背包的位置
		[ProtoMember(1)]
		public int packetIndex { get; set; }

	}
}
namespace  module.fabao.message
{
//法宝评分时,服务端更新法宝的分数
	[Message(OuterOpcode.FaBaoScoreResp)]
	[ProtoContract]
	public partial class FaBaoScoreResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 打造成功了,新铸造成的法宝,在法宝背包的位置
		[ProtoMember(1)]
		public int index { get; set; }

// 分数
		[ProtoMember(2)]
		public List<int> gotScores = new List<int>();

	}
}
namespace  module.fabao.message
{
//开始做评分,点击这个,就开启评分,开始倒计时,直到6次评分完成
	[Message(OuterOpcode.FaBaoScoreStartReq)]
	[ProtoContract]
	public partial class FaBaoScoreStartReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 法宝在背包的位置
		[ProtoMember(1)]
		public int packetIndex { get; set; }

	}
}
namespace  module.fabao.message
{
//开启评分进度条
	[Message(OuterOpcode.FaBaoScoreStartResp)]
	[ProtoContract]
	public partial class FaBaoScoreStartResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 法宝在背包的位置
		[ProtoMember(1)]
		public int packetIndex { get; set; }

// 各个步骤已经获得的分数,list下标就是步骤,所以当前正在进行的步骤,就是下标+1
		[ProtoMember(2)]
		public List<int> gotScores = new List<int>();

// 当前步骤剩余时间,毫秒
		[ProtoMember(3)]
		public int remainTime { get; set; }

		[ProtoMember(4)]
		public string resultStore { get; set; }

		[ProtoMember(5)]
		public int storeIndex { get; set; }

	}
}
namespace  module.fabao.message
{
//穿戴or脱下法宝
	[ResponseType(nameof(FaBaoWearResp))]
	[Message(OuterOpcode.FaBaoWearReq)]
	[ProtoContract]
	public partial class FaBaoWearReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 操作的法宝的位置,也就是fabaoInfo里的index
		[ProtoMember(1)]
		public int packetIndex { get; set; }

// 佩戴目标位置,脱装备可以不传
		[ProtoMember(2)]
		public int wearIndex { get; set; }

// 0是脱,1是穿,2是装备栏换位置
		[ProtoMember(3)]
		public int actionType { get; set; }

	}
}
namespace  module.fabao.message
{
//穿戴成功
	[Message(OuterOpcode.FaBaoWearResp)]
	[ProtoContract]
	public partial class FaBaoWearResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
	}
}
namespace  module.fabao.message
{
//请求法宝铸造,第一步
	[Message(OuterOpcode.FaBaoZhuZaoReq)]
	[ProtoContract]
	public partial class FaBaoZhuZaoReq: Object, IRequest
	{
		public int RpcId{ get; set; }
		[ProtoMember(1)]
		public int fbAuxiId { get; set; }

		[ProtoMember(2)]
		public List<module.item.message.ItemInfo> cailiao = new List<module.item.message.ItemInfo>();

	}
}
namespace  module.fabao.message
{
//法宝铸造,第一步成功返回
	[Message(OuterOpcode.FaBaoZhuZaoResp)]
	[ProtoContract]
	public partial class FaBaoZhuZaoResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 打造成功了,新铸造成的法宝,在法宝背包的位置
		[ProtoMember(1)]
		public int index { get; set; }

	}
}
namespace  module.identity.model
{
	public enum IdentityType
	{
		Identity_none=0,
		ID_ROLE=1,
		ID_ITEM=2,
		ID_MAIL=3,
		ID_ACCOUNT=4,
		ID_BATTLE=5,
		ID_FABAO=6,
		ID_SERVER_TIME=7,
		ID_PET=8,
		ID_EQUIP=9,
	}
}
namespace  module.item.message
{
//打开图鉴请求
	[Message(OuterOpcode.DrugsTuJianReq)]
	[ProtoContract]
	public partial class DrugsTuJianReq: Object, IRequest
	{
		public int RpcId{ get; set; }
	}
}
namespace  module.item.message
{
//图鉴暂时没必要上线推,以后如果要做红点,改成上线推就行了
	[Message(OuterOpcode.DrugsTuJianResp)]
	[ProtoContract]
	public partial class DrugsTuJianResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 丹药总属性
		[ProtoMember(1)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,long> totalAttr { get; set; }

// 丹药图鉴记录
		[ProtoMember(2)]
		public List<module.item.message.vo.DrugsTuJianVo> tuJians = new List<module.item.message.vo.DrugsTuJianVo>();

// 已经学会的配方id,就是丹药id
		[ProtoMember(3)]
		public List<int> peiFangs = new List<int>();

	}
}
namespace  module.item.message
{
//道具信息
	[ProtoContract]
	public partial class ItemInfo: Object
	{
// 背包格子索引
		[ProtoMember(1)]
		public int index { get; set; }

// 道具配置表的id,为0表示是删除
		[ProtoMember(2)]
		public int itemId { get; set; }

// 数量,为0表示是删除这个格子位置的道具
		[ProtoMember(3)]
		public int amount { get; set; }

// 道具过期时间戳
		[ProtoMember(4)]
		public long timeLimit { get; set; }

// 道具可以使用的时间
		[ProtoMember(5)]
		public long canUseTime { get; set; }

// 药效
		[ProtoMember(6)]
		public int drugsEffect { get; set; }

// 属性 key value 属性值
		[ProtoMember(7)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,long> attri { get; set; }

	}
}
namespace  module.item.message
{
//使用物品
	[Message(OuterOpcode.ItemUseReq)]
	[ProtoContract]
	public partial class ItemUseReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 道具在背包中的位置
		[ProtoMember(1)]
		public int itemIndex { get; set; }

// 如果是给宠物用,宠物的id, 否则就是给玩家自己用
		[ProtoMember(2)]
		public long petUuid { get; set; }

		[ProtoMember(3)]
		public int useNum { get; set; }

	}
}
namespace  module.item.message
{
//学会了一种新的丹药配方
	[Message(OuterOpcode.LearnDrugsPeiFangResp)]
	[ProtoContract]
	public partial class LearnDrugsPeiFangResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 丹药id
		[ProtoMember(1)]
		public int drugsId { get; set; }

	}
}
namespace  module.item.message
{
//开始炼丹
	[ResponseType(nameof(LianDanResp))]
	[Message(OuterOpcode.LianDanReq)]
	[ProtoContract]
	public partial class LianDanReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 要练的丹药id, 也就是收到后端发的那个配方id
		[ProtoMember(1)]
		public int drugsId { get; set; }

		[ProtoMember(2)]
		public List<int> cailiao = new List<int>();

	}
}
namespace  module.item.message
{
//炼丹成功返回
	[Message(OuterOpcode.LianDanResp)]
	[ProtoContract]
	public partial class LianDanResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
	}
}
namespace  module.item.message
{
//快捷购买
	[Message(OuterOpcode.QuickBuyItemReq)]
	[ProtoContract]
	public partial class QuickBuyItemReq: Object, IRequest
	{
		public int RpcId{ get; set; }
		[ProtoMember(1)]
		public int itemId { get; set; }

		[ProtoMember(2)]
		public int count { get; set; }

	}
}
namespace  module.item.message
{
//快捷购买成功
	[Message(OuterOpcode.QuickBuyItemResp)]
	[ProtoContract]
	public partial class QuickBuyItemResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public int itemId { get; set; }

		[ProtoMember(2)]
		public int count { get; set; }

	}
}
namespace  module.item.useimpl
{
	public enum ItemEffectType
	{
		ItemEffect_none=0,
		Drugs=4,
	}
}
namespace  module.level.message
{
//与升级相关的一些额外属性,上线和有变化时都发这个消息
	[Message(OuterOpcode.LevelExtResp)]
	[ProtoContract]
	public partial class LevelExtResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 当前等级总的失败次数
		[ProtoMember(1)]
		public int failCount { get; set; }

// 当前等级使用丹药增加的成功率
		[ProtoMember(2)]
		public int probAdd { get; set; }

	}
}
namespace  module.level.message
{
//
	[Message(OuterOpcode.LevelUpReq)]
	[ProtoContract]
	public partial class LevelUpReq: Object, IRequest
	{
		public int RpcId{ get; set; }
	}
}
namespace  module.level.message
{
//
	[Message(OuterOpcode.LevelUpResp)]
	[ProtoContract]
	public partial class LevelUpResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public int newLv { get; set; }

		[ProtoMember(2)]
		public long exp { get; set; }

	}
}
namespace  module.login.message
{
	[ResponseType(nameof(LoginResp))]
	[Message(OuterOpcode.LoginReq)]
	[ProtoContract]
	public partial class LoginReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 账号id
		[ProtoMember(1)]
		public string accountId { get; set; }

		[ProtoMember(2)]
		public string accountName { get; set; }

		[ProtoMember(3)]
		public int serverId { get; set; }

	}
}
namespace  module.login.message
{
//
	[Message(OuterOpcode.LoginResp)]
	[ProtoContract]
	public partial class LoginResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public long playerId { get; set; }

		[ProtoMember(2)]
		public int createTime { get; set; }

	}
}
namespace  module.mail.message
{
//请求邮件删除
	[Message(OuterOpcode.MailDeleteReq)]
	[ProtoContract]
	public partial class MailDeleteReq: Object, IRequest
	{
		public int RpcId{ get; set; }
		[ProtoMember(1)]
		public List<long> deleteList = new List<long>();

	}
}
namespace  module.mail.message
{
//请求邮件阅读
	[Message(OuterOpcode.MailReadReq)]
	[ProtoContract]
	public partial class MailReadReq: Object, IRequest
	{
		public int RpcId{ get; set; }
		[ProtoMember(1)]
		public List<long> readList = new List<long>();

	}
}
namespace  module.mail.message
{
//请求领取附件
	[Message(OuterOpcode.MailReceiveReq)]
	[ProtoContract]
	public partial class MailReceiveReq: Object, IRequest
	{
		public int RpcId{ get; set; }
		[ProtoMember(1)]
		public List<long> reciveList = new List<long>();

	}
}
namespace  module.mail.message
{
//上线初始化/新增邮件,删除,更新邮件都是这一个消息
	[Message(OuterOpcode.MailResp)]
	[ProtoContract]
	public partial class MailResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 初始化/增加/删除/更新的邮件
		[ProtoMember(1)]
		public List<module.mail.message.vo.MailVO> mailList = new List<module.mail.message.vo.MailVO>();

	}
}
namespace  module.mail.message
{
//邮件状态
	public enum MailStatus
	{
		MailStatus_NONE=0,
//创建
		CREATE=1,
//已阅读
		READ=2,
//已领取
		RECV=3,
//已删除
		DELETE=4,
	}
}
namespace  module.mail.message
{
	public enum MailType
	{
		MailType_NONE=0,
		SYSTEM=1,
		GLOBAL=2,
	}
}
namespace  module.mail.model
{
//邮件每个奖励的类型
	public enum MailAttachType
	{
		MailAttachType_NONE=0,
//道具
		ITEM=1,
//法宝
		FABAO=2,
//宠物
		PET=3,
//装备
		Equip=4,
	}
}
namespace  module.pack.message
{
//请求整理背包
	[Message(OuterOpcode.ClassifyPackReq)]
	[ProtoContract]
	public partial class ClassifyPackReq: Object, IRequest
	{
		public int RpcId{ get; set; }
		[ProtoMember(1)]
		public module.pack.model.PackType packType { get; set; }

// 如果是仓库,第几页,从1开始
		[ProtoMember(2)]
		public int page { get; set; }

	}
}
namespace  module.pack.message
{
//道具信息改变推送
	[Message(OuterOpcode.ItemChangeResp)]
	[ProtoContract]
	public partial class ItemChangeResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 改变的道具列表
		[ProtoMember(1)]
		public List<module.item.message.ItemInfo> changeItems = new List<module.item.message.ItemInfo>();

// 道具包裹类型
		[ProtoMember(2)]
		public module.pack.model.PackType packType { get; set; }

// 要不要提示道具信息
		[ProtoMember(3)]
		public bool tip { get; set; }

	}
}
namespace  module.pack.message
{
//请求卖东西
	[Message(OuterOpcode.ItemSellReq)]
	[ProtoContract]
	public partial class ItemSellReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 在背包的位置
		[ProtoMember(1)]
		public int packetIndex { get; set; }

// 数量
		[ProtoMember(2)]
		public int num { get; set; }

	}
}
namespace  module.pack.message
{
//请求扩容
	[Message(OuterOpcode.PackExpansionReq)]
	[ProtoContract]
	public partial class PackExpansionReq: Object, IRequest
	{
		public int RpcId{ get; set; }
		[ProtoMember(1)]
		public module.pack.model.PackType packType { get; set; }

	}
}
namespace  module.pack.message
{
//包裹大小变化,一般用于扩容之后
	[Message(OuterOpcode.PackSizeChangeResp)]
	[ProtoContract]
	public partial class PackSizeChangeResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public int size { get; set; }

		[ProtoMember(2)]
		public module.pack.model.PackType packType { get; set; }

	}
}
namespace  module.pack.message
{
//请求仓库存or取
	[Message(OuterOpcode.PackStoreReq)]
	[ProtoContract]
	public partial class PackStoreReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 0:存 1:取
		[ProtoMember(1)]
		public int reqType { get; set; }

// 位置,存就是背包的位置,取就是仓库的位置,这里发list是方便以后做一键存取多个
		[ProtoMember(2)]
		public List<int> indexs = new List<int>();

// 如果是存入仓库,是第几页,从1开始
		[ProtoMember(3)]
		public int page { get; set; }

	}
}
namespace  module.pack.message
{
//上线推送道具背包信息
	[Message(OuterOpcode.PackageResp)]
	[ProtoContract]
	public partial class PackageResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 背包大小
		[ProtoMember(1)]
		public int size_pack { get; set; }

// 仓库大小
		[ProtoMember(2)]
		public int size_store { get; set; }

// 背包的道具
		[ProtoMember(3)]
		public List<module.item.message.ItemInfo> items_pack = new List<module.item.message.ItemInfo>();

// 仓库的道具
		[ProtoMember(4)]
		public List<module.item.message.ItemInfo> items_store = new List<module.item.message.ItemInfo>();

// 装备栏的道具
		[ProtoMember(5)]
		public List<module.item.message.ItemInfo> items_equip = new List<module.item.message.ItemInfo>();

	}
}
namespace  module.pack.message
{
//穿脱装备
	[Message(OuterOpcode.WearEquipReq)]
	[ProtoContract]
	public partial class WearEquipReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// =0穿,=1脱
		[ProtoMember(1)]
		public int opType { get; set; }

// opType=0时背包位置,opType=1时装备位置
		[ProtoMember(2)]
		public int index { get; set; }

	}
}
namespace  module.pack.model
{
	public enum ItemType
	{
		ItemType_NONE=0,
		ITEM_MONEY=1,
		ITEM_EQUIPMENT=2147483646,
	}
}
namespace  module.pack.model
{
//背包类型
	public enum PackType
	{
		PACK_TYPE_NONE=0,
//仓库
		STORE_TYPE=1,
//背包
		PACK_TYPE=2,
	}
}
namespace  module.pet.message
{
//新增一只宠物or宠物信息变化
	[Message(OuterOpcode.PetAddOrUpdateResp)]
	[ProtoContract]
	public partial class PetAddOrUpdateResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public module.pet.message.vo.PetInfo petInfo { get; set; }

	}
}
namespace  module.pet.message
{
//开始抓捕
	[Message(OuterOpcode.PetCatchReq)]
	[ProtoContract]
	public partial class PetCatchReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 果子id,下标就是第几个笼子
		[ProtoMember(1)]
		public List<int> guozi = new List<int>();

	}
}
namespace  module.pet.message
{
//如果抓捕成功了,返回结果
	[Message(OuterOpcode.PetCatchResp)]
	[ProtoContract]
	public partial class PetCatchResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 结果_lcarrestprobability表的id,下标就是第几个笼子
		[ProtoMember(1)]
		public List<int> results = new List<int>();

	}
}
namespace  module.pet.message
{
//宠物出战
	[ResponseType(nameof(PetChuZhanResp))]
	[Message(OuterOpcode.PetChuZhanReq)]
	[ProtoContract]
	public partial class PetChuZhanReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 宠物uuid
		[ProtoMember(1)]
		public long petUuid { get; set; }

// 要出战的位置,是从1开始的,第一个位置就是1
		[ProtoMember(2)]
		public int zhanIndex { get; set; }

	}
}
namespace  module.pet.message
{
//宠物出战
	[Message(OuterOpcode.PetChuZhanResp)]
	[ProtoContract]
	public partial class PetChuZhanResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
	}
}
namespace  module.pet.message
{
//删除一只宠物
	[Message(OuterOpcode.PetDeleteResp)]
	[ProtoContract]
	public partial class PetDeleteResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public long petUuid { get; set; }

	}
}
namespace  module.pet.message
{
//宠物进化
	[ResponseType(nameof(PetJinhuaResp))]
	[Message(OuterOpcode.PetJinhuaReq)]
	[ProtoContract]
	public partial class PetJinhuaReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 要进化的宠物id
		[ProtoMember(1)]
		public long petUuid { get; set; }

// 被吃的宠物id
		[ProtoMember(2)]
		public long eatPetUuid { get; set; }

	}
}
namespace  module.pet.message
{
//宠物进化
	[Message(OuterOpcode.PetJinhuaResp)]
	[ProtoContract]
	public partial class PetJinhuaResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
	}
}
namespace  module.pet.message
{
//上线推送宠物列表
	[Message(OuterOpcode.PetListResp)]
	[ProtoContract]
	public partial class PetListResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public List<module.pet.message.vo.PetInfo> petList = new List<module.pet.message.vo.PetInfo>();

	}
}
namespace  module.player.message
{
//属性变化消息
	[Message(OuterOpcode.AttrChangeResp)]
	[ProtoContract]
	public partial class AttrChangeResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// key是枚举AttrEnum, 发生改变的属性,里面发的是当前的最新值. 如果需要做提示改变了多少值,需要前端,拿老的值,减去当前值
		[ProtoMember(1)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,long> changes { get; set; }

	}
}
namespace  module.player.message
{
//上线推送角色信息
	[Message(OuterOpcode.PlayerInfoResp)]
	[ProtoContract]
	public partial class PlayerInfoResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public long playerId { get; set; }

		[ProtoMember(2)]
		public string name { get; set; }

		[ProtoMember(3)]
		public int level { get; set; }

		[ProtoMember(4)]
		public long exp { get; set; }

// key是枚举AttrEnum
		[ProtoMember(5)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,long> attributes { get; set; }

// key是枚举ResourceType
		[ProtoMember(6)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,long> resources { get; set; }

	}
}
namespace  module.player.message
{
//资源变化消息
	[Message(OuterOpcode.ResourceResp)]
	[ProtoContract]
	public partial class ResourceResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// key是枚举ResourceType,发生改变的资源,里面发的是当前的最新值. 如果需要做提示改变了多少值,需要前端,拿老的值,减去当前值
		[ProtoMember(1)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,long> changes { get; set; }

	}
}
namespace  module.record.domain
{
//记录类型
	public enum RecordType
	{
		CommType_NONE=0,
//人物升级的一些记录
		LEVEL_UP=1,
		YW_TOTAL_MAP_REWARD=2,
		DRUGS_TUJIAN=3,
	}
}
namespace  module.role.model
{
//资源类型
	public enum ResourceType
	{
		RESOURCE_TYPE_NONE=0,
//仙玉
		XIAN_YU=1001,
//灵石
		LING_SHI=1002,
//宝灵
		BAO_LING=1003,
	}
}
namespace  module.roleid.model
{
	public enum RoleIdNumSize
	{
		RoleIdNumSize_NONE=0,
		SIZE_5=5,
		SIZE_6=6,
		SIZE_7=7,
		SIZE_8=8,
		SIZE_9=9,
		SIZE_10=10,
	}
}
namespace  module.session.message
{
	[ResponseType(nameof(HeartResp))]
	[Message(OuterOpcode.HeartReq)]
	[ProtoContract]
	public partial class HeartReq: Object, IRequest
	{
		public int RpcId{ get; set; }
	}
}
namespace  module.session.message
{
//
	[Message(OuterOpcode.HeartResp)]
	[ProtoContract]
	public partial class HeartResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public long time { get; set; }

// 服务器时间偏移值,修改服务器时间的GM命里就是修改这个值
		[ProtoMember(2)]
		public long offset { get; set; }

	}
}
namespace  module.skill.domain
{
//五行
	public enum FiveElementsType
	{
		FiveElements_NONE=0,
		jin=1,
		mu=2,
		shui=3,
		huo=4,
		tu=5,
	}
}
namespace  module.task.domain
{
	public enum TaskParam
	{
		TaskParam_none=0,
		Task_petId=1,
	}
}
namespace  module.task.domain
{
//任务状态
	public enum TaskState
	{
		TaskState_None=0,
//已接受,未完成
		accept=1,
//已完成,未领取奖励
		finish=2,
//已领取奖励
		done=3,
	}
}
namespace  module.task.domain
{
//任务类型
	public enum TaskType
	{
		TASK_NONE=0,
		TASK_DAILY=1,
		TASK_MAIN=2,
		TASK_CHENG=3,
	}
}
namespace  module.task.message
{
//上线推送任务列表/更新任务进度也发这个消息
	[Message(OuterOpcode.TaskAddOrUpdateResp)]
	[ProtoContract]
	public partial class TaskAddOrUpdateResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public List<module.task.message.vo.TaskVo> taskList = new List<module.task.message.vo.TaskVo>();

	}
}
namespace  module.task.message
{
//删除任务,主要就是日常每日24点自动删除
	[Message(OuterOpcode.TaskDeleteResp)]
	[ProtoContract]
	public partial class TaskDeleteResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public List<int> deletes = new List<int>();

	}
}
namespace  module.task.message
{
//领取任务奖励
	[Message(OuterOpcode.TaskTakeRewardReq)]
	[ProtoContract]
	public partial class TaskTakeRewardReq: Object, IRequest
	{
		public int RpcId{ get; set; }
		[ProtoMember(1)]
		public int taskId { get; set; }

	}
}
namespace  module.test.message
{
//Gm命令列表
	[ResponseType(nameof(GmListResp))]
	[Message(OuterOpcode.GmListReq)]
	[ProtoContract]
	public partial class GmListReq: Object, IRequest
	{
		public int RpcId{ get; set; }
	}
}
namespace  module.test.message
{
//Gm命令列表
	[Message(OuterOpcode.GmListResp)]
	[ProtoContract]
	public partial class GmListResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 测试命令列表
		[ProtoMember(1)]
		public List<string> cmds = new List<string>();

	}
}
namespace  module.test.message
{
//Gm命令
	[ResponseType(nameof(GmResp))]
	[Message(OuterOpcode.GmReq)]
	[ProtoContract]
	public partial class GmReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// Gm命令
		[ProtoMember(1)]
		public string cmdStr { get; set; }

	}
}
namespace  module.test.message
{
//Gm命令返回执行结果
	[Message(OuterOpcode.GmResp)]
	[ProtoContract]
	public partial class GmResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 命令行结果
		[ProtoMember(1)]
		public string resultStr { get; set; }

	}
}
namespace  module.yw.domain
{
//
	[ProtoContract]
	public partial class Information: Object
	{
		[ProtoMember(1)]
		public int id { get; set; }

// 发生事件的时间,单位秒
		[ProtoMember(2)]
		public long time { get; set; }

		[ProtoMember(3)]
		public List<string> paramList = new List<string>();

	}
}
namespace  module.yw.message
{
//购买灵力
	[Message(OuterOpcode.YwBuyLingLiReq)]
	[ProtoContract]
	public partial class YwBuyLingLiReq: Object, IRequest
	{
		public int RpcId{ get; set; }
	}
}
namespace  module.yw.message
{
//购买域外包裹
	[Message(OuterOpcode.YwBuyPacketReq)]
	[ProtoContract]
	public partial class YwBuyPacketReq: Object, IRequest
	{
		public int RpcId{ get; set; }
	}
}
namespace  module.yw.message
{
//购买域外包裹
	[Message(OuterOpcode.YwBuyPacketResp)]
	[ProtoContract]
	public partial class YwBuyPacketResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 背包大小
		[ProtoMember(1)]
		public int packetSize { get; set; }

	}
}
namespace  module.yw.message
{
//请求进入域外战场
	[ResponseType(nameof(YwEntryResp))]
	[Message(OuterOpcode.YwEntryReq)]
	[ProtoContract]
	public partial class YwEntryReq: Object, IRequest
	{
		public int RpcId{ get; set; }
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

	}
}
namespace  module.yw.message
{
//进入返回
	[Message(OuterOpcode.YwEntryResp)]
	[ProtoContract]
	public partial class YwEntryResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

		[ProtoMember(2)]
		public int x { get; set; }

		[ProtoMember(3)]
		public int y { get; set; }

	}
}
namespace  module.yw.message
{
//请求离开域外战场
	[ResponseType(nameof(YwExitResp))]
	[Message(OuterOpcode.YwExitReq)]
	[ProtoContract]
	public partial class YwExitReq: Object, IRequest
	{
		public int RpcId{ get; set; }
	}
}
namespace  module.yw.message
{
//离开了域外战场
	[Message(OuterOpcode.YwExitResp)]
	[ProtoContract]
	public partial class YwExitResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
	}
}
namespace  module.yw.message
{
//增加一条域外频道信息,如果前端还没执行过informationReq,可以直接无视这个消息
	[Message(OuterOpcode.YwInformationAddResp)]
	[ProtoContract]
	public partial class YwInformationAddResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public module.yw.domain.Information addInfo { get; set; }

	}
}
namespace  module.yw.message
{
//请求域外战场的频道信息,这个信息因为比较多而且是跨服的
//所以客户端维护一下,进入战场后如果从来没有请求过,就请求一次一个玩家在游戏过程中只请求一次
	[Message(OuterOpcode.YwInformationListReq)]
	[ProtoContract]
	public partial class YwInformationListReq: Object, IRequest
	{
		public int RpcId{ get; set; }
	}
}
namespace  module.yw.message
{
//域外战场的频道信息返回
	[Message(OuterOpcode.YwInformationListResp)]
	[ProtoContract]
	public partial class YwInformationListResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public List<module.yw.domain.Information> infoList = new List<module.yw.domain.Information>();

	}
}
namespace  module.yw.message
{
//域外战场灵力更新
	[Message(OuterOpcode.YwLingLiUpdateResp)]
	[ProtoContract]
	public partial class YwLingLiUpdateResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 当前拥有的灵力数量
		[ProtoMember(1)]
		public int lingLi { get; set; }

// 最后自动恢复时间
		[ProtoMember(2)]
		public long lastUpdateTime { get; set; }

	}
}
namespace  module.yw.message
{
//域外战场当前所在的格子信息
	[Message(OuterOpcode.YwMapPosInfoResp)]
	[ProtoContract]
	public partial class YwMapPosInfoResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

		[ProtoMember(2)]
		public int x { get; set; }

		[ProtoMember(3)]
		public int y { get; set; }

		[ProtoMember(4)]
		public List<module.yw.message.vo.YwPlayerVo> playerList = new List<module.yw.message.vo.YwPlayerVo>();

// 是否已探索
		[ProtoMember(5)]
		public bool tansuo { get; set; }

// 秘境信息,如果没有秘境,该字段为空
		[ProtoMember(6)]
		public module.yw.message.vo.MiJingVo miJing { get; set; }

	}
}
namespace  module.yw.message
{
//一个新的秘境产生了,广播给在这个地图的所有玩家
	[Message(OuterOpcode.YwMiJingCreateResp)]
	[ProtoContract]
	public partial class YwMiJingCreateResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public module.yw.message.vo.MiJingVo miJing { get; set; }

	}
}
namespace  module.yw.message
{
//秘境更新
	[Message(OuterOpcode.YwMiJingUpdateResp)]
	[ProtoContract]
	public partial class YwMiJingUpdateResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

		[ProtoMember(2)]
		public int x { get; set; }

		[ProtoMember(3)]
		public int y { get; set; }

// 1是已探索,2是提前关闭了
		[ProtoMember(4)]
		public int action { get; set; }

	}
}
namespace  module.yw.message
{
//请求移动
	[Message(OuterOpcode.YwMoveReq)]
	[ProtoContract]
	public partial class YwMoveReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 这里需要把mapId带过来,是因为,可能他开着面板时,被别人打出去了,所以服务端要验证所在地图是否正确
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

// x,自己要去哪个格子
		[ProtoMember(2)]
		public int x { get; set; }

// y,自己要去哪格子
		[ProtoMember(3)]
		public int y { get; set; }

	}
}
namespace  module.yw.message
{
//进入地图或打开界面时,覆盖更新玩家已经走过的格子
//同时前端收到YwMapPosInfoResp时也自己把这里面的坐标维护一下到已行走过的格子吧
	[Message(OuterOpcode.YwMovedPosResp)]
	[ProtoContract]
	public partial class YwMovedPosResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 当地图di,前端需要判断只有当自己所在的地图是这个地图,才需要更新
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

// 已经走过的格子,直接覆盖就行
		[ProtoMember(2)]
		public List<module.Pos> movedPath = new List<module.Pos>();

	}
}
namespace  module.yw.message
{
//从主界面打开域外战场
	[ResponseType(nameof(YwOpenResp))]
	[Message(OuterOpcode.YwOpenReq)]
	[ProtoContract]
	public partial class YwOpenReq: Object, IRequest
	{
		public int RpcId{ get; set; }
	}
}
namespace  module.yw.message
{
//从主界面打开域外战场返回消息
	[Message(OuterOpcode.YwOpenResp)]
	[ProtoContract]
	public partial class YwOpenResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 当前所在地图di,=0表示没在战场地图
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

		[ProtoMember(2)]
		public int x { get; set; }

		[ProtoMember(3)]
		public int y { get; set; }

	}
}
namespace  module.yw.message
{
//直接覆盖域外战场的储物袋,有其他玩家会抢劫,懒得一个一个更新了
	[Message(OuterOpcode.YwPacketResp)]
	[ProtoContract]
	public partial class YwPacketResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 背包大小
		[ProtoMember(1)]
		public int packetSize { get; set; }

// 直接覆盖掉背包的内容,其中里面的index,可能会超出packetSize--策划要求满了也能继续添加
		[ProtoMember(2)]
		public List<module.common.model.CommItemInfo> rewards = new List<module.common.model.CommItemInfo>();

	}
}
namespace  module.yw.message
{
//请求pK
	[Message(OuterOpcode.YwPkReq)]
	[ProtoContract]
	public partial class YwPkReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 这里需要把mapId带过来,是因为,可能他开着面板时,被别人打出去了,所以服务端要验证所在地图是否正确
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

// 自己当前所在的格子x
		[ProtoMember(2)]
		public int x { get; set; }

// 自己当前所在的格子y
		[ProtoMember(3)]
		public int y { get; set; }

// 目标角色id
		[ProtoMember(4)]
		public long targetPlayerId { get; set; }

// 目标角色名字
		[ProtoMember(5)]
		public string targetPlayerName { get; set; }

	}
}
namespace  module.yw.message
{
//战斗结果返回
	[Message(OuterOpcode.YwPkResp)]
	[ProtoContract]
	public partial class YwPkResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 战报
		[ProtoMember(1)]
		public module.battle.domain.BattleRecord record { get; set; }

	}
}
namespace  module.yw.message
{
//请求探索当前所在的格子的秘境
	[Message(OuterOpcode.YwTanSuoMiJingReq)]
	[ProtoContract]
	public partial class YwTanSuoMiJingReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 这里需要把mapId带过来,是因为,可能他开着面板时,被别人打出去了,所以服务端要验证所在地图是否正确
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

// 自己当前所在的格子x
		[ProtoMember(2)]
		public int x { get; set; }

// 自己当前所在的格子y
		[ProtoMember(3)]
		public int y { get; set; }

	}
}
namespace  module.yw.message
{
//请求探索当前所在的格子
	[Message(OuterOpcode.YwTanSuoReq)]
	[ProtoContract]
	public partial class YwTanSuoReq: Object, IRequest
	{
		public int RpcId{ get; set; }
// 这里需要把mapId带过来,是因为,可能他开着面板时,被别人打出去了,所以服务端要验证所在地图是否正确
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

// 自己当前所在的格子x
		[ProtoMember(2)]
		public int x { get; set; }

// 自己当前所在的格子y
		[ProtoMember(3)]
		public int y { get; set; }

	}
}
namespace  module.yw.message
{
//请求探索成功,返回被探索的地图和坐标
	[Message(OuterOpcode.YwTanSuoResp)]
	[ProtoContract]
	public partial class YwTanSuoResp: Object, IResponse
	{
		public int RpcId{ get; set; }
		public int Error{ get; set; }
		public string Message{ get; set; }
// 返回被探索的地图
		[ProtoMember(1)]
		public int lowerMapId { get; set; }

// 返回被探索的地图x
		[ProtoMember(2)]
		public int x { get; set; }

// 返回被探索的地图y
		[ProtoMember(3)]
		public int y { get; set; }

	}
}
namespace  module.equip.message.vo
{
//装备信息
	[ProtoContract]
	public partial class EquipInfo: Object
	{
// 装备背包格子索引
		[ProtoMember(1)]
		public int index { get; set; }

// 装备配置表的id,为0表示是删除
		[ProtoMember(2)]
		public int equipId { get; set; }

// 属性 key value 属性值
		[ProtoMember(3)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,long> attri { get; set; }

	}
}
namespace  module.fabao.message.vo
{
//
	[ProtoContract]
	public partial class FaBaoInfo: Object
	{
// 背包索引
		[ProtoMember(1)]
		public int index { get; set; }

// 法宝id,为0表示是删除
		[ProtoMember(2)]
		public int faBaoId { get; set; }

// 法宝阶数,=0表示是个胚子
		[ProtoMember(3)]
		public int stage { get; set; }

// 基础属性 key value 属性值
		[ProtoMember(4)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,long> attri { get; set; }

// 附加属性 key value 属性值
		[ProtoMember(5)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,long> attri_Spe { get; set; }

// 评分阶段的得分情况,可能为null--胚子从来没开始评分,或当法宝打造完成,已经不是胚子,这个就一定是null
		[ProtoMember(6)]
		public List<int> gotScores = new List<int>();

// 技能
		[ProtoMember(7)]
		public int skillId { get; set; }

// 打造时间戳,秒
		[ProtoMember(8)]
		public long createTime { get; set; }

	}
}
namespace  module.fabao.packet.impl
{
//法宝背包类型
	public enum FaBaoPackType
	{
		FaBaoPackType_NONE=0,
//仓库
		STORE_TYPE=1,
//背包
		PACK_TYPE=2,
//装备栏
		EQUIP_TYPE=3,
	}
}
namespace  module.item.message.vo
{
//道具使用记录
	[ProtoContract]
	public partial class DrugsTuJianVo: Object
	{
		[ProtoMember(1)]
		public int itemId { get; set; }

// 生效效果次数
		[ProtoMember(2)]
		public int effectCount { get; set; }

// 效果总值
		[ProtoMember(3)]
		public int effectValue { get; set; }

// 效果属性
		[ProtoMember(4)]
		public module.attri.AttrEnum attr { get; set; }

// 最低药效,万分比的int值,1000表示10%
		[ProtoMember(5)]
		public int minEffect { get; set; }

	}
}
namespace  module.mail.message.vo
{
//邮件附件奖励
	[ProtoContract]
	public partial class MailAttach: Object
	{
// 附件的类型
		[ProtoMember(1)]
		public module.mail.model.MailAttachType type { get; set; }

// 模型id
		[ProtoMember(2)]
		public int modelId { get; set; }

// 数量
		[ProtoMember(3)]
		public int num { get; set; }

	}
}
namespace  module.mail.message.vo
{
//邮件信息
	[ProtoContract]
	public partial class MailVO: Object
	{
// 唯一id
		[ProtoMember(1)]
		public long id { get; set; }

// 邮件标题
		[ProtoMember(2)]
		public string title { get; set; }

// 邮件内容
		[ProtoMember(3)]
		public string content { get; set; }

// 邮件状态
		[ProtoMember(4)]
		public module.mail.message.MailStatus status { get; set; }

// 发送时间,毫秒
		[ProtoMember(5)]
		public long createTime { get; set; }

// 过期时间,毫秒
		[ProtoMember(6)]
		public long expireTimeMillis { get; set; }

// 附件奖励
		[ProtoMember(7)]
		public List<module.mail.message.vo.MailAttach> attachs = new List<module.mail.message.vo.MailAttach>();

	}
}
namespace  module.pet.message.vo
{
//
	[ProtoContract]
	public partial class PetInfo: Object
	{
		[ProtoMember(1)]
		public long uuid { get; set; }

// 宠物配置表id
		[ProtoMember(2)]
		public int petId { get; set; }

		[ProtoMember(3)]
		public int level { get; set; }

		[ProtoMember(4)]
		public long exp { get; set; }

// 出战位置,1-6
		[ProtoMember(5)]
		public int zhanIndex { get; set; }

		[ProtoMember(6)]
		public List<int> skills = new List<int>();

		[ProtoMember(7)]
		[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
		public Dictionary<int,long> attrs { get; set; }

		[ProtoMember(8)]
		public int jinhuaExp { get; set; }

	}
}
namespace  module.task.message.vo
{
//
	[ProtoContract]
	public partial class TaskVo: Object
	{
// 任务id,前端自己判断一下,如果这个任务id,是当前已存在任务的后置任务,替换已存在的那个任务
		[ProtoMember(1)]
		public int taskId { get; set; }

		[ProtoMember(2)]
		public int process { get; set; }

		[ProtoMember(3)]
		public module.task.domain.TaskState state { get; set; }

	}
}
namespace  module.yw.message.vo
{
//秘境信息
	[ProtoContract]
	public partial class MiJingVo: Object
	{
// ywevent表的id
		[ProtoMember(1)]
		public int eventId { get; set; }

// 开始时间
		[ProtoMember(2)]
		public long startTime { get; set; }

		[ProtoMember(3)]
		public int lowerMapId { get; set; }

		[ProtoMember(4)]
		public int x { get; set; }

		[ProtoMember(5)]
		public int y { get; set; }

		[ProtoMember(6)]
		public bool hadTanSuo { get; set; }

	}
}
namespace  module.yw.message.vo
{
//
	[ProtoContract]
	public partial class YwPetVo: Object
	{
// 宠物的id
		[ProtoMember(1)]
		public int petModelId { get; set; }

// 站位
		[ProtoMember(2)]
		public int site { get; set; }

	}
}
namespace  module.yw.message.vo
{
//格子上的玩家简要信息
	[ProtoContract]
	public partial class YwPlayerSimpleInfo: Object
	{
		[ProtoMember(1)]
		public long playerId { get; set; }

		[ProtoMember(2)]
		public string name { get; set; }

		[ProtoMember(3)]
		public int level { get; set; }

	}
}
namespace  module.yw.message.vo
{
//
	[ProtoContract]
	public partial class YwPlayerVo: Object
	{
		[ProtoMember(1)]
		public module.yw.message.vo.YwPlayerSimpleInfo simpleInfo { get; set; }

		[ProtoMember(2)]
		public List<module.yw.message.vo.YwPetVo> petList = new List<module.yw.message.vo.YwPetVo>();

	}
}
