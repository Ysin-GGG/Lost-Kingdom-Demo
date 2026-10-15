using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace LostKingdom.Utility
{
    public static class LKUtility
    {
        private static List<Vector2> pointsArr = new List<Vector2>();

        /// <summary> 添加新对象到当前组件 </summary>
        /// <typeparam Name="T"> 继承了组件类的类型 </typeparam>
        /// <param Name="ele"> 当前组件 </param>
        /// <param Name="name"> 命名 </param>
        /// <param Name="active"> 是否在创建同时激活 </param>
        public static T AddElement<T>(this Component ele, string name, bool active = true) where T : Component
            => ele.gameObject.AddElement<T>(name, active);

        /// <summary> 添加新对象到当前组件 </summary>
        /// <typeparam Name="T"> 继承了组件类的类型 </typeparam>
        /// <param Name="ele"> 当前组件 </param>
        /// <param Name="name"> 命名 </param>
        /// <param Name="active"> 是否在创建同时激活 </param>
        public static T AddElement<T>(this GameObject gobj, string name, bool active = true) where T : Component
        {
            T ele = new GameObject(name).AddComponent<T>();
            ele.gameObject.transform.SetParent(gobj.transform);
            ele.gameObject.SetActive(active);
            return ele;
        }

        /// <summary> 添加新组件到当前对象 </summary>
        public static T AddComponent<T>(this Component component, bool active = true) where T : Component
        {
            T ele = component.gameObject.AddComponent<T>();
            ele.gameObject.SetActive(active);
            return ele;
        }

        public static bool SetActive(this Component component, bool? active = null)
        {
            if (component == null) return false;
            bool value = active ?? !component.gameObject.activeSelf;
            component.gameObject.SetActive(value);
            return value;
        }

        public static bool SetActive(this GameObject gobj, bool? active = null)
        {
            if (gobj == null) return false;
            bool value = active ?? !gobj.gameObject.activeSelf;
            gobj.gameObject.SetActive(value);
            return value;
        }

        public static bool GetActive(this Component component)
        {
            return component.gameObject.activeSelf;
        }

        public static bool GetActive(this GameObject gobj)
        {
            return gobj.gameObject.activeSelf;
        }

        public static void SetParent(this GameObject gobj, MonoBehaviour parent)
        {
            gobj.transform.SetParent(parent.transform);
        }

        public static void SetParent(this GameObject gobj, GameObject parent)
        {
            gobj.transform.SetParent(parent.transform);
        }

        public static void SetParent(this GameObject gobj, Transform parent)
        {
            gobj.transform.SetParent(parent);
        }

        /// <summary> 获取贴图精灵 </summary>
        /// <param Name="texture2d"> 贴图 </param>
        public static Sprite GetSprite(this Texture2D texture2d)
            => Sprite.Create(texture2d, new Rect(0f, 0f, texture2d.width, texture2d.height), Vector2.one * .5f);

        /// <summary> 获取贴图精灵 </summary>
        /// <param Name="texture2d"> 贴图 </param>
        public static Vector2 GetSize(this Texture2D texture2d)
            => new Vector2(texture2d.width, texture2d.height);

        /// <summary> </summary>
        /// <param Name="rectTransform"></param>
        public static void SetAnchor(this RectTransform rectTransform)
        {
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }

        /// <summary> </summary>
        /// <param Name="rectTransform"></param>
        public static void SetAnchor(this RectTransform rectTransform, float minX, float minY, float maxX, float maxY)
        {
            rectTransform.pivot = Vector2.one * .5f;
            rectTransform.anchorMin = new Vector2(minX, minY);
            rectTransform.anchorMax = new Vector2(maxX, maxY);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }

        /// <summary> </summary>
        /// <param Name="rectTransform"></param>
        public static void SetAnchor(this RectTransform rectTransform, float value, bool dontResetSize = false)
        {
            rectTransform.anchorMin = new Vector2(value, value);
            rectTransform.anchorMax = new Vector2(value, value);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            if (!dontResetSize)
            {
                rectTransform.pivot = Vector2.one * .5f;
                rectTransform.sizeDelta = Vector2.zero;
            }
        }

        /// <summary> </summary>
        /// <param Name="rectTransform"></param>
        public static void SetAnchor(this RectTransform rectTransform, float valueX, float valueY, bool dontResetSize = false)
        {
            rectTransform.anchorMin = new Vector2(valueX, valueY);
            rectTransform.anchorMax = new Vector2(valueX, valueY);
            rectTransform.anchoredPosition = Vector2.zero;
            if (!dontResetSize)
            {
                rectTransform.pivot = Vector2.one * .5f;
                rectTransform.sizeDelta = Vector2.zero;
            }
        }

        /// <summary> 改变锚点 </summary>
        /// <param Name="rectTransform"> 组件矩形 </param>
        /// <param Name="minX"> 锚点左端坐标 </param>
        /// <param Name="minY"> 锚点下端坐标 </param>
        /// <param Name="maxX"> 锚点右端坐标 </param>
        /// <param Name="maxY"> 锚点上端坐标 </param>
        /// <param Name="pivotX"> 锚点水平坐标 </param>
        /// <param Name="pivotY"> 锚点垂直坐标 </param>
        /// <param Name="dontResetSize"> 不改变原尺寸 </param>
        public static void SetAnchor(this RectTransform rectTransform, float minX, float minY, float maxX, float maxY, float pivotX, float pivotY, bool dontResetSize = false)
        {
            rectTransform.pivot = new Vector2(pivotX, pivotY);
            rectTransform.anchorMin = new Vector2(minX, minY);
            rectTransform.anchorMax = new Vector2(maxX, maxY);
            rectTransform.anchoredPosition = Vector2.zero;
            if (!dontResetSize) rectTransform.sizeDelta = Vector2.zero;
        }

        /// <summary> </summary>
        /// <param Name="rectTransform"></param>
        /// <param Name="anchor"> （minX, minY, maxX, maxY）</param>
        public static void SetAnchor(this RectTransform rectTransform, Vector4 anchor, bool dontResetSize = false)
        {
            rectTransform.anchorMin = new Vector2(anchor.x, anchor.y);
            rectTransform.anchorMax = new Vector2(anchor.z, anchor.w);
            rectTransform.anchoredPosition = Vector2.zero;
            if (!dontResetSize)
            {
                rectTransform.pivot = Vector2.one * .5f;
                rectTransform.sizeDelta = Vector2.zero;
            }
        }

        /// <summary> 设置对象的锚点 </summary>
        /// <param Name="rectTransform"></param>
        /// <param Name="anchor"> （minX, minY, maxX, maxY）</param>
        /// <param Name="pivot"></param>
        public static void SetAnchor(this RectTransform rectTransform, Vector4 anchor, Vector2 pivot, bool dontResetSize = false)
        {
            rectTransform.pivot = pivot;
            rectTransform.anchorMin = new Vector2(anchor.x, anchor.y);
            rectTransform.anchorMax = new Vector2(anchor.z, anchor.w);
            rectTransform.anchoredPosition = Vector2.zero;
            if (!dontResetSize) rectTransform.sizeDelta = Vector2.zero;
        }

        /// <summary> 设置对象的位置尺寸 </summary>
        /// <param Name="rectTransform"></param>
        /// <param Name="rect"> （x=横坐标, y=纵坐标, width=宽度, height=高度）</param>
        public static void SetXYWH(this RectTransform rectTransform, Rect rect) => LKUtility.SetXYWH(rectTransform, rect.x, rect.y, rect.width, rect.height);

        /// <summary> 设置对象的位置尺寸 </summary>
        /// <param Name="rectTransform"></param>
        /// <param Name="xywh"> （x=横坐标, y=纵坐标, z=宽度, w=高度）</param>
        public static void SetXYWH(this RectTransform rectTransform, Vector4 xywh) => LKUtility.SetXYWH(rectTransform, xywh.x, xywh.y, xywh.z, xywh.w);

        /// <summary> 设置对象的位置尺寸 </summary>
        /// <param Name="rectTransform"></param>
        /// <param Name="x">横坐标</param>
        /// <param Name="y">纵坐标</param>
        /// <param Name="width">宽度</param>
        /// <param Name="height">高度</param>
        public static void SetXYWH(this RectTransform rectTransform, int x, int y, int width, int height) => LKUtility.SetXYWH(rectTransform, (float)x, (float)y, (float)width, (float)height);

        /// <summary> 设置对象的位置尺寸 </summary>
        /// <param Name="rectTransform"></param>
        /// <param Name="x">横坐标</param>
        /// <param Name="y">纵坐标</param>
        /// <param Name="width">宽度</param>
        /// <param Name="height">高度</param>
        public static void SetXYWH(this RectTransform rectTransform, float x, float y, float width, float height)
        {
            bool flagX = rectTransform.anchorMin.x == rectTransform.anchorMax.x;
            bool flagY = rectTransform.anchorMin.y == rectTransform.anchorMax.y;
            if (flagX || flagY)
            {
                rectTransform.localPosition = new Vector3(flagX ? x : rectTransform.localPosition.x, flagY ? y : rectTransform.localPosition.y, rectTransform.localPosition.z);
                rectTransform.sizeDelta = new Vector2(flagX ? width : rectTransform.sizeDelta.x, flagX ? height : rectTransform.sizeDelta.y);
            }
            if (!flagX || !flagY)
            {
                if (!flagX) rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                if (!flagY) rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                rectTransform.anchoredPosition = new Vector2(flagX ? rectTransform.anchoredPosition.x : x, flagY ? rectTransform.anchoredPosition.y : y);
            }
        }

        public static Sprite[] GetSlices(string path) => LKUtility.GetSlices(path, Vector2.one * .5f);

        [System.Obsolete]
        public static Sprite[] GetSlices(string path, Vector2 pivot, float pixelsPerUnit = 100f)
        {
            Texture2D t2d = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            TextureImporter imporeter = AssetImporter.GetAtPath(path) as TextureImporter;
            SpriteMetaData[] meta = imporeter.spritesheet;
            Sprite[] slice = new Sprite[meta.Length];
            string name = Path.GetFileNameWithoutExtension(path);
            for (int i = 0; i < meta.Length; i++)
            {
                slice[i] = Sprite.Create(t2d, meta[i].rect, pivot, pixelsPerUnit);
                slice[i].name = $"{name}_{i}";
            }
            return slice;
        }

        public static Sprite[] GetSlices(Texture2D t2d) => LKUtility.GetSlices(t2d, Vector2.one * .5f);

        [System.Obsolete]
        public static Sprite[] GetSlices(Texture2D t2d, Vector2 pivot, float pixelsPerUnit = 100f)
        {
            string path = AssetDatabase.GetAssetPath(t2d);
            TextureImporter imporeter = AssetImporter.GetAtPath(path) as TextureImporter;
            SpriteMetaData[] meta = imporeter.spritesheet;
            Sprite[] slice = new Sprite[meta.Length];
            string name = Path.GetFileNameWithoutExtension(path);
            for (int i = 0; i < meta.Length; i++)
            {
                slice[i] = Sprite.Create(t2d, meta[i].rect, pivot, pixelsPerUnit);
                slice[i].name = $"{name}_{i}";
            }
            return slice;
        }

        public static void SetLayer(this GameObject obj, byte id)
        {
            obj.gameObject.layer = id;
        }

        public static void SetLayer(this Transform trans, byte id)
        {
            trans.gameObject.layer = id;
        }

        public static void SetChildrenLayer(this GameObject obj, byte id)
        {
            for (int i = 0, j = obj.gameObject.transform.childCount; i < j; i++)
            {
                Transform child = obj.transform.GetChild(i);
                child.gameObject.layer = id;
                child.SetChildrenLayer(id);
            }
        }

        public static void SetChildrenLayer(this Transform trans, byte id)
        {
            for (int i = 0, j = trans.childCount; i < j; i++)
            {
                Transform child = trans.GetChild(i);
                child.gameObject.layer = id;
                child.SetChildrenLayer(id);
            }
        }

        /// <summary> 布尔值转0或1 </summary>
        /// <param name="flag"> 布尔值 </param>
        /// <returns> 为true时，返回1，否则返回0 </returns>
        public static int To01(this bool flag) => flag ? 1 : 0;

        /// <summary> 布尔值转-1或1 </summary>
        /// <param name="flag"> 布尔值 </param>
        /// <returns> 为true时，返回1，否则返回-1 </returns>
        public static int To11(this bool flag) => flag ? 1 : -1;

        public static void Write(this BinaryWriter writer, Vector2 data)
        {
            writer.Write(data.x);
            writer.Write(data.y);
        }

        public static void Write(this BinaryWriter writer, Vector3 data)
        {
            writer.Write(data.x);
            writer.Write(data.y);
            writer.Write(data.z);
        }

        public static void Write(this BinaryWriter writer, Vector2Int data)
        {
            writer.Write(data.x);
            writer.Write(data.y);
        }

        public static void Write(this BinaryWriter writer, Vector3Int data)
        {
            writer.Write(data.x);
            writer.Write(data.y);
            writer.Write(data.z);
        }

        public static void Write(this BinaryWriter writer, params int[] datas)
        {
            writer.Write(datas.Length);
            foreach (int data in datas)
                writer.Write(data);
        }

        public static void Write(this BinaryWriter writer, params float[] datas)
        {
            writer.Write(datas.Length);
            foreach (float data in datas)
                writer.Write(data);
        }

        public static void AutoSetShape(this PolygonCollider2D collider, Sprite sprite)
        {
            if (collider == null || sprite == null) return;
            int shapeCount = sprite.GetPhysicsShapeCount();
            if (shapeCount > 0)
            {
                collider.pathCount = shapeCount;
                for (int i = 0; i < shapeCount; i++)
                {
                    sprite.GetPhysicsShape(i, LKUtility.pointsArr);
                    collider.SetPath(i, LKUtility.pointsArr);
                    LKUtility.pointsArr.Clear();
                }
            }
        }
    }
}