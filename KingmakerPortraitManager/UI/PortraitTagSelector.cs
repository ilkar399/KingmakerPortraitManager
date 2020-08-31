using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Kingmaker;
using Kingmaker.Controllers;
using Kingmaker.PubSubSystem;
using Kingmaker.UI;
using Kingmaker.UI.Constructor;
using Kingmaker.UI.LevelUp;
using Kingmaker.UI.SettingsUI;
using Kingmaker.UnitLogic.Class.LevelUp;
using ModMaker.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static KingmakerPortraitManager.Main;
using static KingmakerPortraitManager.Utility.StatusWrapper;
using KingmakerPortraitManager.Menu;
using System.Runtime.CompilerServices;
using TinyJson;
using System.Reflection.Emit;

namespace KingmakerPortraitManager.UI
{
    public class PortraitTagSelector : MonoBehaviour, ISequentialSelectorChanged, IGlobalSubscriber
    {
        public static string[] portraitIDsUI;
        //?        private int portraitIDsTagCount;

        [SerializeField]
        [UsedImplicitly]
        private CharBSequentialSelector m_Tags;

        [SerializeField]
        [UsedImplicitly]
        private TextMeshProUGUI m_Label;

        private bool m_enabled;
        private bool m_isInit;
 //       private CanvasGroup _canvasGroup;
        private RectTransform tagSelectorTransform;
        private CanvasGroup tagSelectorCanvasGroup;


        private CharacterBuildController m_CharacterBuildController
        {
            get
            {
                return Game.Instance.UI.CharacterBuildController;
            }
        }

