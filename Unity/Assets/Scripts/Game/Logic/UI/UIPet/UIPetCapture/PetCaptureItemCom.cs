using FairyGUI;
using ET;
using GameEvent;
using System;
using Frame;

public enum PetCaptureState
{
    empty,
    food,
    prepare,
    capture,
    finish,
}
public class PetCaptureItemCom : UIBase
{
    public Controller ctrlState;
    public GProgressBar timeBar;
    public GTextField textFoodName;
    public GTextField textDes;

    public int selectFoodId;
    public PetCaptureInfo data;
    public PetCaptureState state;
    public Action<int> finishCallback;
}
class PetCaptureItemComAwakeSystem : AwakeSystem<PetCaptureItemCom, GComponent>
{
    protected override void Awake(PetCaptureItemCom self, GComponent a)
	{
		self.Awake(a);
        self.ctrlState = a.GetController("state");
        self.timeBar = a.GetProgressBar("bar");
        self.textFoodName = a.GetText("foodName");
        self.textDes = a.GetText("des");
        var textTitle = a.GetText("title");
        var btnBg = a.GetButton("btnBg");
        for(int i = 0; i < 4; i++)
        {
            self.AddChildWithId<PetCaptureFoodItemCom, GComponent>(i, a.GetCom($"item{i}"));
        }

        textTitle.text = string.Format(ConfigSystem.Ins.GetLanguage("uipet_cagenumname"), self.Id + 1) ;

        btnBg.onClick.Add(self.ClickItem);

    }
}

public static class PetCaptureItemComSystem
{
	
    public static int GetIndex(this PetCaptureItemCom self)
    {
        return (int)self.Id;
    }
    public static void Empty(this PetCaptureItemCom self)
    {
        self.ctrlState.selectedIndex = 0;
        self.state = PetCaptureState.empty;
        self.selectFoodId = 0;
    }
    public static void ShowFood(this PetCaptureItemCom self)
    {
        self.selectFoodId = 0;
        self.ctrlState.selectedIndex = 1;
        self.state = PetCaptureState.food;
        var model = DataSystem.Ins.ItemModel;
        var lingguo = ConfigSystem.Ins.GetGlobal<int[]>("LingGuo");
        for(int i = 0; i < lingguo.Length; i++)
        {
            var item = self.GetChild<PetCaptureFoodItemCom>(i);
            var remainCount = model.GetItemCount(lingguo[i]) - self.GetParent<UIPetCapture>().GetUseFoodCount(lingguo[i]);
            item.Set(lingguo[i], remainCount,self.ClickFoodItem);
        }
    }
    public static void PrepareCapture(this PetCaptureItemCom self)
    {
        self.ctrlState.selectedIndex = 2;
        self.state = PetCaptureState.prepare;
       self.textFoodName.text = ItemUtil.GetItemName(self.selectFoodId);
        self.timeBar.value = 0;
    }
    public static async void CaptureIng(this PetCaptureItemCom self,PetCaptureInfo data)
    {
        self.data = data;
        self.ctrlState.selectedIndex = 2;
        self.state = PetCaptureState.capture;
       self.textFoodName.text = ItemUtil.GetItemName(self.data.foodId);
        var now = TimeHelper.ServerNow();
        if(now >= data.endTime)
        {
            self.Finished();
            return;
        }
        var time = now - data.startTime;
        var max = data.endTime - data.startTime;
        self.timeBar.value = (time + 0.0f) / max * 100;
        self.timeBar.TweenValue(100, (data.endTime - now)/1000.0f);
        var token = self.GetToken(1);
        var flag = await TimerComponent.Instance.WaitAsync(data.endTime - now, token);
        if (!flag) return;
        self.Finished();
    }
    public static void Finished(this PetCaptureItemCom self)
    {
        self.ctrlState.selectedIndex = 3;
        self.state = PetCaptureState.finish;
        int index = (int)self.Id;
        var cfg = ConfigSystem.Ins.GetLCArrestProbability(self.data.lcArrestProbabilityId);
        if (cfg == null) return;
        var param = cfg.Get<int[]>("Reward");
        var type = param[0];
        var id = param[1];
        var num = param[2];
        if(type == 0)
        {
            self.textDes.text = ConfigSystem.Ins.GetLanguage("uipet_capturenothing");
        }else if(type == 1)
        {
            self.textDes.text = string.Format(ConfigSystem.Ins.GetLanguage("gain_item_reward_tip"),ItemUtil.GetItemName(id),num);
        }else if(type == 2)
        {
            var fb = ConfigSystem.Ins.GetFBMagicWeaponConfig(id);
            if (fb != null)
                self.textDes.text = string.Format(ConfigSystem.Ins.GetLanguage("gain_fabao_reward_tip"), fb.Get<string>("FBName"), num);
            else
                self.textDes.text = ConfigSystem.Ins.GetLanguage("uipet_capturenothing");
        }
        else
        {
            var pet = ConfigSystem.Ins.GetLCPetAttribute(id);
            if (pet != null)
                self.textDes.text = string.Format(ConfigSystem.Ins.GetLanguage("gain_pet_reward_tip"), pet.Get<string>("Name"), num);
            else
                self.textDes.text = string.Format(ConfigSystem.Ins.GetLanguage("uipet_petnotfind"), id);
        }
        if(type != 0)
            UISystem.Ins.ShowFlowTipView(self.textDes.text);
        DataSystem.Ins.PetModel.FinishedCapture(index);
        self.finishCallback?.Invoke(index);
    }
    public static void ClickItem(this PetCaptureItemCom self)
    {
        if (self.state == PetCaptureState.empty || self.state == PetCaptureState.prepare)
        {
            self.ShowFood();
        } else if(self.state == PetCaptureState.capture)
        {
            GTween.Kill(self.timeBar);
            self.Finished();
        }
        
    }
    public static void ClickFoodItem(this PetCaptureItemCom self,int itemId)
    {
        self.selectFoodId = itemId;
        self.PrepareCapture();
        self.GetParent<UIPetCapture>().SelectFoodFinish();
    }
}