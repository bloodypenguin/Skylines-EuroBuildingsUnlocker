using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ColossalFramework.DataBinding;
using ColossalFramework.UI;
using UnityEngine;

namespace EuroBuildingsUnlocker
{
    public class OptionsManager : MonoBehaviour
    {
        [Flags]
        public enum ModOptions : long
        {
            None = 0,
            LoadNativeGrowables = 1,
            LoadNonNativeGrowables = 2,
            OverrideNativeTrafficLights = 4,
            AddCustomAssetsGameObject = 8
        }

        GameObject m_optionsButtonGo;
        GameObject m_optionsPanel;
        GameObject m_optionsList;
        GameObject m_checkboxTemplate;


        UICheckBox m_loadNativeGrowablesCheckBox = null;
        UICheckBox m_loadNonNativeGrowablesCheckBox = null;
        UICheckBox m_overrideNativeTrafficLightsCheckBox = null;
        UICheckBox m_addCustomAssetsGameObjectCheckBox = null;

        bool m_initialized;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void Start()
        {
#if DEBUG
            //foreach (var item in GameObject.FindObjectsOfType<GameObject>())
            //{
            //	if (item.transform.parent == null)
            //		Initializer.PrintGameObjects(item, "MainMenuScene_110b.txt");
            //}
#endif

            GameObject contentManager = GameObject.Find("(Library) ContentManagerPanel");
            if (contentManager == null)
                return;

            Transform mods = contentManager.transform.GetChild(0).FindChild("Mods");
            if (mods == null)
            {
                //Logger.LogInfo("Can't find mods");
                return;
            }

            UILabel modLabel = mods.GetComponentsInChildren<UILabel>().FirstOrDefault(l => l.text.Contains("BlooadyPenguin") || l.text.Contains("EuropeanBuildingsUnlocker"));
            if (modLabel == null)
            {
                //Logger.LogInfo("Can't find label");
                return;
            }

            GameObject mod = modLabel.transform.parent.gameObject;
            UIButton shareButton = mod.GetComponentsInChildren<UIButton>(true).FirstOrDefault(b => b.name == "Share");
            if (shareButton == null)
                return;

            //// Options Button
            //Transform shareButtonTransform = mod.transform.FindChild("Share");
            //if (shareButtonTransform == null)
            //{
            //	//Logger.LogInfo("Can't find share");
            //	return;
            //}

            //UIButton shareButton = shareButtonTransform.gameObject.GetComponent<UIButton>();
            this.m_optionsButtonGo = Instantiate<GameObject>(shareButton.gameObject);
            this.m_optionsButtonGo.name = "Options";
            UIButton optionsButton = mod.GetComponent<UIPanel>().AttachUIComponent(this.m_optionsButtonGo) as UIButton;
            this.m_optionsButtonGo.transform.localPosition = shareButton.transform.localPosition;

            optionsButton.isVisible = true;
            optionsButton.text = "Options";
            optionsButton.eventClick += OpenOptionsPanel;
            optionsButton.position += Vector3.right * (optionsButton.width * 1.1f);

            // Options Panel
            GameObject optionsPanel = GameObject.Find("(Library) OptionsPanel");
            this.m_optionsPanel = Instantiate<GameObject>(optionsPanel);
            this.m_optionsPanel.transform.SetParent(GameObject.Find("(Library) ContentManagerPanel").transform);
            GameObject.Destroy(this.m_optionsPanel.GetComponent<OptionsPanel>());

            m_checkboxTemplate = this.m_optionsPanel.GetComponentsInChildren<UICheckBox>().FirstOrDefault(c => c.name == "EdgeScrolling").gameObject;
            GameObject.Destroy(m_checkboxTemplate.GetComponent<BindProperty>());

            // clear panel but keep title
            GameObject caption = null;
            foreach (Transform transform in m_optionsPanel.transform)
            {
                if (transform.name == "Caption")
                    caption = transform.gameObject;
                else
                    GameObject.Destroy(transform.gameObject);
            }

            this.m_optionsPanel.GetComponent<UIPanel>().autoFitChildrenVertically = true;
            this.m_optionsPanel.GetComponent<UIPanel>().autoFitChildrenHorizontally = true;

            // set caption
            caption.transform.FindChild("Label").GetComponent<UILabel>().text = "EuropeanBuildingsUnlocker Options";

            // clear close event
            UIButton closeButton = caption.transform.FindChild("Close").GetComponent<UIButton>();
            GameObject.Destroy(closeButton.GetComponent<BindEvent>());
            closeButton.eventClick += CloseOptionsPanel;

            // set options list
            m_optionsList = Instantiate<GameObject>(mod.transform.parent.gameObject);
            for (int i = m_optionsList.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(m_optionsList.transform.GetChild(i).gameObject);
            }
            m_optionsList.transform.SetParent(this.m_optionsPanel.transform);
            m_optionsList.GetComponent<UIScrollablePanel>().AlignTo(this.m_optionsPanel.GetComponent<UIPanel>(), UIAlignAnchor.TopLeft);
            m_optionsList.GetComponent<UIScrollablePanel>().position += new Vector3(caption.transform.FindChild("Label").GetComponent<UILabel>().height, -caption.transform.FindChild("Label").GetComponent<UILabel>().height * 2f);

            // save button
            GameObject save = Instantiate<GameObject>(this.m_optionsButtonGo);
            save.transform.SetParent(this.m_optionsPanel.transform);

            UIButton saveButton = save.GetComponent<UIButton>();
            saveButton.isVisible = true;
            saveButton.eventClick += OnSave;
            saveButton.color = Color.green;
            saveButton.text = "Save";
            saveButton.AlignTo(m_optionsPanel.GetComponent<UIPanel>(), UIAlignAnchor.BottomRight);
            Vector3 cornerOffset = new Vector3(saveButton.width * -0.1f, saveButton.height * 0.3f);
            saveButton.position += cornerOffset;

            // add options
            m_loadNativeGrowablesCheckBox = AddOptionCheckbox("Load native vanilla growables", 0);
            m_loadNativeGrowablesCheckBox.eventCheckChanged += CheckGrowablesCheckBoxes;
            m_loadNonNativeGrowablesCheckBox = AddOptionCheckbox("Load non-native vanilla growables", 1);
            m_loadNonNativeGrowablesCheckBox.eventCheckChanged += CheckGrowablesCheckBoxes;
            m_overrideNativeTrafficLightsCheckBox = AddOptionCheckbox("Override native traffic lights", 2);
            m_addCustomAssetsGameObjectCheckBox = AddOptionCheckbox("Add 'Custom Assets Collection' GameObject (may affect loading time)", 3);

            LoadOptions();

            m_initialized = true;
        }

