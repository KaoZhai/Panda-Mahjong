using System;
using System.Collections.Generic;

namespace Utils
{
    public class RoomID
    {
        private const int minID = 100000;
        private const int maxID = 999999;
        private Random rand = new Random();
        private Dictionary<string, int> roomIDDic = new Dictionary<string, int>();

        public int Get(string roomName)
        {
            if (roomIDDic.ContainsKey(roomName))
                return roomIDDic[roomName];

            return roomIDDic[roomName] = rand.Next(minID, maxID);
        }

        public void Del(string roomName)
        {
            if (roomIDDic.ContainsKey(roomName))
                roomIDDic.Remove(roomName);
        }
    }
}