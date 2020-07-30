using Rewired;
using RewiredConsts;
using Player = Rewired.Player;

namespace EZWork
{
    public class EZInput : EZSingletonStatic<EZInput>
    {
        public enum UINavigationState
        {
            UI,
            GamePlay,
            None
        }
        protected EZInput(){ }
        
        private Player player;
        public Player Player
        {
            get
            {
                if (player == null) {
                    player =  player = ReInput.players.GetPlayer(RewiredConsts.Player.Player0);
                }
                return player;
            }
        }

        private ControllerMapLayoutManager layoutManager;
        public ControllerMapLayoutManager LayoutManager
        {
            get
            {
                if (layoutManager == null) {
                    layoutManager = Player.controllers.maps.layoutManager;
                }
                return layoutManager;
            }
        }
        
        // Gameplay RuleSet
        private ControllerMapLayoutManager.RuleSet ruleSetGameplay;
        public ControllerMapLayoutManager.RuleSet RuleSetGameplay
        {
            get
            {
                if (ruleSetGameplay == null) {
                    foreach (var tmpRu in LayoutManager.ruleSets) {
                        if (tmpRu.tag.Equals(nameof(LayoutManagerRuleSet
                            .LayoutManager_Gameplay))) {
                            ruleSetGameplay = tmpRu;
                        }
                    }
                }
                return ruleSetGameplay;
            }
        }
        
        // UI RuleSet
        private ControllerMapLayoutManager.RuleSet ruleSetUI;
        public ControllerMapLayoutManager.RuleSet RuleSetUI
        {
            get
            {
                if (ruleSetUI == null) {
                    foreach (var tmpRu in LayoutManager.ruleSets) {
                        if (tmpRu.tag.Equals(nameof(LayoutManagerRuleSet
                            .LayoutManager_UI))) {
                            ruleSetUI = tmpRu;
                        }
                    }
                }
                return ruleSetUI;
            }
        }

        /// <summary>
        /// 当前是否正在控制UI
        /// </summary>
        public UINavigationState mUINavigationState = UINavigationState.None;
        public UINavigationState InputState
        {
            get => mUINavigationState;
            set => mUINavigationState = value;
        }

        /// <summary>
        /// 按钮按下
        /// </summary>
        public bool GetButtonDown(EZKeyCode keyCode)
        {
            if (mUINavigationState == UINavigationState.GamePlay) {
                switch (keyCode) {
                    case EZKeyCode.Left:
                        return Player.GetButtonDown(RewiredConsts.Action.Left);
                    case EZKeyCode.Right:
                        return Player.GetButtonDown(RewiredConsts.Action.Right);
                    case EZKeyCode.Up:
                        return Player.GetButtonDown(RewiredConsts.Action.Up);
                    case EZKeyCode.Down:
                        return Player.GetButtonDown(RewiredConsts.Action.Down);
                    case EZKeyCode.PickUp:
                        return Player.GetButtonDown(RewiredConsts.Action.PickUp);
                    case EZKeyCode.ChangeMoveState:
                        return Player.GetButtonDown(RewiredConsts.Action.ChangeMoveState);
                    case EZKeyCode.Submit:
                        return Player.GetButtonDown(RewiredConsts.Action.Submit);
                    case EZKeyCode.Cancel:
                        return Player.GetButtonDown(RewiredConsts.Action.Cancel);
                    case EZKeyCode.OpenBag:
                        return Player.GetButtonDown(RewiredConsts.Action.OpenBag);
                    case EZKeyCode.OpenMission:
                        return Player.GetButtonDown(RewiredConsts.Action.OpenMission);
                    case EZKeyCode.OpenSetting:
                        return Player.GetButtonDown(RewiredConsts.Action.OpenSetting);
                    case EZKeyCode.OpenHelp:
                        return Player.GetButtonDown(RewiredConsts.Action.OpenHelp);
                    case EZKeyCode.StopTime:
                        return Player.GetButtonDown(RewiredConsts.Action.StopTime);
                    case EZKeyCode.Space:
                        return Player.GetButtonDown(RewiredConsts.Action.Space);
                    case EZKeyCode.Map:
                        return Player.GetButtonDown(RewiredConsts.Action.Map);
                    case EZKeyCode.SkipProcess:
                        return Player.GetButtonDown(RewiredConsts.Action.SkipProcess);
					case EZKeyCode.Survey:
                        return Player.GetButtonDown(RewiredConsts.Action.Survey);
                }
            }
            else if(mUINavigationState == UINavigationState.UI){
                switch (keyCode) {
                    case EZKeyCode.UILeft:
                        return Player.GetButtonDown(RewiredConsts.Action.UILeft);
                    case EZKeyCode.UIRight:
                        return Player.GetButtonDown(RewiredConsts.Action.UIRight);
                    case EZKeyCode.UIUp:
                        return Player.GetButtonDown(RewiredConsts.Action.UIUp);
                    case EZKeyCode.UIDown:
                        return Player.GetButtonDown(RewiredConsts.Action.UIDown);
                    case EZKeyCode.UISubmit:
                         return Player.GetButtonDown(RewiredConsts.Action.UISubmit);
                    case EZKeyCode.UICancel:
                         return Player.GetButtonDown(RewiredConsts.Action.UICancel);
                    case EZKeyCode.UIOpenBag:
                        return Player.GetButtonDown(RewiredConsts.Action.UIOpenBag);
                    case EZKeyCode.UIOpenMission:
                        return Player.GetButtonDown(RewiredConsts.Action.UIOpenMission);
                    case EZKeyCode.UIDialogSkip:
                        return Player.GetButtonDown(RewiredConsts.Action.UIDialogSkip);
					case EZKeyCode.UISpace:
						return Player.GetButtonDown(RewiredConsts.Action.UISpace);
				}
			}

           
            return false;
        }
        
