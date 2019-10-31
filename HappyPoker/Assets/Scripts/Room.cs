using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

namespace KBEngine
{
    public class Room : RoomBase
    {
        public override void onGameMupeChanged(int oldValue)
        {
            KBEngine.Event.fireOut("updateGameMupe", new object[] { oldValue });
        }
    }


}