        private void CheckGrowablesCheckBoxes(UIComponent component, bool loadGrowables)
        {
            if (component == m_loadNativeGrowablesCheckBox && !loadGrowables)
            {
                if (!m_loadNonNativeGrowablesCheckBox.isChecked)
                {
                    m_loadNativeGrowablesCheckBox.isChecked = true;
                }
            }
            else if (component == m_loadNonNativeGrowablesCheckBox && !loadGrowables)
            {
                if (!m_loadNativeGrowablesCheckBox.isChecked)
                {
                    m_loadNonNativeGrowablesCheckBox.isChecked = true;
                }
            }
        }

        UICheckBox AddOptionCheckbox(string text, int zOrder = -1)
        {
            GameObject newCheckbox = Instantiate<GameObject>(m_checkboxTemplate);
            newCheckbox.transform.SetParent(m_optionsList.transform);

            UICheckBox checkBox = newCheckbox.GetComponent<UICheckBox>();
            checkBox.isChecked = false;
            checkBox.text = text;
            checkBox.isVisible = true;
            if (zOrder != -1)
                checkBox.zOrder = zOrder;

            return checkBox;
        }

        private void OpenOptionsPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            LoadOptions();
            this.m_optionsPanel.GetComponent<UIPanel>().isVisible = true;
            this.m_optionsPanel.GetComponent<UIPanel>().BringToFront();
        }

