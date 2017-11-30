using System;
using System.Collections;
using UnityEngine;

namespace Framework.Tools
{
    public static class TimeUtility
    {
        public enum TimeType
        {
            hhmmss,
            mmss,
            ss
        }

        public static void Timer(this MonoBehaviour InMono, float InTime, Action OnFinishedAction)
        {
            InMono.StartCoroutine(TimerEnumerator(InTime, OnFinishedAction));
        }

        public static IEnumerator TimerEnumerator(float InTime, Action OnFinishedAction)
        {
            if (null == OnFinishedAction) yield break;
            yield return new WaitForSeconds(InTime);
            OnFinishedAction();
        }

        public static void Timer(this MonoBehaviour InMono, int InTime, Action OnFinishedAction)
        {
            InMono.StartCoroutine(TimerEnumerator(InTime, OnFinishedAction));
        }

        public static IEnumerator TimerEnumerator(int InTime, Action OnFinishedAction)
        {
            if (null == OnFinishedAction) yield break;
            yield return new WaitForSeconds(InTime);
            OnFinishedAction();
        }

        public static string ConvertTimeFormat(int InTime, TimeType InType = TimeType.hhmmss)
        {
            switch (InType)
            {
                case TimeType.hhmmss:
                    {
                        int hour = InTime / 3600;
                        int minute = InTime % 3600 / 60;
                        int second = InTime % 60;
                        return (hour > 9 ? "" : "0") + hour + (minute > 9 ? ":" : ":0") + minute + (second > 9 ? ":" : ":0") + second;
                    }

                case TimeType.mmss:
                    {
                        int minute = InTime % 3600 / 60;
                        int second = InTime % 60;
                        return (minute > 9 ? "" : "0") + minute + (second > 9 ? ":" : ":0") + second;
                    }

                case TimeType.ss:
                    return InTime > 9 ? InTime.ToString() : "0" + InTime;

                default:
                    return string.Empty;
            }
        }

        public class HourMeter
        {
            private MonoBehaviour mono;
            private IEnumerator coroutine;

            private int leftTime;
            
            private Action<int> timerAction;
            private Action<int, string> timerMultiAction;
            private Action finishAction;

            private TimeType type;
            
            public enum TimerState
            {
                Running,
                Pause,
                Stoped
            }
            private TimerState timerState;

            private float waitTime;
            private WaitForSeconds wait;
            
            private HourMeter(MonoBehaviour InMono, int InTime)
            {
                mono = InMono;
                leftTime = InTime;

                waitTime = 1;
                wait = new WaitForSeconds(waitTime);
            }

            public HourMeter(MonoBehaviour InMono, int InTime, Action InFinishAction)
                : this(InMono, InTime)
            {
                if (null == InFinishAction)
                {
                    timerState = TimerState.Stoped;
                    Debug.LogWarning("Nothing to do~");
                }
                else
                {
                    finishAction = InFinishAction;

                    coroutine = Timer();
                    mono.StartCoroutine(coroutine);
                }
            }

            private IEnumerator Timer()
            {
                while (leftTime > 0)
                {
                    yield return wait;
                    --leftTime;
                }
                leftTime = 0;

                timerState = TimerState.Stoped;
                finishAction();
            }

            public HourMeter(MonoBehaviour InMono, int InTime, Action<int> InTimer,
                Action InFinishAction)
                : this(InMono, InTime)
            {
                if (null == InTimer && null == InFinishAction)
                {
                    timerState = TimerState.Stoped;
                    Debug.LogWarning("Nothing to do~");
                }
                else
                {
                    timerAction = InTimer;
                    finishAction = InFinishAction;

                    coroutine = SingleTimer();
                    mono.StartCoroutine(coroutine);

                    timerState = TimerState.Running;
                }
            }

            private IEnumerator SingleTimer()
            {
                while (leftTime > 0)
                {
                    timerAction(leftTime);
                    yield return wait;
                    --leftTime;
                }

                timerState = TimerState.Stoped;
                leftTime = 0;
                timerAction(0);
                finishAction();
            }

