using System;

namespace mi
{
    [Serializable]
    public class OnTouchListenerResult
    {
        /// <summary>触发此次事件的触摸点列表</summary>
        public Touch[] changedTouches;
        /// <summary>事件触发时的时间戳</summary>
        public long timeStamp;
        /// <summary>当前所有触摸点的列表</summary>
        public Touch[] touches;
    }
}