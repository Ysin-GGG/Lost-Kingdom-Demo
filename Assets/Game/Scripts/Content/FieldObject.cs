using LostKingdom.Components;
using LostKingdom.DataStructure;
using LostKingdom.Interface;
using LostKingdom.IO;
using LostKingdom.Manager;
using LostKingdom.Utility;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LostKingdom.Content
{
    public class FieldObject : IDataSL
    {
        public ObjectMemory obj { get; private set; }
        public float Rotation { get => this.rotation.z; set => this.rotation.z = value; }
        public float PositionX { get => this.position.x; set => this.position.x = value; }
        public float PositionY { get => this.position.y; set => this.position.y = value; }
        public Vector2 Position { get => this.position; set => this.position = value; }
        private Vector2 position;
        private Vector3 rotation;
        private Vector3 scale;

        private bool hasRenderer;
        private bool hasCollider;
        private bool hasInteraction;
        private bool hasLight;
        private bool rendererEnabled;
        private bool colliderEnabled;
        private bool interactionEnabled;
        private bool lightEnabled;
        private Sprite sprite;
        private Material material;
        private LKColor spriteColor;
        private bool flipX;
        private bool flipY;
        private Vector2 size;
        private SpriteDrawMode drawMode;
        private SpriteTileMode tileMode;
        private SpriteSortPoint spriteSortPoint;
        private SpriteMaskInteraction maskInteraction;
        private uint renderingLayerMask;
        private int sortingLayerID;
        private int sortingOrder;
        private int clickEventID;
        private LKColor lightColor;
        private Light2D.LightType lightType;
        private float intensity;
        private float falloffIntensity;
        private float shapeLightFalloffSize;
        private Sprite lightCookieSprite;
        private float pointLightInnerRadius;
        private float pointLightOuterRadius;
        private float pointLightInnerAngle;
        private float pointLightOuterAngle;
        private int blendStyleIndex;
        private int lightOrder;
        private Light2D.OverlapOperation overlapOperation;
        private bool shadowsEnabled;
        private float shadowIntensity;
        private bool volumeIntensityEnabled;
        private float volumeIntensity;
        private bool volumetricShadowsEnabled;

        public void SaveData(ObjectMemory obj)
        {
            this.obj = obj;
            if (obj != null)
            {
                this.position = obj.transform.position;
                this.rotation = obj.transform.eulerAngles;
                this.scale = obj.transform.localScale;
                if (this.hasRenderer = obj.HasRenderer)
                {
                    this.rendererEnabled = obj.Renderer.enabled;
                    this.sprite = obj.Renderer.sprite;
                    this.material = obj.Renderer.sharedMaterial;
                    this.spriteColor = obj.Renderer.color;
                    this.flipX = obj.Renderer.flipX;
                    this.flipY = obj.Renderer.flipY;
                    this.size = obj.Renderer.size;
                    this.drawMode = obj.Renderer.drawMode;
                    this.tileMode = obj.Renderer.tileMode;
                    this.spriteSortPoint = obj.Renderer.spriteSortPoint;
                    this.maskInteraction = obj.Renderer.maskInteraction;
                    this.renderingLayerMask = obj.Renderer.renderingLayerMask;
                    this.sortingLayerID = obj.Renderer.sortingLayerID;
                    this.sortingOrder = obj.Renderer.sortingOrder;
                }
                if (this.hasCollider = obj.HasCollider)
                {
                    this.colliderEnabled = obj.Collider.enabled;
                }
                if (this.hasInteraction = obj.HasInteraction)
                {
                    this.interactionEnabled = obj.Interaction.enabled;
                    this.clickEventID = obj.Interaction.ClickEventID;
                }
                if (this.hasLight = obj.HasLight)
                {
                    this.lightEnabled = obj.Light.enabled;
                    this.lightColor = obj.Light.color;
                    this.lightType = obj.Light.lightType;
                    this.intensity = obj.Light.intensity;
                    this.falloffIntensity = obj.Light.falloffIntensity;
                    this.shapeLightFalloffSize = obj.Light.shapeLightFalloffSize;
                    this.lightCookieSprite = obj.Light.lightCookieSprite;
                    this.pointLightInnerRadius = obj.Light.pointLightInnerRadius;
                    this.pointLightOuterRadius = obj.Light.pointLightOuterRadius;
                    this.pointLightInnerAngle = obj.Light.pointLightInnerAngle;
                    this.pointLightOuterAngle = obj.Light.pointLightOuterAngle;
                    this.blendStyleIndex = obj.Light.blendStyleIndex;
                    this.lightOrder = obj.Light.lightOrder;
                    this.overlapOperation = obj.Light.overlapOperation;
                    this.shadowsEnabled = obj.Light.shadowsEnabled;
                    this.shadowIntensity = obj.Light.shadowIntensity;
                    this.volumeIntensityEnabled = obj.Light.volumeIntensityEnabled;
                    this.volumeIntensity = obj.Light.volumeIntensity;
                    this.volumetricShadowsEnabled = obj.Light.volumetricShadowsEnabled;
                }
            }
        }

        public void LoadData(ObjectMemory obj)
        {
            if (obj != null)
            {
                obj.transform.position = this.position;
                obj.transform.eulerAngles = this.rotation;
                obj.transform.localScale = this.scale;
                if (this.hasRenderer)
                {
                    obj.Renderer.enabled = this.rendererEnabled;
                    obj.Renderer.sprite = this.sprite;
                    obj.Renderer.sharedMaterial = this.material;
                    obj.Renderer.color = this.spriteColor;
                    obj.Renderer.flipX = this.flipX;
                    obj.Renderer.flipY = this.flipY;
                    obj.Renderer.size = this.size;
                    obj.Renderer.drawMode = this.drawMode;
                    obj.Renderer.tileMode = this.tileMode;
                    obj.Renderer.spriteSortPoint = this.spriteSortPoint;
                    obj.Renderer.maskInteraction = this.maskInteraction;
                    obj.Renderer.renderingLayerMask = this.renderingLayerMask;
                    obj.Renderer.sortingLayerID = this.sortingLayerID;
                    obj.Renderer.sortingOrder = this.sortingOrder;
                }
                
                if (this.hasCollider)
                {
                    obj.Collider.enabled = this.colliderEnabled;
                    obj.Collider.AutoSetShape(this.sprite);
                }
                if (this.hasInteraction)
                {
                    obj.Interaction.enabled = this.interactionEnabled;
                    obj.Interaction.ClickEventID = this.clickEventID;
                }
                if (this.hasLight)
                {
                    obj.Light.enabled = this.lightEnabled;
                    obj.Light.color = this.lightColor;
                    obj.Light.lightType = this.lightType;
                    obj.Light.intensity = this.intensity;
                    obj.Light.falloffIntensity = this.falloffIntensity;
                    obj.Light.shapeLightFalloffSize = this.shapeLightFalloffSize;
                    obj.Light.lightCookieSprite = this.lightCookieSprite;
                    obj.Light.pointLightInnerRadius = this.pointLightInnerRadius;
                    obj.Light.pointLightOuterRadius = this.pointLightOuterRadius;
                    obj.Light.pointLightInnerAngle = this.pointLightInnerAngle;
                    obj.Light.pointLightOuterAngle = this.pointLightOuterAngle;
                    obj.Light.blendStyleIndex = this.blendStyleIndex;
                    obj.Light.lightOrder = this.lightOrder;
                    obj.Light.overlapOperation = this.overlapOperation;
                    obj.Light.shadowsEnabled = this.shadowsEnabled;
                    obj.Light.shadowIntensity = this.shadowIntensity;
                    obj.Light.volumeIntensityEnabled = this.volumeIntensityEnabled;
                    obj.Light.volumeIntensity = this.volumeIntensity;
                    obj.Light.volumetricShadowsEnabled = this.volumetricShadowsEnabled;
                }
            }
        }

        public void SaveData(DataBuffer buffer)
        {
            buffer.Input<byte>(
                (byte)new Bit8(
                    this.hasRenderer,
                    this.hasCollider,
                    this.hasInteraction,
                    this.hasLight,
                    this.flipX,
                    this.flipY
                ),
                (byte)new Bit8(
                    this.rendererEnabled,
                    this.colliderEnabled,
                    this.interactionEnabled,
                    this.lightEnabled,
                    this.shadowsEnabled,
                    this.volumeIntensityEnabled,
                    this.volumetricShadowsEnabled
                ),
                (byte)this.drawMode,
                (byte)this.tileMode,
                (byte)this.spriteSortPoint,
                (byte)this.maskInteraction,
                (byte)this.lightType,
                (byte)this.overlapOperation
            );

            buffer.Input<int>(
                AssetManager.GetSpriteID(this.sprite),
                AssetManager.GetSpriteID(this.lightCookieSprite),
                AssetManager.GetMaterialID(this.material),
                this.sortingLayerID,
                this.sortingOrder,
                this.clickEventID,
                this.blendStyleIndex,
                this.lightOrder
            );

            buffer.Input<uint>(
                (uint)(this.spriteColor ?? new LKColor(255)),
                (uint)(this.lightColor ?? new LKColor(255)),
                this.renderingLayerMask
            );

            buffer.Input<float>(
                    this.intensity,
                    this.falloffIntensity,
                    this.shapeLightFalloffSize,
                    this.pointLightInnerRadius,
                    this.pointLightOuterRadius,
                    this.pointLightInnerAngle,
                    this.pointLightOuterAngle,
                    this.shadowIntensity,
                    this.volumeIntensity
                );

            buffer.Input<Vector2>(this.size);
            buffer.Input<Vector2>(this.position);
            buffer.Input<Vector3>(this.rotation);
            buffer.Input<Vector3>(this.scale);
        }

        public void LoadData(DataBuffer buffer)
        {
            new Bit8(buffer.Output<byte>()).GetValues(
                out this.hasRenderer,
                out this.hasCollider,
                out this.hasInteraction,
                out this.hasLight,
                out this.flipX,
                out this.flipY
            );

            new Bit8(buffer.Output<byte>()).GetValues(
                out this.rendererEnabled,
                out this.colliderEnabled,
                out this.interactionEnabled,
                out this.lightEnabled,
                out this.shadowsEnabled,
                out this.volumeIntensityEnabled,
                out this.volumetricShadowsEnabled
            );

            this.drawMode = (SpriteDrawMode)buffer.Output<byte>();
            this.tileMode = (SpriteTileMode)buffer.Output<byte>();
            this.spriteSortPoint = (SpriteSortPoint)buffer.Output<byte>();
            this.maskInteraction = (SpriteMaskInteraction)buffer.Output<byte>();
            this.lightType = (Light2D.LightType)buffer.Output<byte>();
            this.overlapOperation = (Light2D.OverlapOperation)buffer.Output<byte>();

            this.sprite = AssetManager.GetSprite(buffer.Output<int>());
            this.lightCookieSprite = AssetManager.GetSprite(buffer.Output<int>());
            this.material = AssetManager.GetMaterial(buffer.Output<int>());

            buffer.Output<int>(out this.sortingLayerID);
            buffer.Output<int>(out this.sortingOrder);
            buffer.Output<int>(out this.clickEventID);
            buffer.Output<int>(out this.blendStyleIndex);
            buffer.Output<int>(out this.lightOrder);

            this.spriteColor = buffer.Output<uint>();
            this.lightColor = buffer.Output<uint>();
            this.renderingLayerMask = buffer.Output<uint>();

            buffer.Output<float>(out this.intensity);
            buffer.Output<float>(out this.falloffIntensity);
            buffer.Output<float>(out this.shapeLightFalloffSize);
            buffer.Output<float>(out this.pointLightInnerRadius);
            buffer.Output<float>(out this.pointLightOuterRadius);
            buffer.Output<float>(out this.pointLightInnerAngle);
            buffer.Output<float>(out this.pointLightOuterAngle);
            buffer.Output<float>(out this.shadowIntensity);
            buffer.Output<float>(out this.volumeIntensity);

            buffer.Output<Vector2>(out this.size);
            buffer.Output<Vector2>(out this.position);
            buffer.Output<Vector3>(out this.rotation);
            buffer.Output<Vector3>(out this.scale);
        }
    }
}