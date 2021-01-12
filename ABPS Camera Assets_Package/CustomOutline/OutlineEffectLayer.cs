
using Bolt;
using CustomOutline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace OutlineEffect
{
    public class OutlineEffectLayer : MonoBehaviour
    {
        public static OutlineEffectLayer Instance { get; private set; }

        private LinkedSet<OutlineGroup> outlineGroups = new LinkedSet<OutlineGroup>();


		[Range(1.0f, 6.0f)]
		public float lineThickness = 1.25f;
		[Range(0, 10)]
		public float lineIntensity = .5f;
		[Range(0, 1)]
		public float fillAmount = 0.2f;

		public Color lineColor0 = Color.red;
		public Color lineColor1 = Color.green;
		public Color lineColor2 = Color.blue;

		public bool additiveRendering = false;
		public bool backfaceCulling = true;

		public Color fillColor = Color.blue;
		public bool useFillColor = false;

		[Header("These settings can affect performance!")]
		public bool cornerOutlines = false;
		public bool addLinesBetweenColors = false;

		[Header("Advanced settings")]
		public bool scaleWithScreenSize = true;
		[Range(0.0f, 1.0f)]
		public float alphaCutoff = .5f;
		public bool flipY = false;
		public Camera sourceCamera;
		public bool autoEnableOutlines = false;

		[HideInInspector]
		public Camera outlineCamera;
		Material outline1Material;
		Material outline2Material;
		Material outline3Material;
		Material outlineEraseMaterial;
		Shader outlineShader;
		Shader outlineBufferShader;
		[HideInInspector]
		public Material outlineShaderMaterial;
		[HideInInspector]
		public RenderTexture renderTexture;
		[HideInInspector]
		public RenderTexture extraRenderTexture;

		CommandBuffer commandBuffer;

		bool RenderTheNextFrame;

		Material GetMaterialFromID(int ID)
		{
			if (ID == 0)
				return outline1Material;
			else if (ID == 1)
				return outline2Material;
			else if (ID == 2)
				return outline3Material;
			else
				return outline1Material;
		}
		List<Material> materialBuffer = new List<Material>();
		Material CreateMaterial(Color emissionColor)
		{
			Material m = new Material(outlineBufferShader);
			m.SetColor("_Color", emissionColor);
			m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			m.SetInt("_ZWrite", 0);
			m.DisableKeyword("_ALPHATEST_ON");
			m.EnableKeyword("_ALPHABLEND_ON");
			m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			m.renderQueue = 3000;
			return m;
		}

		

		private void Awake()
		{
			//if (Instance != null)
			//{
			//	Destroy(this);
			//	throw new System.Exception("you can only have one outline camera in the scene");
			//}
			//Instance = this;
		}

		// Start is called before the first frame update
		void Start()
        {
			CreateMaterialsIfNeeded();
			UpdateMaterialsPublicProperties();

			if (sourceCamera == null)
			{
				sourceCamera = GetComponent<Camera>();

				if (sourceCamera == null)
					sourceCamera = Camera.main;
			}

			if (outlineCamera == null)
			{
				foreach (Camera c in GetComponentsInChildren<Camera>())
				{
					if (c.name == "Outline Camera")
					{
						outlineCamera = c;
						c.enabled = false;

						break;
					}
				}

				if (outlineCamera == null)
				{
					GameObject cameraGameObject = new GameObject("Outline Camera");
					cameraGameObject.transform.parent = sourceCamera.transform;
					outlineCamera = cameraGameObject.AddComponent<Camera>();
					outlineCamera.enabled = false;
				}
			}

			if (renderTexture != null)
				renderTexture.Release();
			if (extraRenderTexture != null)
				renderTexture.Release();
			renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
			extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
			UpdateOutlineCameraFromSource();

			commandBuffer = new CommandBuffer();
			outlineCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);
		}

		public void OnPreRender()
		{
			if (commandBuffer == null)
				return;

			// The first frame during which there are no outlines, we still need to render 
			// to clear out any outlines that were being rendered on the previous frame
			if (outlineGroups.Count == 0)
			{
				if (!RenderTheNextFrame)
					return;

				RenderTheNextFrame = false;
			}
			else
			{
				RenderTheNextFrame = true;
			}

			CreateMaterialsIfNeeded();

			if (renderTexture == null || renderTexture.width != sourceCamera.pixelWidth || renderTexture.height != sourceCamera.pixelHeight)
			{
				if (renderTexture != null)
					renderTexture.Release();
				if (extraRenderTexture != null)
					renderTexture.Release();
				renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
				extraRenderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
				outlineCamera.targetTexture = renderTexture;
			}
			UpdateMaterialsPublicProperties();
			UpdateOutlineCameraFromSource();
			outlineCamera.targetTexture = renderTexture;
			commandBuffer.SetRenderTarget(renderTexture);

			commandBuffer.Clear();

			foreach (OutlineGroup group in outlineGroups)
			{
				LayerMask mask = sourceCamera.cullingMask;

				foreach (OutlineComponent comp in group.outlineComponents)
				{

					if (comp != null && mask == (mask | (1 << comp.gameObject.layer)))
					{
						for (int v = 0; v < comp.SharedMaterials.Length; v++)
						{
							Material m = null;

							if (comp.SharedMaterials[v].HasProperty("_MainTex") && comp.SharedMaterials[v].mainTexture != null && comp.SharedMaterials[v])
							{
								foreach (Material g in materialBuffer)
								{
									if (g.mainTexture == comp.SharedMaterials[v].mainTexture)
									{
										if (group.eraseRenderer && g.color == outlineEraseMaterial.color)
											m = g;
										else if (!group.eraseRenderer && g.color == GetMaterialFromID(group.colorID).color)
											m = g;
									}
								}

								if (m == null)
								{
									if (group.eraseRenderer)
										m = new Material(outlineEraseMaterial);
									else
										m = new Material(GetMaterialFromID(group.colorID));

									m.mainTexture = comp.SharedMaterials[v].mainTexture;
									materialBuffer.Add(m);
								}
							}
							else
							{
								if (group.eraseRenderer)
									m = outlineEraseMaterial;
								else
									m = GetMaterialFromID(group.colorID);
							}

							if (backfaceCulling)
								m.SetInt("_Culling", (int)UnityEngine.Rendering.CullMode.Back);
							else
								m.SetInt("_Culling", (int)UnityEngine.Rendering.CullMode.Off);

							MeshFilter mL = comp.meshFilter;
							SkinnedMeshRenderer sMR = comp.skinnedMeshRenderer;
							SpriteRenderer sR = comp.spriteRenderer;
							if (mL)
							{
								if (mL.sharedMesh != null)
								{
									if (v < mL.sharedMesh.subMeshCount)
										commandBuffer.DrawRenderer(comp.renderer, m, v, 0);
								}
							}
							else if (sMR)
							{
								if (sMR.sharedMesh != null)
								{
									if (v < sMR.sharedMesh.subMeshCount)
										commandBuffer.DrawRenderer(comp.renderer, m, v, 0);
								}
							}
							else if (sR)
							{
								commandBuffer.DrawRenderer(comp.renderer, m, v, 0);
							}
						}
					}
				}
			}
			outlineCamera.Render();
		}


		private void OnEnable()
		{
			Instance = this;
			//OutlineGroup[] og = FindObjectsOfType<OutlineGroup>();
			//if (autoEnableOutlines)
			//{
			//	foreach (OutlineGroup g in og)
			//	{
			//		g.enabled = false;
			//		g.enabled = true;
			//	}
			//}
			//else
			//{
			//	foreach (OutlineGroup g in og)
			//	{
			//		if (!outlineGroups.Contains(g))
			//		{
			//			outlineGroups.Add(g);
			//		}
			//	}
			//}
		}

		private void OnDisable()
		{
			//Instance = null;
		}

		void OnDestroy()
		{
			if (renderTexture != null)
				renderTexture.Release();
			if (extraRenderTexture != null)
				extraRenderTexture.Release();
			DestroyMaterials();
		}

		private void DestroyMaterials()
		{
			foreach (Material m in materialBuffer)
				DestroyImmediate(m);
			materialBuffer.Clear();
			DestroyImmediate(outlineShaderMaterial);
			DestroyImmediate(outlineEraseMaterial);
			DestroyImmediate(outline1Material);
			DestroyImmediate(outline2Material);
			DestroyImmediate(outline3Material);
			outlineShader = null;
			outlineBufferShader = null;
			outlineShaderMaterial = null;
			outlineEraseMaterial = null;
			outline1Material = null;
			outline2Material = null;
			outline3Material = null;
		}

		[ImageEffectOpaque]
		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (outlineShaderMaterial != null)
			{
				outlineShaderMaterial.SetTexture("_OutlineSource", renderTexture);

				if (addLinesBetweenColors)
				{
					Graphics.Blit(source, extraRenderTexture, outlineShaderMaterial, 0);
					outlineShaderMaterial.SetTexture("_OutlineSource", extraRenderTexture);
				}
				Graphics.Blit(source, destination, outlineShaderMaterial, 1);
			}
		}

		private void CreateMaterialsIfNeeded()
		{
			if (outlineShader == null)
				outlineShader = Resources.Load<Shader>("OutlineShader");
			if (outlineBufferShader == null)
			{
				outlineBufferShader = Resources.Load<Shader>("OutlineBufferShader");
			}
			if (outlineShaderMaterial == null)
			{
				outlineShaderMaterial = new Material(outlineShader);
				outlineShaderMaterial.hideFlags = HideFlags.HideAndDontSave;
				UpdateMaterialsPublicProperties();
			}
			if (outlineEraseMaterial == null)
				outlineEraseMaterial = CreateMaterial(new Color(0, 0, 0, 0));
			if (outline1Material == null)
				outline1Material = CreateMaterial(new Color(1, 0, 0, 0));
			if (outline2Material == null)
				outline2Material = CreateMaterial(new Color(0, 1, 0, 0));
			if (outline3Material == null)
				outline3Material = CreateMaterial(new Color(0, 0, 1, 0));
		}


		public void UpdateMaterialsPublicProperties()
		{
			if (outlineShaderMaterial)
			{
				float scalingFactor = 1;
				if (scaleWithScreenSize)
				{
					// If Screen.height gets bigger, outlines gets thicker
					scalingFactor = Screen.height / 360.0f;
				}

				// If scaling is too small (height less than 360 pixels), make sure you still render the outlines, but render them with 1 thickness
				if (scaleWithScreenSize && scalingFactor < 1)
				{
					if (UnityEngine.XR.XRSettings.isDeviceActive && sourceCamera.stereoTargetEye != StereoTargetEyeMask.None)
					{
						outlineShaderMaterial.SetFloat("_LineThicknessX", (1 / 1000.0f) * (1.0f / UnityEngine.XR.XRSettings.eyeTextureWidth) * 1000.0f);
						outlineShaderMaterial.SetFloat("_LineThicknessY", (1 / 1000.0f) * (1.0f / UnityEngine.XR.XRSettings.eyeTextureHeight) * 1000.0f);
					}
					else
					{
						outlineShaderMaterial.SetFloat("_LineThicknessX", (1 / 1000.0f) * (1.0f / Screen.width) * 1000.0f);
						outlineShaderMaterial.SetFloat("_LineThicknessY", (1 / 1000.0f) * (1.0f / Screen.height) * 1000.0f);
					}
				}
				else
				{
					if (UnityEngine.XR.XRSettings.isDeviceActive && sourceCamera.stereoTargetEye != StereoTargetEyeMask.None)
					{
						outlineShaderMaterial.SetFloat("_LineThicknessX", scalingFactor * (lineThickness / 1000.0f) * (1.0f / UnityEngine.XR.XRSettings.eyeTextureWidth) * 1000.0f);
						outlineShaderMaterial.SetFloat("_LineThicknessY", scalingFactor * (lineThickness / 1000.0f) * (1.0f / UnityEngine.XR.XRSettings.eyeTextureHeight) * 1000.0f);
					}
					else
					{
						outlineShaderMaterial.SetFloat("_LineThicknessX", scalingFactor * (lineThickness / 1000.0f) * (1.0f / Screen.width) * 1000.0f);
						outlineShaderMaterial.SetFloat("_LineThicknessY", scalingFactor * (lineThickness / 1000.0f) * (1.0f / Screen.height) * 1000.0f);
					}
				}
				outlineShaderMaterial.SetFloat("_LineIntensity", lineIntensity);
				outlineShaderMaterial.SetFloat("_FillAmount", fillAmount);
				outlineShaderMaterial.SetColor("_FillColor", fillColor);
				outlineShaderMaterial.SetFloat("_UseFillColor", useFillColor ? 1 : 0);
				outlineShaderMaterial.SetColor("_LineColor1", lineColor0 * lineColor0);
				outlineShaderMaterial.SetColor("_LineColor2", lineColor1 * lineColor1);
				outlineShaderMaterial.SetColor("_LineColor3", lineColor2 * lineColor2);
				if (flipY)
					outlineShaderMaterial.SetInt("_FlipY", 1);
				else
					outlineShaderMaterial.SetInt("_FlipY", 0);
				if (!additiveRendering)
					outlineShaderMaterial.SetInt("_Dark", 1);
				else
					outlineShaderMaterial.SetInt("_Dark", 0);
				if (cornerOutlines)
					outlineShaderMaterial.SetInt("_CornerOutlines", 1);
				else
					outlineShaderMaterial.SetInt("_CornerOutlines", 0);

				Shader.SetGlobalFloat("_OutlineAlphaCutoff", alphaCutoff);
			}
		}


		void UpdateOutlineCameraFromSource()
		{
			outlineCamera.CopyFrom(sourceCamera);
			outlineCamera.renderingPath = RenderingPath.Forward;
			outlineCamera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			outlineCamera.clearFlags = CameraClearFlags.SolidColor;
			outlineCamera.rect = new Rect(0, 0, 1, 1);
			outlineCamera.cullingMask = 0;
			outlineCamera.targetTexture = renderTexture;
			outlineCamera.enabled = false;
#if UNITY_5_6_OR_NEWER
			outlineCamera.allowHDR = false;
#else
			outlineCamera.hdr = false;
#endif
		}

		// Update is called once per frame
		void Update()
        {

        }

      

        public void AddOutline(OutlineGroup group)
        {
			outlineGroups.Add(group);
		
        }

        public void RemoveOutline(OutlineGroup group)
        {
			outlineGroups.Remove(group);
		
        }
    }
}

