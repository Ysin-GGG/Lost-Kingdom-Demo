using LostKingdom.Content;
using LostKingdom.ID;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace LostKingdom.UI
{
    [DisallowMultipleComponent]
    public sealed class InitiativeTrack : LKBehaviour
    {
        [SerializeField]
        private GameObject _obj_scratch;
        [SerializeField]
        private List<Image> _list_avatars;

        public void UpdateSelf(LinkedList<Entity> curr, LinkedList<Entity> next)
        {
            List<Entity> entities = new List<Entity>(curr.Count + next.Count);
            foreach (Entity e in curr) entities.Add(e);
            foreach (Entity e in next) if (!curr.Contains(e)) entities.Add(e);
            for (int i = this._list_avatars.Count; i < entities.Count; i++)
            {
                Image image = Image.Instantiate(this._list_avatars[1]);
                image.transform.SetParent(this.transform);
                image.name = this._list_avatars[1].name.Substring(0, this._list_avatars[1].name.Length - 1) + i;
                this._list_avatars.Add(image);
            }
            for (int i = 0; i < this._list_avatars.Count; i++)
            {
                if (i < entities.Count)
                {
                    this._list_avatars[i].SetActive(true);
                    if (entities[i] is Character character) this._list_avatars[i].color = new LKColor(255);
                    else if (entities[i] is Enemy) this._list_avatars[i].color = new LKColor(ColorID.Enemy);
                    else this._list_avatars[i].color = new LKColor(ColorID.Ally);
                }
                else this._list_avatars[i].SetActive(false);
            }
            this._obj_scratch.transform.SetSiblingIndex(curr.Count);
        }
    }
}