        //Creating objects
        public static PortraitTagSelector CreateObject()
        {
            UICommon uICommon = Game.Instance.UI.Common;
            GameObject parentElement = uICommon?.transform.Find("CharacterBuild/Body/Content")?.gameObject;
            GameObject preFab = uICommon?.transform.Find("CharacterBuild/Body/Content/RaceRightSide/Constitution/SequentialSelector")?.gameObject;
            if (!parentElement || !preFab)
            {
                return null;
            }
 //           Vector3 preFabPos = preFab.transform.localPosition;
            //GameObject portraitTagSelector = new GameObject("PortraitTagSelector", typeof(RectTransform),typeof(PortraitTagSelector));
            GameObject portraitTagSelector = Instantiate(preFab, parentElement.transform, false);
            //portraitTagSelector.transform.SetParent(parentElement.transform);
            portraitTagSelector.transform.SetSiblingIndex(0);
            //Initialize transform
            Mod.Debug("Initialize transform");
            portraitTagSelector.name = "PortraitTagSelector";
            RectTransform rectPortraitTagSelector = (RectTransform)portraitTagSelector.transform;
            rectPortraitTagSelector.anchorMin = new Vector2(0f, 0f);
            rectPortraitTagSelector.anchorMax = new Vector2(1f, 1f);
            rectPortraitTagSelector.pivot = new Vector2(1f, 1f);
            //           rectPortraitTagSelector.localPosition = preFabPos;
            rectPortraitTagSelector.localPosition -= rectPortraitTagSelector.forward;
            rectPortraitTagSelector.rotation = Quaternion.identity;
            //TODO Initialize children positions (copy from prefab?)
            CharBSequentialSelector charBSequentialSelector = portraitTagSelector.gameObject.GetComponent<CharBSequentialSelector>();
            //Buttons
            Button BackButton = portraitTagSelector.transform.Find("SequentialSelector/ButtonPlaceBackSelector").gameObject.GetComponent<Button>();
            charBSequentialSelector.m_BackButton = BackButton;
            RectTransform rectbb = (RectTransform)BackButton.transform;
            //TODO
            rectbb.anchoredPosition = rectbb.parent.position;
            rectbb.anchorMin = new Vector2(0.0F, 0.0F);
            rectbb.anchorMax = new Vector2(0.3F, 1.0F);
            Button NextButton = portraitTagSelector.transform.Find("SequentialSelector/ButtonPlaceNextSelector").gameObject.GetComponent<Button>();
            charBSequentialSelector.m_NextButton = NextButton;
            RectTransform rectnb = (RectTransform)NextButton.transform;
            //TODO
            rectnb.anchoredPosition = rectbb.parent.position;
            rectnb.anchorMin = new Vector2(0.7F, 0.0F);
            rectnb.anchorMax = new Vector2(1.1F, 1.0F);
            TextMeshProUGUI Counter = portraitTagSelector.transform.Find("SequentialSelector/GameObject/Counter").gameObject.GetComponent<TextMeshProUGUI>();
            charBSequentialSelector.m_Counter = Counter;
            RectTransform rectCounter = (RectTransform)Counter.transform;
            rectCounter.anchoredPosition = rectCounter.parent.position;
            rectCounter.anchorMin = new Vector2(0.5F, 1.0F);
            rectCounter.anchorMax = new Vector2(0.5F, 1.0F);
            GameObject labelObject = new GameObject("PortraitTagSelectorLabel", typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(rectPortraitTagSelector, false);
            TextMeshProUGUI _Label = labelObject.GetComponent<TextMeshProUGUI>();
            charBSequentialSelector.m_Label = _Label;
            RectTransform rectLabel = (RectTransform)_Label.transform;
            rectLabel.anchoredPosition = rectCounter.parent.position;
            rectLabel.anchorMin = new Vector2(0.5F, 0.0F);
            rectLabel.anchorMax = new Vector2(0.5F, 0.0F);
            return portraitTagSelector.AddComponent<PortraitTagSelector>();
        }

        //Initializing and filling data
        public void Awake()
        {
            this.FillData();
            tagSelectorTransform = (RectTransform)transform.Find("PortraitTagSelector");
            //Setup(this.gameObject);
            //TODO - canvasGroup
            //TODO - add button
        }

        //Setup UI element positions. TODO
        public void Setup(GameObject portraitTagSelector)
        {
            CharBSequentialSelector charBSequentialSelector = portraitTagSelector.gameObject.GetComponent<CharBSequentialSelector>();
            RectTransform rectbb = (RectTransform)charBSequentialSelector.m_BackButton.transform;
            RectTransform rectnb = (RectTransform)charBSequentialSelector.m_NextButton.transform;
            RectTransform rectCounter = (RectTransform)charBSequentialSelector.m_Counter.transform;
            RectTransform rectLabel = (RectTransform)m_Label.transform;
        }

        //Initializing
		public void Init()		
		{
            if (this.m_isInit)
            {
                return;
            }
            EventBus.Subscribe(this);
            this.m_Tags = this.gameObject.GetComponent<CharBSequentialSelector>();
            this.m_Label = this.gameObject.transform.Find("PortraitTagSelectorLabel").gameObject.GetComponent<TextMeshProUGUI>();
            this.m_Label.text = Local["GUI_TagSelector_Label"];
            this.m_isInit = true;
		}

        //Cleaning up 
		public void Clear()
        {
            if (this.m_isInit && this.m_Tags)
            {
                this.m_Tags.CurrentElementIndex = -1;
            }
            EventBus.Unsubscribe(this);
            if (portraitIDsUI != null)
                portraitIDsUI = null;
        }

        //Update UI element status if needed. TODO - change to canvas hiding
        public void Update()
        {
            if (canSelectPortrait())
            {
                if (!m_enabled)
                {
                    m_enabled = true;
                    this.FillData();
                    this.gameObject.SetActive(true);
                }
            }
            else
            {
                Mod.Log("Update");
                Mod.Log((Game.Instance.UI.CharacterBuildController.CurrentPhase.GetValueOrDefault()).ToString());
                if (m_enabled)
                {
                    m_enabled = false;
                    /*this.gameObject.SetActive(false);*/
                }
            }
        }

        public void OnEnable()
        {
            EventBus.Subscribe(this);
            Mod.Debug(MethodBase.GetCurrentMethod());
        }

        public void OnDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            Mod.Debug((new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
            Mod.Debug(Game.Instance.UI.CharacterBuildController.CurrentPhase.GetValueOrDefault());
            EventBus.Unsubscribe(this);
            if (portraitIDsUI != null)
                portraitIDsUI = null;
        }


        public void OnDestroy()
        {
            EventBus.Unsubscribe(this);
            if (portraitIDsUI != null)
                portraitIDsUI = null;
        }

        //Filling up data and initializing
		public void FillData()
        {
            base.gameObject.SetActive(true);
            this.Init();
            List<string> list = Tags.AllTagListUI();
            this.m_Tags.Init(list, Local["GUI_TagSelectorItem_Label"]);
            this.m_Tags.SetIndex(0);
            this.m_Tags.FillData();
            Mod.Log("FillData");
            Mod.Log(Game.Instance.UI.CharacterBuildController.CurrentPhase.GetValueOrDefault());
            if (canSelectPortrait())
            {
                this.enabled = true;
                this.m_enabled = true;
                this.gameObject.SetActive(true);
            }
            else
            {
                this.enabled = false;
                this.m_enabled = false;
            }
        }

        //Handling selected element change
		public void HandleChooseElement(CharBSequentialSelector selector, int index)
        {
            if (selector = this.m_Tags)
            {
                portraitIDsUI = Tags.filterPortraitsUI(this.m_Tags.m_CurrentItemName.text);
                //TODO: invoke increasing tag by index to (this.CurrentTagIndex);
            }
        }

        public int CurrentTagIndex
        {
            get
            {
                return this.m_Tags.CurrentElementIndex;
            }
        }

		public bool IsSelected()
        {
			return !(Game.Instance.UI.CharacterBuildController.CurrentPhase.GetValueOrDefault() == Kingmaker.UI.LevelUp.Phase.CharBPhase.Type.Portrait);
        }

	}
}
