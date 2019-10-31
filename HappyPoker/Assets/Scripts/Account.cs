using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

namespace KBEngine
{
    public class Account :  AccountBase{

        public override void __init__()
        {
            base.__init__();
            //触发登陆事件
            KBEngine.Event.fireOut("onLoginSuccessfully", new object[] { KBEngineApp.app.entity_uuid, id, this });
        }

        //重载实体的客户端方法,激发返回错误信息事件，在相应界面进行监听该事件
        public override void returnErrorMessage(sbyte arg1)
        {
            Debug.Log("激发事件！");
            KBEngine.Event.fireOut("returnErrorMessage", new object[] { (int)arg1 });
        }

        //重载实体的客户端方法：初始化扑克牌
        public override void initGamePoker()
        {
            KBEngine.Event.fireOut("initGamePoker");
        }

        //告诉客户端叫地主
        public override void notifyToCallLandlord()
        {
            KBEngine.Event.fireOut("notifyToCallLandlord");
        }

        //重载实体的客户端方法：显示玩家准备游戏
        public override void playerReadyGame(sbyte arg1, sbyte arg2)
        {
            KBEngine.Event.fireOut("playerReadyGame", new object[] { (int)arg1, (int)arg2 });
        }

        //告诉客户端谁抢地主了或者不抢地主
        public override void playerToLandlord(sbyte arg1, sbyte arg2)
        {
            KBEngine.Event.fireOut("playerToLandlord", new object[] { (int)arg1, (int)arg2 });
        }

        //告诉客户端加载三张地主牌
        public override void loadLandlordPoker(sbyte arg1, POKERLIST arg2)
        {
            KBEngine.Event.fireOut("loadLandlordPoker", new object[] { (int)arg1, arg2 });
        }

        //通知出牌
        public override void notifyToPlayPoker(sbyte arg1)
        {
            KBEngine.Event.fireOut("notifyToPlayPoker",new object[] { (int)arg1});
        }

        //牌型错误
        public override void playPokerFailed()
        {
            KBEngine.Event.fireOut("playPokerFailed");
        }


        //显示玩家出的牌
        public override void displayPoker(POKERLIST arg1)
        {
            Debug.Log("Account event displayPoker");
            KBEngine.Event.fireOut("displayPoker", new object[] { arg1 });
        }

        //显示过牌
        public override void passPoker()
        {
            KBEngine.Event.fireOut("passPoker");
        }

        //游戏结束
        public override void pokerGameOver(int arg1)
        {
            KBEngine.Event.fireOut("pokerGameOver", new object[] { arg1 });
        }
      
   
        //好友数据
        public override void friendListData(FRIENDDATALIST arg1)
        {
            KBEngine.Event.fireOut("friendListData", new object[] { arg1 });
        }

        //邮件数据
        public override void receiveMail(MAILDATA arg1)
        {
            KBEngine.Event.fireOut("receiveMail", new object[] { arg1 });
        }

        //通知谁继续游戏
        public override void stayInRoom(int arg1)
        {
            KBEngine.Event.fireOut("stayInRoom", new object[] { arg1 });
        }

    }
}
