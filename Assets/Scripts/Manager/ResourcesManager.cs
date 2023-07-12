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

            // csv ���۰������� ���� ó��
            if (string.IsNullOrWhiteSpace(codeItemName) || codeItemName == "�ڵ� ������ �̸�") continue;

            // ������ ����
            Item item = System.Activator.CreateInstance(System.Type.GetType("Item_" + codeItemName)) as Item;

            if (item == null)
            {
                Debug.LogAssertion("Projectile not found : " + codeItemName + ", " + itemName);
                return;
            }
            
            List<string> lore = new List<string>();
            // 2�� �� ������ �� 2ĭ�� �ڵ� ������ �̸�, ������ �̸��̱� ����
            for (int i = 0; i < texts.Length - 2; i++)
            {
                // ���� ���׷��̵尡 ���� �Ϳ� ���� ���� ó��
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
