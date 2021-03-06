﻿using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Kingmaker;
using Kingmaker.PubSubSystem;
using Kingmaker.UI;
using Kingmaker.UI.LevelUp;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static KingmakerPortraitManager.Main;
using static KingmakerPortraitManager.Utility.StatusWrapper;

namespace KingmakerPortraitManager.UI
{
    public class PortraitTagSelector : MonoBehaviour, ISequentialSelectorChanged, IGlobalSubscriber
    {
        public static string[] portraitIDsUI;
        public static string[] portraitIDsFilter;
        //?        private int portraitIDsTagCount;

        [SerializeField]
        [UsedImplicitly]
        internal CharBSequentialSelector m_Tags;

        [SerializeField]
        [UsedImplicitly]
        private TextMeshProUGUI m_Label;

        private bool m_enabled;
        private bool m_isInit;
        private RectTransform tagSelectorTransform;
        private CanvasGroup tagSelectorCanvasGroup;
        private Dictionary<string, TagData> tagsData;
        private Dictionary<string, TagData> allPortraitsData;
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
            //TODO: remake to instantiate from Constitution instead
            GameObject preFab = uICommon?.transform.Find("CharacterBuild/Body/Content/RaceRightSide/Constitution")?.gameObject;
            TextMeshProUGUI labelPreFab = uICommon?.transform.Find("CharacterBuild/Body/Content/RaceRightSide/Constitution/LabelPlace/Label/LabelText").gameObject.GetComponent<TextMeshProUGUI>();
            if (!parentElement || !preFab || !labelPreFab)
            {
                return null;
            }
            GameObject portraitTagSelectorCanvas = Instantiate(preFab, parentElement.transform, false);
            portraitTagSelectorCanvas.AddComponent<CanvasGroup>();
            portraitTagSelectorCanvas.transform.SetSiblingIndex(0);
            //Initialize transform
            Mod.Debug("Initialize transform");
            portraitTagSelectorCanvas.name = "PortraitTagSelector";
            RectTransform rectPortraitTagSelector = (RectTransform)portraitTagSelectorCanvas.transform;
            rectPortraitTagSelector.anchorMin = new Vector2(0.68f, 1.0f);
            rectPortraitTagSelector.anchorMax = new Vector2(0.68f, 1.0f);
            rectPortraitTagSelector.pivot = new Vector2(0.5f, 0.5f);
            //           rectPortraitTagSelector.localPosition = preFabPos;
            rectPortraitTagSelector.localPosition -= rectPortraitTagSelector.forward;
            rectPortraitTagSelector.rotation = Quaternion.identity;
            //TODO Initialize children positions (copy from prefab?)
            TextMeshProUGUI _Label = portraitTagSelectorCanvas.transform.Find("LabelPlace/Label/LabelText").gameObject.GetComponent<TextMeshProUGUI>();
            RectTransform rectLabel = (RectTransform)_Label.transform;
            rectLabel.anchoredPosition = rectLabel.parent.position;
            rectLabel.anchorMin = new Vector2(0.0F, 1.0F);
            rectLabel.anchorMax = new Vector2(0.0F, 1.0F);
            rectLabel.pivot = new Vector2(0.5F, 0.5F);
            rectLabel.localPosition += rectLabel.forward;
            UIHelpers.CopyTextMeshProUGUI(ref _Label, labelPreFab);
            CharBSequentialSelector charBSequentialSelector = portraitTagSelectorCanvas.transform.Find("SequentialSelector").gameObject.GetComponent<CharBSequentialSelector>();
            //Buttons
            Button BackButton = portraitTagSelectorCanvas.transform.Find("SequentialSelector/SequentialSelector/ButtonPlaceBackSelector").gameObject.GetComponent<Button>();
            charBSequentialSelector.m_BackButton = BackButton;
            Button NextButton = portraitTagSelectorCanvas.transform.Find("SequentialSelector/SequentialSelector/ButtonPlaceNextSelector").gameObject.GetComponent<Button>();
            charBSequentialSelector.m_NextButton = NextButton;
            TextMeshProUGUI Counter = portraitTagSelectorCanvas.transform.Find("SequentialSelector/SequentialSelector/GameObject/Counter").gameObject.GetComponent<TextMeshProUGUI>();
            charBSequentialSelector.m_Counter = Counter;
            charBSequentialSelector.m_Label = _Label;
            Mod.Debug("Initialized");
            return portraitTagSelectorCanvas.AddComponent<PortraitTagSelector>();
        }

