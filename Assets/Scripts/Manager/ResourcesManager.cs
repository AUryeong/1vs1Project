using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{

    private bool isWritingResources = false;
    private Dictionary<string, GameObject> projectiles = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> itemIcons = new Dictionary<string, Sprite>();
    public Dictionary<string, Item> items = new Dictionary<string, Item>();
    public override void OnReset()
    {
        if (!isWritingResources)
            ReadResource();
        else
            foreach (Item item in items.Values)
                item.OnReset();
    }

    public void ReadResource()
    {
        foreach (var projectile in Resources.LoadAll<GameObject>("Projectile"))
            projectiles.Add(projectile.name, projectile);

        foreach (var icon in Resources.LoadAll<Sprite>("Item/Icon"))
            itemIcons.Add(icon.name, icon);

        ReadItem();

        isWritingResources = true;
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

            List<string> lore = new List<string>();
            // 2를 뺀 이유는 앞 2칸은 코드 아이템 이름, 아이템 이름이기 때문
            for (int i = 0; i < texts.Length - 2; i++)
            {
                // 이후 업그레이드가 없는 것에 대한 예외 처리
                if (string.IsNullOrWhiteSpace(texts[i + 2])) break;
                lore.Add(texts[i + 2]);
            }

            item.Init(itemName, lore.ToArray(), lore.Count - 1, GetItemIcon(codeItemName));

            items.Add(codeItemName, item);
        }
    }

    public Sprite GetItemIcon(string spriteName)
    {
        if (!itemIcons.ContainsKey(spriteName))
        {
            Debug.LogAssertion("Sprite not found : " + spriteName);
            return null;
        }
        return itemIcons[spriteName];
    }

    public GameObject GetProjectile(string projectileName)
    {
        if (!projectiles.ContainsKey(projectileName))
        {
            Debug.LogAssertion("Projectile not found : " + projectileName);
            return null;
        }
        return PoolManager.Instance.Init(projectiles[projectileName]);
    }
}