            public HourMeter(MonoBehaviour InMono, int InTime, TimeType InType,
                Action<int, string> InMultiTimer,
                Action InFinishAction)
                : this(InMono, InTime)
            {
                if (null == InMultiTimer && null == InFinishAction)
                {
                    timerState = TimerState.Stoped;
                    Debug.LogWarning("Nothing to do~");
                }
                else
                {
                    type = InType;
                    timerMultiAction = InMultiTimer;
                    finishAction = InFinishAction;

                    timerState = TimerState.Running;
                    coroutine = MutiTimer();
                    mono.StartCoroutine(coroutine);
                }
            }

            private IEnumerator MutiTimer()
            {
                int hour, minute, second;
                switch (type)
                {
                    case TimeType.mmss:
                            while (leftTime > 0)
                            {
                                minute = leftTime % 3600 / 60;
                                second = leftTime % 60;
                                timerMultiAction(leftTime, (minute > 9 ? "" : "0") + minute + (second > 9 ? ":" : ":0") + second);
                                yield return wait;
                                --leftTime;
                            }
                            
                            timerMultiAction(0, "00:00");
                        break;

                    case TimeType.hhmmss:
                            while (leftTime > 0)
                            {
                                hour = leftTime / 3600;
                                minute = leftTime % 3600 / 60;
                                second = leftTime % 60;
                                timerMultiAction(leftTime, (hour > 9 ? "" : "0") + hour + (minute > 9 ? ":" : ":0") + minute + (second > 9 ? ":" : ":0") + second);
                                yield return wait;
                                --leftTime;
                            }
                            timerMultiAction(0, "00");
                        break;

                    case TimeType.ss:
                        while (leftTime > 0)
                        {
                            timerMultiAction(leftTime, leftTime > 9 ? leftTime.ToString() : "0" + leftTime);
                            yield return wait;
                            --leftTime;
                        }

                        timerMultiAction(0, "00:00:00");
                        break;
                }

                timerState = TimerState.Stoped;
                finishAction();
            }

            public void ResetTime(int InTime)
            {
                if (timerState != TimerState.Stoped)
                {
                    leftTime = InTime;

                    if (null != timerAction) timerAction.Invoke(leftTime);
                    if (null != timerMultiAction) timerMultiAction.Invoke(leftTime, ConvertTimeFormat(leftTime, type));
                }
                else
                {
                    Debug.LogWarning("The timer has stopped.");
                }
            }

            public int AddTime(int InTime)
            {
                if (timerState != TimerState.Stoped)
                {
                    leftTime += InTime;

                    if (null != timerAction) timerAction.Invoke(leftTime);
                    if (null != timerMultiAction) timerMultiAction.Invoke(leftTime, ConvertTimeFormat(leftTime, type));

                    return leftTime;
                }
                else
                {
                    Debug.LogWarning("The timer has stopped.");
                    return InTime;
                }
            }

            public int SubTime(int InTime)
            {
                if (timerState != TimerState.Stoped)
                {
                    leftTime -= InTime;

                    if (null != timerAction) timerAction.Invoke(leftTime);
                    if (null != timerMultiAction) timerMultiAction.Invoke(leftTime, ConvertTimeFormat(leftTime, type));

                    return leftTime;
                }
                else
                {
                    Debug.LogWarning("The timer has stopped.");
                    return InTime;
                }
            }

            public int Pause()
            {
                if (timerState == TimerState.Running)
                {
                    timerState = TimerState.Pause;
                    
                    mono.StopCoroutine(coroutine);

                    //When you stop a coroutine it will be stoped after the current frame is completed.
                    //yield return new WaitForSeconds(time) will be executed once in update method.
                    //so you need to add time to the left time.
                    ++leftTime;
                }

                return leftTime;
            }

            public void Resume()
            {
                if (timerState == TimerState.Pause)
                {
                    timerState = TimerState.Running;
                    mono.StartCoroutine(coroutine);
                }
            }

            public void Accelerate(int InTimes)
            {
                waitTime /= InTimes;
                wait = new WaitForSeconds(waitTime);
            }

            public void Decelerate(int InTimes)
            {
                waitTime *= InTimes;
                wait = new WaitForSeconds(waitTime);
            }

            public void SetTimerSpeed(int InSpeed)
            {
                waitTime = InSpeed;
                wait = new WaitForSeconds(waitTime);
            }

            public int Stop()
            {
                if(timerState == TimerState.Running) mono.StopCoroutine(coroutine);
               
                timerState = TimerState.Stoped;
                coroutine = null;

                return leftTime;
            }
        }
    }
}
