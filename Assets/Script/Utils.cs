using System;
using System.Collections.Generic;

namespace Utils
{
    public class RoomID
    {
        private const int MinID = 100000;
        private const int MaxID = 999999;
        private readonly Random rand = new Random();
        private readonly Dictionary<string, int> roomIDDic = new Dictionary<string, int>();

        public int Get(string roomName)
        {
            if (roomIDDic.TryGetValue(roomName, out var value))
                return value;

            return roomIDDic[roomName] = rand.Next(MinID, MaxID);
        }

        public void Del(string roomName)
        {
            if (roomIDDic.ContainsKey(roomName))
                roomIDDic.Remove(roomName);
        }
    }
}