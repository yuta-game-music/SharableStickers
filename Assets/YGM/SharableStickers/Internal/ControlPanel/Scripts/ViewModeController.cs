
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace YGM.SharableStickers.ControlPanel
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ViewModeController : UdonSharpBehaviourWithUtils
    {
        [SerializeField] private ViewModeItem[] m_items;
        public void Setup(SharableStickers.System system)
        {
            system.RegisterViewModeChangeEventHandler(this, nameof(UpdateView));
            UpdateView();
        }

        public void UpdateView()
        {
            foreach (var item in m_items)
            {
                if (item == null) continue;
                item.UpdateView();
            }
        }
    }

}