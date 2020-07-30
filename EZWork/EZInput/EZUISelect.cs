using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace EZWork
{
    
    [RequireComponent(typeof(Selectable))]
    public class EZUISelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
    {
        [HideInInspector]
        public Selectable DefaultSelect, LastSelect;
        
        // 鼠标悬浮高亮
        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
  
        // 当其他Button高亮时，恢复本Button正常显示；否则将同时存在两个高亮
        public void OnDeselect(BaseEventData eventData)
        {
            GetComponent<Selectable>().OnPointerExit(null);
        }
    
        // 鼠标离开时取消高亮；否则持续高亮
        public void OnPointerExit(PointerEventData eventData)
        {
            if (EZInput.Instance.mUINavigationState == EZInput.UINavigationState.UI)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }
    
    public enum EZUISelectType
    {
        Default,
        Last
    }

}
