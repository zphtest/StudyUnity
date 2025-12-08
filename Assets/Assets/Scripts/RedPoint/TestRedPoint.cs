using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StudyUnity
{
    public class TestRedPoint : MonoBehaviour
    {
        public bool DoTest = false;
        public int RedPointCount = 0;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (DoTest)
            {
                OnBagItemUpdate(RedPointCount);
            }
        }
    
    
        void OnBagItemUpdate(int newItemCount)
        {
            // 只需要设置叶子节点
            // 比如路径是：Main -> Bag -> Tab_Material

            int count = newItemCount; //GetMaterialNewCount(); // 获取业务数据
    
            if (count > 1)
                RedDotManager.Instance.SetNodeValue("BtnEvents.Christmas.BtnClick", RedDotType.Number, count);
            else if(count == 1)
                RedDotManager.Instance.SetNodeValue("BtnEvents.Christmas.BtnClick", RedDotType.Dot);
            else if(count == -1)
                RedDotManager.Instance.SetNodeValue("BtnEvents.Christmas.BtnClick", RedDotType.New);
            else
                RedDotManager.Instance.SetNodeValue("BtnEvents.Christmas.BtnClick", RedDotType.None);
        
            // 此时：
            // 1. "Main.Bag.Tab_Material" 变数字
            // 2. "Main.Bag" 自动变成数字或红点（取决于聚合逻辑）
            // 3. "Main" 自动变红
            // 4. 如果 "Main" 界面没打开，UI组件不存在，没关系，数据层已经更新。
            // 5. 下次打开 "Main" 界面，UI组件Start时会取到最新数据。
        }
    }
}