        private void CloseOptionsPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.m_optionsPanel.GetComponent<UIPanel>().isVisible = false;
        }

        private void OnSave(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.m_optionsPanel.GetComponent<UIPanel>().isVisible = false;
            if (!(m_loadNativeGrowablesCheckBox.isChecked || m_loadNonNativeGrowablesCheckBox.isChecked))
            {
                throw new Exception("European Buildings Unlocker  - at least one set of growables must be loaded!");
            }
            Options options = new Options();
            EuroBuildingsUnlocker.Options = ModOptions.None;
            if (this.m_loadNativeGrowablesCheckBox.isChecked)
            {
                options.loadNativeGrowables = true;
                EuroBuildingsUnlocker.Options |= ModOptions.LoadNativeGrowables;
            }
            if (this.m_loadNonNativeGrowablesCheckBox.isChecked)
            {
                options.loadNonNativeGrowables = true;
                EuroBuildingsUnlocker.Options |= ModOptions.LoadNonNativeGrowables;
            }
            if (this.m_overrideNativeTrafficLightsCheckBox.isChecked)
            {
                options.overrideNativeTraficLights = true;
                EuroBuildingsUnlocker.Options |= ModOptions.OverrideNativeTrafficLights;
            }
            if (this.m_addCustomAssetsGameObjectCheckBox.isChecked)
            {
                options.addCustomAssetsGameObject = true;
                EuroBuildingsUnlocker.Options |= ModOptions.AddCustomAssetsGameObject;
            }

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
                using (StreamWriter streamWriter = new StreamWriter("CSL-EuroBuildingsUnlocker.xml"))
                {
                    xmlSerializer.Serialize(streamWriter, options);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Unexpected " + e.GetType().Name + " saving options: " + e.Message + "\n" + e.StackTrace);
            }
        }

        public void LoadOptions()
        {
            EuroBuildingsUnlocker.Options = ModOptions.None;
            Options options = new Options();
            options.loadNativeGrowables = true;
            options.loadNonNativeGrowables = true;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Options));
                using (StreamReader streamReader = new StreamReader("CSL-EuroBuildingsUnlocker.xml"))
                {
                    options = (Options)xmlSerializer.Deserialize(streamReader);
                }
            }
            catch (FileNotFoundException)
            {
                // No options file yet
                if (this.m_loadNativeGrowablesCheckBox != null)
                {
                    //set defaults
                    EuroBuildingsUnlocker.Options = ModOptions.LoadNonNativeGrowables | ModOptions.LoadNativeGrowables;
                    this.m_loadNativeGrowablesCheckBox.isChecked = true;
                    this.m_loadNonNativeGrowablesCheckBox.isChecked = true;
                }
                return;
            }
            catch (Exception e)
            {
                Debug.LogError("Unexpected " + e.GetType().Name + " loading options: " + e.Message + "\n" + e.StackTrace);
                return;
            }
            if (!(options.loadNativeGrowables || options.loadNonNativeGrowables))
            {
                throw new Exception("European Buildings Unlocker  - at least one set of vanilla growables must be loaded!");
            }
            if (options.loadNativeGrowables)
                EuroBuildingsUnlocker.Options |= ModOptions.LoadNativeGrowables;

            if (options.loadNonNativeGrowables)
                EuroBuildingsUnlocker.Options |= ModOptions.LoadNonNativeGrowables;

            if (options.overrideNativeTraficLights)
                EuroBuildingsUnlocker.Options |= ModOptions.OverrideNativeTrafficLights;

            if (options.addCustomAssetsGameObject)
                EuroBuildingsUnlocker.Options |= ModOptions.AddCustomAssetsGameObject;

            if (this.m_loadNativeGrowablesCheckBox != null)
            {
                this.m_loadNativeGrowablesCheckBox.isChecked = options.loadNativeGrowables;
                this.m_loadNonNativeGrowablesCheckBox.isChecked = options.loadNonNativeGrowables;
                this.m_overrideNativeTrafficLightsCheckBox.isChecked = options.overrideNativeTraficLights;
                this.m_addCustomAssetsGameObjectCheckBox.isChecked = options.addCustomAssetsGameObject;
            }
        }

        void OnLevelWasLoaded(int level)
        {
            if (level == 5 || level == 3)
                m_initialized = false;
        }

        void Update()
        {
            if (Application.loadedLevel == 3 || Application.loadedLevel == 5) // both are the main menu!?
            {
                if (!m_initialized || m_optionsButtonGo == null)
                    Start();
                else if (m_optionsButtonGo.GetComponent<UIButton>().isVisible == false)
                    m_optionsButtonGo.GetComponent<UIButton>().isVisible = true;
            }
        }

        public struct Options
        {
            public bool loadNativeGrowables;
            public bool loadNonNativeGrowables;
            public bool overrideNativeTraficLights;
            public bool addCustomAssetsGameObject;
        }
    }
}