        /// <summary>
        /// 按钮被持续按住
        /// </summary>
        public bool GetButton(EZKeyCode keyCode)
        {
            if (mUINavigationState == UINavigationState.GamePlay) {
                switch (keyCode) {
                    case EZKeyCode.Left:
                        return Player.GetButton(RewiredConsts.Action.Left);
                    case EZKeyCode.Right:
                        return Player.GetButton(RewiredConsts.Action.Right);
                    case EZKeyCode.Up:
                        return Player.GetButton(RewiredConsts.Action.Up);
                    case EZKeyCode.Down:
                        return Player.GetButton(RewiredConsts.Action.Down);
                    case EZKeyCode.ChangeMoveState:
                        return Player.GetButton(RewiredConsts.Action.ChangeMoveState);
                }
            }
            else if(mUINavigationState == UINavigationState.UI){
                switch (keyCode) {
                    case EZKeyCode.UILeft:
                        return Player.GetButton(RewiredConsts.Action.UILeft);
                    case EZKeyCode.UIRight:
                        return Player.GetButton(RewiredConsts.Action.UIRight);
                    case EZKeyCode.UIUp:
                        return Player.GetButton(RewiredConsts.Action.UIUp);
                    case EZKeyCode.UIDown:
                        return Player.GetButton(RewiredConsts.Action.UIDown);
                    case EZKeyCode.UIDialogSkip:
                        return Player.GetButton(RewiredConsts.Action.UIDialogSkip);
                }
            }

            
            return false;
        }
        
        /// <summary>
        /// 按钮抬起
        /// </summary>
        public bool GetButtonUP(EZKeyCode keyCode)
        {
            if (mUINavigationState == UINavigationState.GamePlay) {
                switch (keyCode) {
                    case EZKeyCode.Left:
                        return Player.GetButtonUp(RewiredConsts.Action.Left);
                    case EZKeyCode.Right:
                        return Player.GetButtonUp(RewiredConsts.Action.Right);
                    case EZKeyCode.Up:
                        return Player.GetButtonUp(RewiredConsts.Action.Up);
                    case EZKeyCode.Down:
                        return Player.GetButtonUp(RewiredConsts.Action.Down);
                    case EZKeyCode.PickUp:
                        return Player.GetButtonUp(RewiredConsts.Action.PickUp);
                    case EZKeyCode.ChangeMoveState:
                        return Player.GetButtonUp(RewiredConsts.Action.ChangeMoveState);
                    case EZKeyCode.Submit:
                         return Player.GetButtonUp(RewiredConsts.Action.Submit);
                    case EZKeyCode.Cancel:
                         return Player.GetButtonUp(RewiredConsts.Action.Cancel);
                }
            }
            else if(mUINavigationState == UINavigationState.UI){
                switch (keyCode) {
                    case EZKeyCode.UILeft:
                        return Player.GetButtonUp(RewiredConsts.Action.UILeft);
                    case EZKeyCode.UIRight:
                        return Player.GetButtonUp(RewiredConsts.Action.UIRight);
                    case EZKeyCode.UIUp:
                        return Player.GetButtonUp(RewiredConsts.Action.UIUp);
                    case EZKeyCode.UIDown:
                        return Player.GetButtonUp(RewiredConsts.Action.UIDown);
                    case EZKeyCode.UISubmit:
                        return Player.GetButtonUp(RewiredConsts.Action.UISubmit);
                    case EZKeyCode.UICancel:
                         return Player.GetButtonUp(RewiredConsts.Action.UICancel);
                    case EZKeyCode.UIDialogSkip:
                        return Player.GetButtonUp(RewiredConsts.Action.UIDialogSkip);
                }
            }

            
            return false;
        }

        /// <summary>
        /// 切换到 UI 控制
        /// </summary>
        public void SwitchToUI()
        {
            // 避免重复调用
            if (mUINavigationState == UINavigationState.UI)
                return;
            mUINavigationState = UINavigationState.UI;
            
            RuleSetGameplay.enabled = false;
            RuleSetUI.enabled = true;
            LayoutManager.Apply();
        }

        /// <summary>
        /// 切换到 Gameplay 控制
        /// </summary>
        public void SwitchToGameplay()
        {
            // 避免重复调用
            if (mUINavigationState == UINavigationState.GamePlay)
                return;
            mUINavigationState = UINavigationState.GamePlay;
            
            RuleSetGameplay.enabled = true;
            RuleSetUI.enabled = false;
            LayoutManager.Apply();
        }
        
        /// <summary>
        /// 不进行任何控制
        /// </summary>
        public void SwitchToNone()
        {
            mUINavigationState = UINavigationState.None;
            RuleSetGameplay.enabled = false;
            RuleSetUI.enabled = false;
            LayoutManager.Apply();
        }
    }

    /// <summary>
    /// 操作动作种类
    /// </summary>
    public enum EZKeyCode
    {
        Left,
        Right,
        Up,
        Down,
        Submit,
        Cancel,
        PickUp,
        ChangeMoveState,
        OpenBag,
        OpenMission,
        OpenSetting,
        OpenHelp,
        StopTime,
        Space,
        Map,
        SkipProcess,
		Survey,
        UILeft,
        UIRight,
        UIUp,
        UIDown,
        UISubmit,
        UICancel,
        UIOpenBag,
        UIOpenMission,
        UIDialogSkip,
		UISpace
    }

}

