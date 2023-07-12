using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{
    protected override bool IsDontDestroying => true;
    
    private readonly Dictionary<string, Item> items = new Dictionary<string, Item>();
    protected override void OnCreated()
    {
        ReadResource();
    }

    protected override void OnReset()
    {
        foreach (Item item in items.Values)
            item.OnReset();
    }

    private void ReadResource()
    {
        foreach (var projectile in Resources.LoadAll<GameObject>(nameof(Projectile)))
            PoolManager.Instance.AddPoolData(projectile.name, projectile);

        ReadItem();
    }

    private void ReadItem()
    {
        foreach (string line in Resources.Load<TextAsset>("Item/ItemList").text.Split('\n'))
        {
            string[] texts = line.Split(',');

            string codeItemName = texts[0];
            string itemName = texts[1];

            // csv 제작과정에서 예외 처리
            if (string.IsNullOrWhiteSpace(codeItemName) || codeItemName == "코드 아이템 이름") continue;

            // 아이템 생성
            Item item = System.Activator.CreateInstance(System.Type.GetType("Item_" + codeItemName)) as Item;

            if (item == null)
            {
                Debug.LogAssertion("Projectile not found : " + codeItemName + ", " + itemName);
                return;
            }
            
            List<string> lore = new List<string>();
            // 2를 뺀 이유는 앞 2칸은 코드 아이템 이름, 아이템 이름이기 때문
            for (int i = 0; i < texts.Length - 2; i++)
            {
                // 이후 업그레이드가 없는 것에 대한 예외 처리
                if (string.IsNullOrWhiteSpace(texts[i + 2])) break;
                lore.Add(texts[i + 2]);
            }

            item.Init(itemName, lore.ToArray(), lore.Count - 1);

            items.Add(codeItemName, item);
        }
    }

    public Item GetItem(string itemName)
    {
        if (!items.ContainsKey(itemName))
        {
            Debug.LogAssertion("Item not found : " + itemName);
            return null;
        }
        return items[itemName];
    }

    public List<Item> GetAllItems()
    {
        return items.Values.ToList();
    }
}
