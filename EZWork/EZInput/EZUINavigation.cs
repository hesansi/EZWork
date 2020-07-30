using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EZWork
{
    public class EZUINavigation : EZSingleton<EZUINavigation>
    {
        protected EZUINavigation(){}
        /// <summary>
        /// <按钮组名称，默认选中按钮>Dictionary
        /// </summary>
        public Dictionary<string, EZUISelect> SelectGroup = new Dictionary<string, EZUISelect>();
        /// <summary>
        /// 正在使用的按钮组名称List，用于维护前后关系
        /// </summary>
        public List<String> GroupList = new List<string>();
        
        /// <summary>
        /// 切换按钮组
        /// </summary>
        /// <param name="group">按钮组名称</param>
        /// <param name="select">选中按钮</param>
        /// <param name="selectType">选中默认按钮，还是上次离开按钮；如果上次离开按钮不存在，则选中默认按钮</param>
        public void SwitchGroup(string group, EZUISelect select, EZUISelectType selectType = EZUISelectType.Default)
        {
            // 1. 切换输入规则
            EZInput.Instance.SwitchToUI();
            // 2. 添加当前组
            if (SelectGroup.ContainsKey(group)) {
                // 防止对象已被清除，但Key仍在
                if (!SelectGroup[group]) {
                    SelectGroup.Remove(group);
                    SelectGroup.Add(group, select);
                }
            }
            else {
                SelectGroup.Add(group, select);
            }

            // 3. 在设置下个组前，先缓存当前组的 LastSelect
            if (GroupList.Count > 0) {
                    SelectGroup[GroupList.Last()].LastSelect =
                        EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            }

            // 4. 新组添加到最后
            if (GroupList.Contains(group)) {
                GroupList.Remove(group);
            }
            GroupList.Add(group);

            // 5. 设置选中
            SetGroupSelect(group, selectType);
        }

        /// <summary>
        /// 返回上个按钮组
        /// </summary>
        /// <param name="selectType">选中默认按钮，还是上次离开按钮</param>
        public void BackGroup(EZUISelectType selectType = EZUISelectType.Default)
        {
            int index = GroupList.Count - 2;
            if (index >= 0) {
                string groupName = GroupList[index];
                SwitchGroup(groupName, SelectGroup[groupName], selectType);
                GroupList.RemoveAt(index);
            }
            else {
                ClearAllGroup();
            }
        }
        
        /// <summary>
        /// 切换输入规则，清除缓存按钮组，移除选中按钮
        /// </summary>
        public void ClearAllGroup()
        {
            // 切换输入规则
            EZInput.Instance.SwitchToGameplay();
            // 移除选中按钮
            EventSystem.current.SetSelectedGameObject(null);
            // 清除缓存按钮组
            GroupList.Clear();
        }
        
        /// <summary>
        /// 设置按钮组的选中按钮
        /// </summary>
        /// <param name="group">按钮组名称</param>
        /// <param name="selectType">选中默认按钮，还是上次离开按钮</param>
        public void SetGroupSelect(string group, EZUISelectType selectType = EZUISelectType.Default)
        {
            switch (selectType) {
                case EZUISelectType.Default:
                    // 如果 DefaultSelect 不存在，则当前组件所在对象即为 DefaultSelect
                    if (!SelectGroup[group].DefaultSelect) {
                        SelectGroup[group].DefaultSelect = SelectGroup[group].GetComponent<Selectable>();
                    }
                    EventSystem.current.SetSelectedGameObject(SelectGroup[group].DefaultSelect.gameObject);
                    break;
                case EZUISelectType.Last:
                    if (SelectGroup[group].LastSelect) {
                        EventSystem.current.SetSelectedGameObject(SelectGroup[group].LastSelect.gameObject);
                    }
                    else {
                        // 如果 LastSelect 不存在，则使用 DefaultSelect
                        EventSystem.current.SetSelectedGameObject(SelectGroup[group].DefaultSelect.gameObject);
                    }
                    break;
            }
        }

    }



}

