using FairyGUI;
using ET;
using GameEvent;
using module.pet.message;
using System.Collections.Generic;
using Frame;

public class UIPetCapture : UIBase
{
    public GList msgItemList;
    public Controller ctrlState;

}
class UIPetCaptureAwakeSystem : AwakeSystem<UIPetCapture, GComponent>
{
    protected override void Awake(UIPetCapture self, GComponent a)
	{
		self.Awake(a);
        self.msgItemList = a.GetList("msgList");
        self.ctrlState = a.GetController("state");
        var btnCapture = a.GetButton("btnCapture");
        var btnClose = a.GetButton("btnClose");
        var btnAgain = a.GetButton("btnAgain");

        for(int i = 0; i < ConstValue.PET_CAPTURE_COUNT; i++)
        {
            var item = self.AddChildWithId<PetCaptureItemCom, GComponent>(i, a.GetCom($"item{i}"));
            item.finishCallback = self.CaptureFinishCallback;
        }
        btnCapture.onClick.Add(self.ClickCapture);
        btnAgain.onClick.Add(self.ClickAgain);
        btnClose.onClick.Add(self.ClickClose);

        self.msgItemList.itemRenderer = self.MsgItemRender;

        self.Refresh();
	}
}

public static class UIPetCaptureSystem
{
	public static void Refresh(this UIPetCapture self)
	{
        var model = DataSystem.Ins.PetModel;
        for (int i = 0; i < ConstValue.PET_CAPTURE_COUNT; i++)
        {
            var item = self.GetChild<PetCaptureItemCom>(i);
            var data = model.GetCpatureInfo(i);
            if (data == null)
                item.Empty();
            else
                item.CaptureIng(data);
        }
        self.RefreshState();
        self.msgItemList.numItems = model.captureMsgList.Count;

    }
    public static void RefreshState(this UIPetCapture self)
    {
        var model = DataSystem.Ins.PetModel;
        if (model.captureList.Count == 0)
            self.ctrlState.selectedIndex = 0;
        else if (model.CheckCaptureFinish())
            self.ctrlState.selectedIndex = 2;
        else
            self.ctrlState.selectedIndex = 1;
    }
    public static int GetUseFoodCount(this UIPetCapture self, int itemId)
    {
        int count = 0;
        for (int i = 0; i < ConstValue.PET_CAPTURE_COUNT; i++)
        {
            var item = self.GetChild<PetCaptureItemCom>(i);
            if (item.selectFoodId == itemId)
                count++;
        }
        return count;
    }
    public static void SelectFoodFinish(this UIPetCapture self)
    {
        for (int i = 0; i < ConstValue.PET_CAPTURE_COUNT; i++)
        {
            var item = self.GetChild<PetCaptureItemCom>(i);
            if (item.state == PetCaptureState.food)
                item.ShowFood();
        }
    }
    public static void MsgItemRender(this UIPetCapture self,int index ,GObject go)
    {
        go.asCom.GetText("text").text = DataSystem.Ins.PetModel.captureMsgList[index];
    }
    public static async void AutoCreateMsg(this UIPetCapture self)
    {
        var token = self.GetToken(1);
        var model = DataSystem.Ins.PetModel;
        while (!model.CheckCaptureFinish())
        {
            var flag = await TimerComponent.Instance.WaitAsync(1000,token);
            if (!flag) return;
            model.CreateCpatureMsg();
            self.msgItemList.numItems = model.captureMsgList.Count;
        }
    }
    public static void ClickClose(this UIPetCapture self)
    {
        if (!DataSystem.Ins.PetModel.CheckCaptureFinish())
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("uipet_inthecapture"));
            return;
        }
        DataSystem.Ins.PetModel.CaptureClear();
        self.CloseSelf();
    }
    public static async void ClickCapture(this UIPetCapture self)
    {
        var token = self.GetToken(1);
        var list = new List<int>();
        for(int i = 0; i < ConstValue.PET_CAPTURE_COUNT; i++)
        {
            var item = self.GetChild<PetCaptureItemCom>(i);
            if(item.selectFoodId == 0)
            {
                UISystem.Ins.ShowFlowTipView(string.Format(ConfigSystem.Ins.GetLanguage("uipet_cagenofood"),i + 1));
                return;
            }
            list.Add(item.selectFoodId);
        }
        var req = new PetCatchReq() { guozi = list };
        var resp = (PetCatchResp)await NetSystem.Call(req, typeof(PetCatchResp), token);
        if (resp == null) return;
        for (int i = 0; i < ConstValue.PET_CAPTURE_COUNT; i++)
        {
            DataSystem.Ins.PetModel.captureList[i].foodId = list[i];
        }
        self.Refresh();
        self.AutoCreateMsg();
    }
    public static void ClickAgain(this UIPetCapture self)
    {
        DataSystem.Ins.PetModel.CaptureClear();
        self.Refresh();
    }
    public static void CaptureFinishCallback(this UIPetCapture self, int index)
    {
        self.RefreshState();
    }


}