        //Initializing and filling data
        public void Awake()
        {
            this.FillData();
            tagSelectorTransform = (RectTransform)transform.Find("PortraitTagSelector");
            tagSelectorCanvasGroup = gameObject.GetComponent<CanvasGroup>();
            Mod.Debug("canvasgroup awake");
            Mod.Debug(tagSelectorCanvasGroup.ToString());
            tagSelectorCanvasGroup.alpha = 0f;
            //TODO - add button
        }

        //Initializing
		public void Init()		
		{
            if (this.m_isInit)
            {
                return;
            }
            EventBus.Subscribe(this);
            this.m_Tags = this.gameObject.transform.Find("SequentialSelector").gameObject.GetComponent<CharBSequentialSelector>();
            this.m_Label = this.gameObject.transform.Find("SequentialSelector").gameObject.GetComponent<CharBSequentialSelector>().m_Label;
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
                    tagSelectorCanvasGroup.DOFade(1f, 0.5f).SetUpdate(true);
                    tagSelectorTransform.DOAnchorPosY(0f, 0.5f, false).SetUpdate(true);
                }
            }
            else
            {
                if (m_enabled)
                {
                    m_enabled = false;
                    tagSelectorCanvasGroup.DOFade(0f, 0.5f).SetUpdate(true);
                    tagSelectorTransform.DOAnchorPosY(tagSelectorTransform.rect.height, 0.5f, false).SetUpdate(true);

                }
            }
        }

        public void OnEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            EventBus.Subscribe(this);
            if (canSelectPortrait())
            {
                this.enabled = true;
                this.gameObject.SetActive(true);
            }
            else
            {
                this.enabled = false;
            }
            this.Update();
        }

        public void OnDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            tagSelectorCanvasGroup.DOKill();
            tagSelectorTransform.DOKill();
            EventBus.Unsubscribe(this);
            if (portraitIDsUI != null)
                portraitIDsUI = null;
        }


        public void OnDestroy()
        {
            EventBus.Unsubscribe(this);
            if (portraitIDsUI != null)
                portraitIDsUI = null;
            if (portraitIDsFilter != null)
                portraitIDsFilter = null;
        }

        //Filling up data and initializing
		public void FillData()
        {
            base.gameObject.SetActive(true);
            this.Init();

            this.tagsData = Tags.LoadTagsData(false);
            this.allPortraitsData = new Dictionary<string, TagData>();
            this.allPortraitsData = Helpers.LoadAllPortraitsTags(tagsData, true);
            List<string> list = Tags.AllTagListUI(tagsData, allPortraitsData);
            this.m_Tags.Init(list, Local["GUI_TagSelectorItem_Label"]);
            this.m_Tags.SetIndex(0);
            this.m_Tags.FillData();
#if (DEBUG)
            Mod.Log("FillData");
            Mod.Log(Game.Instance.UI.CharacterBuildController.CurrentPhase.GetValueOrDefault());
#endif
        }

        //Handling selected element change
		public void HandleChooseElement(CharBSequentialSelector selector, int index)
        {
            if (!base.gameObject.activeInHierarchy)
            {
                return;
            }
            if (selector == this.m_Tags)
            {
#if (DEBUG)
                Mod.Debug(MethodBase.GetCurrentMethod());
#endif
                portraitIDsUI = Tags.filterPortraitsUI(this.m_Tags.m_CurrentItemName.text,this.allPortraitsData);
                if (Game.Instance.UI.CharacterBuildController.Portrait.IsUnlocked)
                {
                    Game.Instance?.UI.CharacterBuildController.Portrait.PortraitSelector.HandleClickUpload(false);
                }
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
