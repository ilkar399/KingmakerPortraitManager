using JetBrains.Annotations;
using Kingmaker.EntitySystem.Entities;
using Kingmaker;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using UnityEngine;
using Newtonsoft.Json.Linq;
using ModMaker;
using ModMaker.Utility;
using System;
using System.Reflection;
using KingmakerPortraitManager.UI;
using static KingmakerPortraitManager.Main;
using Kingmaker.UnitLogic.Class.LevelUp.Actions;

namespace KingmakerPortraitManager.Controllers
{
    public class UIController: IModEventHandler, ILevelUpInitiateUIHandler, ILevelUpCompleteUIHandler
    {
        public PortraitTagSelector PortraitTagSelector { get; private set; }
        public int Priority => 500;

        public void Attach()
        {
            if (!PortraitTagSelector)
            {
                PortraitTagSelector = PortraitTagSelector.CreateObject();
            }
        }

        public void Detach()
        {
            if (PortraitTagSelector)
            {
                try
                {
                    PortraitTagSelector.Clear();
                    PortraitTagSelector.SafeDestroy();
                }
                finally
                {
                    PortraitTagSelector = null;
                }
            }
        }

#if (DEBUG)
        public void Clear()
        {
            Transform portraitTagSelector;
            while (portraitTagSelector = Game.Instance.UI.Common.transform.Find("CharacterBuild/Body/Content/PortraitTagSelector"))
            {
                portraitTagSelector.SafeDestroy();
                PortraitTagSelector  = null;
            }
        }
#endif

        public void Update()
        {
            Detach();
            Attach();
        }

        #region Event Handlers

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            Mod.Core.UI = this;
            Attach();

            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            EventBus.Unsubscribe(this);
            
            Detach();
            Mod.Core.UI = null;
        }

        public void HandleLevelUpStart(UnitDescriptor unit, [CanBeNull] JToken unitJson = null, Action onSuccess = null, LevelUpState.CharBuildMode mode = LevelUpState.CharBuildMode.LevelUp)
        {
            Attach();
        }

        public void HandleLevelUpComplete(UnitEntityData unit, bool isChargen)
        {
            Detach();
        }
        #endregion
    }


}
