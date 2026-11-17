import os
import re

print("Starting complete generation of Settings UI & Main Menu refonte...")

GUID_IMAGE = "fe87c0e1cc204ed48ad3b37840f39efc"
GUID_BUTTON = "4e29b1a8efbd4b44bb3f3716e73f07ff"
GUID_TEXT = "5f7201a12d95ffc409449d95f23cf332"
GUID_SLIDER = "67db9e8f0e2ae9c40bc1e2b64352a6b4"
GUID_OUTLINE = "cfabb0440166ab443bba8876756fdfa9"
GUID_SETTINGS_UI = "6b3c4d5e6f7a8b9c0d1e2f3a4b5c6d7e"

SPRITE_SAVANNAH = "4bb3b67e2c71a2143a23837a0b61997f"
SPRITE_PLAY = "dbcf336d95e4aac4cac683a121967428"
SPRITE_COG = "f0f739589437a8a4ab43b4c1feed2d91"
SPRITE_SCORE_BADGE = "bc60ec6c2fb85d64481f39f2fb9e1c9d"
SPRITE_SOUND_ON = "074340e3ae824374fafff55c8eeed40c"
SPRITE_SOUND_OFF = "240a6320005158a47a54eab1650f7e8e"
SPRITE_RETICLE_CLASSIC = "5047fb28b20d59e41a03af6c60a9650d"
SPRITE_RETICLE_NDOP = "612ea7f093f791742b7f2243d0378fda"
SPRITE_RETICLE_PRECISION = "532b0a0fd08f4d045af5099b86a61466"

FONT_POPPINS = "eb6ca33255770ae49b8a0020ace003b5"

def generate_button(go_id, rt_id, cr_id, img_id, btn_id, txt_go_id, txt_rt_id, txt_cr_id, txt_comp_id, 
                    name, father_id, anchor_min, anchor_max, pos, size, bg_color, text_str, text_color, 
                    font_size, target_id=0, target_type="", method_name=""):
    calls_str = ""
    if target_id != 0:
        calls_str = f"""      - m_Target: {{fileID: {target_id}}}
        m_TargetAssemblyTypeName: {target_type}
        m_MethodName: {method_name}
        m_Mode: 1
        m_Arguments:
          m_ObjectArgument: {{fileID: 0}}
          m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine
          m_IntArgument: 0
          m_FloatArgument: 0
          m_StringArgument: 
          m_BoolArgument: 0
        m_CallState: 2"""

    return f"""--- !u!1 &{go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {rt_id}}}
  - component: {{fileID: {cr_id}}}
  - component: {{fileID: {img_id}}}
  - component: {{fileID: {btn_id}}}
  m_Layer: 5
  m_Name: {name}
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &{rt_id}
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: {txt_rt_id}}}
  m_Father: {{fileID: {father_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {anchor_min}
  m_AnchorMax: {anchor_max}
  m_AnchoredPosition: {pos}
  m_SizeDelta: {size}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &{cr_id}
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_CullTransparentMesh: 1
--- !u!114 &{img_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_IMAGE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {bg_color}
  m_RaycastTarget: 1
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_Sprite: {{fileID: 0}}
  m_Type: 0
  m_PreserveAspect: 0
  m_FillCenter: 1
  m_FillMethod: 4
  m_FillAmount: 1
  m_FillClockwise: 1
  m_FillOrigin: 0
  m_UseSpriteMesh: 0
  m_PixelsPerUnitMultiplier: 1
--- !u!114 &{btn_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_BUTTON}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Navigation:
    m_Mode: 3
    m_WrapAround: 0
    m_SelectOnUp: {{fileID: 0}}
    m_SelectOnDown: {{fileID: 0}}
    m_SelectOnLeft: {{fileID: 0}}
    m_SelectOnRight: {{fileID: 0}}
  m_Transition: 1
  m_Colors:
    m_NormalColor: {{r: 1, g: 1, b: 1, a: 1}}
    m_HighlightedColor: {{r: 0.95, g: 0.95, b: 0.95, a: 1}}
    m_PressedColor: {{r: 0.75, g: 0.75, b: 0.75, a: 1}}
    m_SelectedColor: {{r: 1, g: 1, b: 1, a: 1}}
    m_DisabledColor: {{r: 0.5, g: 0.5, b: 0.5, a: 0.5}}
    m_ColorMultiplier: 1
    m_FadeDuration: 0.1
  m_SpriteState:
    m_HighlightedSprite: {{fileID: 0}}
    m_PressedSprite: {{fileID: 0}}
    m_SelectedSprite: {{fileID: 0}}
    m_DisabledSprite: {{fileID: 0}}
  m_AnimationTriggers:
    m_NormalTrigger: Normal
    m_HighlightedTrigger: Highlighted
    m_PressedTrigger: Pressed
    m_SelectedTrigger: Selected
    m_DisabledTrigger: Disabled
  m_Interactable: 1
  m_TargetGraphic: {{fileID: {img_id}}}
  m_OnClick:
    m_PersistentCalls:
      m_Calls:
{calls_str}
--- !u!1 &{txt_go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {txt_rt_id}}}
  - component: {{fileID: {txt_cr_id}}}
  - component: {{fileID: {txt_comp_id}}}
  m_Layer: 5
  m_Name: Text
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &{txt_rt_id}
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {txt_go_id}}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {rt_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &{txt_cr_id}
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {txt_go_id}}}
  m_CullTransparentMesh: 1
--- !u!114 &{txt_comp_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {txt_go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_TEXT}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {text_color}
  m_RaycastTarget: 0
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_FontData:
    m_Font: {{fileID: 12800000, guid: {FONT_POPPINS}, type: 3}}
    m_FontSize: {font_size}
    m_FontStyle: 1
    m_BestFit: 0
    m_MinSize: 10
    m_MaxSize: 60
    m_Alignment: 4
    m_AlignByGeometry: 0
    m_RichText: 1
    m_HorizontalOverflow: 0
    m_VerticalOverflow: 0
    m_LineSpacing: 1
  m_Text: "{text_str}"
"""

def generate_settings_panel_yaml(parent_rt_id):
    yaml_blocks = []

    # 1. Root GameObject
    yaml_blocks.append(f"""--- !u!1 &320000100
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000101}}
  - component: {{fileID: 320000102}}
  - component: {{fileID: 320000103}}
  - component: {{fileID: 320000104}}
  m_Layer: 5
  m_Name: SettingsPanel
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 0
--- !u!224 &320000101
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000100}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000111}}
  m_Father: {{fileID: {parent_rt_id}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &320000102
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000100}}
  m_CullTransparentMesh: 1
--- !u!114 &320000103
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000100}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_IMAGE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 0.03, g: 0.04, b: 0.07, a: 0.90}}
  m_RaycastTarget: 1
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_Sprite: {{fileID: 0}}
  m_Type: 0
  m_PreserveAspect: 0
  m_FillCenter: 1
  m_FillMethod: 4
  m_FillAmount: 1
  m_FillClockwise: 1
  m_FillOrigin: 0
  m_UseSpriteMesh: 0
  m_PixelsPerUnitMultiplier: 1
--- !u!114 &320000104
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000100}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_SETTINGS_UI}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  panelRoot: {{fileID: 320000100}}
  closeButton: {{fileID: 320000133}}
  saveButton: {{fileID: 320000714}}
  headerTitleText: {{fileID: 320000128}}
  tabButtons:
  - {{fileID: 320000146}}
  - {{fileID: 320000156}}
  - {{fileID: 320000166}}
  - {{fileID: 320000176}}
  - {{fileID: 320000186}}
  tabContents:
  - {{fileID: 320000210}}
  - {{fileID: 320000310}}
  - {{fileID: 320000410}}
  - {{fileID: 320000510}}
  - {{fileID: 320000610}}
  tabButtonTexts:
  - {{fileID: 320000149}}
  - {{fileID: 320000159}}
  - {{fileID: 320000169}}
  - {{fileID: 320000179}}
  - {{fileID: 320000189}}
  masterSlider: {{fileID: 0}}
  masterValueText: {{fileID: 0}}
  sfxSlider: {{fileID: 0}}
  sfxValueText: {{fileID: 0}}
  bgmSlider: {{fileID: 0}}
  bgmValueText: {{fileID: 0}}
  buzzSlider: {{fileID: 0}}
  buzzValueText: {{fileID: 0}}
  muteGlobalButton: {{fileID: 320000224}}
  muteGlobalStatusText: {{fileID: 320000229}}
  hapticBiteButton: {{fileID: 320000324}}
  hapticBiteStatusText: {{fileID: 320000329}}
  handednessButton: {{fileID: 320000334}}
  handednessStatusText: {{fileID: 320000339}}
  gyroSlider: {{fileID: 0}}
  gyroValueText: {{fileID: 0}}
  reticleStyleButtons: []
  reticleColorButtons: []
  radarToggleButton: {{fileID: 320000444}}
  radarToggleStatusText: {{fileID: 320000449}}
  radarRadiusSlider: {{fileID: 0}}
  radarRadiusValueText: {{fileID: 0}}
  reticlePreviewImage: {{fileID: 320000423}}
  reticleSprites:
  - {{fileID: 21300000, guid: {SPRITE_RETICLE_CLASSIC}, type: 3}}
  - {{fileID: 21300000, guid: {SPRITE_RETICLE_NDOP}, type: 3}}
  - {{fileID: 21300000, guid: {SPRITE_RETICLE_PRECISION}, type: 3}}
  fps30Button: {{fileID: 320000524}}
  fps60Button: {{fileID: 320000534}}
  fpsModeStatusText: {{fileID: 320000539}}
  showFPSButton: {{fileID: 320000544}}
  showFPSStatusText: {{fileID: 320000549}}
  langFRButton: {{fileID: 320000624}}
  langENButton: {{fileID: 320000634}}
  resetScoreButton: {{fileID: 320000644}}
  resetScoreFeedbackText: {{fileID: 320000649}}
  creditsText: {{fileID: 320000658}}
""")

    # 2. ModalWindow Container (1020 x 1780 px - Full Length Exploitation!)
    yaml_blocks.append(f"""--- !u!1 &320000110
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000111}}
  - component: {{fileID: 320000112}}
  - component: {{fileID: 320000113}}
  m_Layer: 5
  m_Name: ModalWindow
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000111
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000110}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000121}}
  - {{fileID: 320000141}}
  - {{fileID: 320000201}}
  - {{fileID: 320000701}}
  m_Father: {{fileID: 320000101}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0.5}}
  m_AnchorMax: {{x: 0.5, y: 0.5}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 1020, y: 1780}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &320000112
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000110}}
  m_CullTransparentMesh: 1
--- !u!114 &320000113
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000110}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_IMAGE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 0.08, g: 0.09, b: 0.13, a: 0.98}}
  m_RaycastTarget: 1
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_Sprite: {{fileID: 0}}
  m_Type: 0
  m_PreserveAspect: 0
  m_FillCenter: 1
  m_FillMethod: 4
  m_FillAmount: 1
  m_FillClockwise: 1
  m_FillOrigin: 0
  m_UseSpriteMesh: 0
  m_PixelsPerUnitMultiplier: 1
""")

    # 3. HeaderBar (Y = -80, Size 980 x 120)
    yaml_blocks.append(f"""--- !u!1 &320000120
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000121}}
  m_Layer: 5
  m_Name: HeaderBar
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000121
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000120}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000126}}
  - {{fileID: 320000130}}
  m_Father: {{fileID: 320000111}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 1}}
  m_AnchorMax: {{x: 0.5, y: 1}}
  m_AnchoredPosition: {{x: 0, y: -80}}
  m_SizeDelta: {{x: 980, y: 120}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!1 &320000125
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000126}}
  - component: {{fileID: 320000127}}
  - component: {{fileID: 320000128}}
  m_Layer: 5
  m_Name: HeaderTitleText
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000126
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000125}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 320000121}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &320000127
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000125}}
  m_CullTransparentMesh: 1
--- !u!114 &320000128
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000125}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_TEXT}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 0.96, g: 0.72, b: 0.18, a: 1}}
  m_RaycastTarget: 0
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_FontData:
    m_Font: {{fileID: 12800000, guid: {FONT_POPPINS}, type: 3}}
    m_FontSize: 48
    m_FontStyle: 1
    m_BestFit: 0
    m_MinSize: 10
    m_MaxSize: 60
    m_Alignment: 4
    m_AlignByGeometry: 0
    m_RichText: 1
    m_HorizontalOverflow: 0
    m_VerticalOverflow: 0
    m_LineSpacing: 1
  m_Text: "PARAMETRES"
""")

    # Close button [X]
    yaml_blocks.append(generate_button(
        go_id=320000129, rt_id=320000130, cr_id=320000131, img_id=320000132, btn_id=320000133,
        txt_go_id=320000134, txt_rt_id=320000135, txt_cr_id=320000136, txt_comp_id=320000137,
        name="CloseButton", father_id=320000121,
        anchor_min="{x: 1, y: 0.5}", anchor_max="{x: 1, y: 0.5}", pos="{x: -60, y: 0}", size="{x: 80, y: 80}",
        bg_color="{r: 0.8, g: 0.2, b: 0.2, a: 1}", text_str="X", text_color="{r: 1, g: 1, b: 1, a: 1}",
        font_size=38, target_id=320000104, target_type="SettingsPanelUI, Assembly-CSharp", method_name="ClosePanel"
    ))

    # 4. TabBar (Y = -190, Size 980 x 90)
    yaml_blocks.append(f"""--- !u!1 &320000140
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000141}}
  m_Layer: 5
  m_Name: TabBar
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000141
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000140}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000143}}
  - {{fileID: 320000153}}
  - {{fileID: 320000163}}
  - {{fileID: 320000173}}
  - {{fileID: 320000183}}
  m_Father: {{fileID: 320000111}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 1}}
  m_AnchorMax: {{x: 0.5, y: 1}}
  m_AnchoredPosition: {{x: 0, y: -190}}
  m_SizeDelta: {{x: 980, y: 90}}
  m_Pivot: {{x: 0.5, y: 0.5}}
""")

    tabs = [
        ("TabBtn_Audio", "Audio", -380, 320000142, 320000143, 320000144, 320000145, 320000146, 320000147, 320000148, 320000149, 320000150),
        ("TabBtn_Controls", "Controles", -190, 320000152, 320000153, 320000154, 320000155, 320000156, 320000157, 320000158, 320000159, 320000160),
        ("TabBtn_Reticle", "Viseur", 0, 320000162, 320000163, 320000164, 320000165, 320000166, 320000167, 320000168, 320000169, 320000170),
        ("TabBtn_Graphics", "Graphismes", 190, 320000172, 320000173, 320000174, 320000175, 320000176, 320000177, 320000178, 320000179, 320000180),
        ("TabBtn_System", "Systeme", 380, 320000182, 320000183, 320000184, 320000185, 320000186, 320000187, 320000188, 320000189, 320000190),
    ]

    for name, title, x_pos, g_id, rt_id, cr_id, img_id, b_id, tg_id, trt_id, tcr_id, tcomp_id in tabs:
        yaml_blocks.append(generate_button(
            go_id=g_id, rt_id=rt_id, cr_id=cr_id, img_id=img_id, btn_id=b_id,
            txt_go_id=tg_id, txt_rt_id=trt_id, txt_cr_id=tcr_id, txt_comp_id=tcomp_id,
            name=name, father_id=320000141,
            anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
            pos=f"{{x: {x_pos}, y: 0}}", size="{x: 180, y: 80}",
            bg_color="{r: 0.15, g: 0.17, b: 0.24, a: 1}", text_str=title,
            text_color="{r: 0.95, g: 0.95, b: 0.95, a: 1}", font_size=28
        ))

    # 5. ContentArea (Spanning Y = 15, Size 980 x 1280 px!)
    yaml_blocks.append(f"""--- !u!1 &320000200
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000201}}
  m_Layer: 5
  m_Name: ContentArea
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000201
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000200}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000211}}
  - {{fileID: 320000311}}
  - {{fileID: 320000411}}
  - {{fileID: 320000511}}
  - {{fileID: 320000611}}
  m_Father: {{fileID: 320000111}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0.5}}
  m_AnchorMax: {{x: 0.5, y: 0.5}}
  m_AnchoredPosition: {{x: 0, y: 15}}
  m_SizeDelta: {{x: 980, y: 1280}}
  m_Pivot: {{x: 0.5, y: 0.5}}
""")

    # 5.1 TabContent_Audio
    yaml_blocks.append(f"""--- !u!1 &320000210
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000211}}
  m_Layer: 5
  m_Name: TabContent_Audio
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000211
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000210}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000221}}
  m_Father: {{fileID: 320000201}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
""")

    # Mute Global button in TabContent_Audio
    yaml_blocks.append(generate_button(
        go_id=320000220, rt_id=320000221, cr_id=320000222, img_id=320000223, btn_id=320000224,
        txt_go_id=320000226, txt_rt_id=320000227, txt_cr_id=320000228, txt_comp_id=320000229,
        name="MuteGlobalButton", father_id=320000211,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: 0, y: 400}", size="{x: 880, y: 110}",
        bg_color="{r: 0.20, g: 0.22, b: 0.32, a: 1}",
        text_str="Couper tout le son : Non",
        text_color="{r: 1, g: 1, b: 1, a: 1}", font_size=32
    ))

    # 5.2 TabContent_Controls
    yaml_blocks.append(f"""--- !u!1 &320000310
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000311}}
  m_Layer: 5
  m_Name: TabContent_Controls
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 0
--- !u!224 &320000311
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000310}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000321}}
  - {{fileID: 320000331}}
  m_Father: {{fileID: 320000201}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
""")
    # Haptic button
    yaml_blocks.append(generate_button(
        go_id=320000320, rt_id=320000321, cr_id=320000322, img_id=320000323, btn_id=320000324,
        txt_go_id=320000326, txt_rt_id=320000327, txt_cr_id=320000328, txt_comp_id=320000329,
        name="HapticBiteButton", father_id=320000311,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: 0, y: 350}", size="{x: 880, y: 110}",
        bg_color="{r: 0.20, g: 0.22, b: 0.32, a: 1}",
        text_str="Vibration Piqures : Activee",
        text_color="{r: 1, g: 1, b: 1, a: 1}", font_size=32
    ))
    # Handedness button
    yaml_blocks.append(generate_button(
        go_id=320000330, rt_id=320000331, cr_id=320000332, img_id=320000333, btn_id=320000334,
        txt_go_id=320000336, txt_rt_id=320000337, txt_cr_id=320000338, txt_comp_id=320000339,
        name="HandednessButton", father_id=320000311,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: 0, y: 200}", size="{x: 880, y: 110}",
        bg_color="{r: 0.20, g: 0.22, b: 0.32, a: 1}",
        text_str="Disposition des tirs : Droitier",
        text_color="{r: 1, g: 1, b: 1, a: 1}", font_size=32
    ))

    # 5.3 TabContent_Reticle
    yaml_blocks.append(f"""--- !u!1 &320000410
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000411}}
  m_Layer: 5
  m_Name: TabContent_Reticle
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 0
--- !u!224 &320000411
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000410}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000421}}
  - {{fileID: 320000441}}
  m_Father: {{fileID: 320000201}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!1 &320000420
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000421}}
  - component: {{fileID: 320000422}}
  - component: {{fileID: 320000423}}
  m_Layer: 5
  m_Name: ReticlePreviewImage
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000421
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000420}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 320000411}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0.5}}
  m_AnchorMax: {{x: 0.5, y: 0.5}}
  m_AnchoredPosition: {{x: 0, y: 250}}
  m_SizeDelta: {{x: 260, y: 260}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &320000422
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000420}}
  m_CullTransparentMesh: 1
--- !u!114 &320000423
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000420}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_IMAGE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 1, g: 1, b: 1, a: 1}}
  m_RaycastTarget: 0
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_Sprite: {{fileID: 21300000, guid: {SPRITE_RETICLE_NDOP}, type: 3}}
  m_Type: 0
  m_PreserveAspect: 1
  m_FillCenter: 1
  m_FillMethod: 4
  m_FillAmount: 1
  m_FillClockwise: 1
  m_FillOrigin: 0
  m_UseSpriteMesh: 0
  m_PixelsPerUnitMultiplier: 1
""")
    # Radar toggle button
    yaml_blocks.append(generate_button(
        go_id=320000440, rt_id=320000441, cr_id=320000442, img_id=320000443, btn_id=320000444,
        txt_go_id=320000446, txt_rt_id=320000447, txt_cr_id=320000448, txt_comp_id=320000449,
        name="RadarToggleButton", father_id=320000411,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: 0, y: 0}", size="{x: 880, y: 110}",
        bg_color="{r: 0.20, g: 0.22, b: 0.32, a: 1}",
        text_str="Fleche Radar 360 : Activee",
        text_color="{r: 1, g: 1, b: 1, a: 1}", font_size=32
    ))

    # 5.4 TabContent_Graphics
    yaml_blocks.append(f"""--- !u!1 &320000510
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000511}}
  m_Layer: 5
  m_Name: TabContent_Graphics
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 0
--- !u!224 &320000511
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000510}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000521}}
  - {{fileID: 320000531}}
  - {{fileID: 320000541}}
  m_Father: {{fileID: 320000201}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
""")
    # 30 FPS Button
    yaml_blocks.append(generate_button(
        go_id=320000520, rt_id=320000521, cr_id=320000522, img_id=320000523, btn_id=320000524,
        txt_go_id=320000526, txt_rt_id=320000527, txt_cr_id=320000528, txt_comp_id=320000529,
        name="FPS30Button", father_id=320000511,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: -230, y: 350}", size="{x: 420, y: 100}",
        bg_color="{r: 0.20, g: 0.22, b: 0.32, a: 1}",
        text_str="30 FPS (Eco)",
        text_color="{r: 1, g: 1, b: 1, a: 1}", font_size=28
    ))
    # 60 FPS Button
    yaml_blocks.append(generate_button(
        go_id=320000530, rt_id=320000531, cr_id=320000532, img_id=320000533, btn_id=320000534,
        txt_go_id=320000536, txt_rt_id=320000537, txt_cr_id=320000538, txt_comp_id=320000539,
        name="FPS60Button", father_id=320000511,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: 230, y: 350}", size="{x: 420, y: 100}",
        bg_color="{r: 0.96, g: 0.72, b: 0.18, a: 1}",
        text_str="60 FPS (Fluide)",
        text_color="{r: 0.1, g: 0.1, b: 0.1, a: 1}", font_size=28
    ))
    # Show FPS Counter button
    yaml_blocks.append(generate_button(
        go_id=320000540, rt_id=320000541, cr_id=320000542, img_id=320000543, btn_id=320000544,
        txt_go_id=320000546, txt_rt_id=320000547, txt_cr_id=320000548, txt_comp_id=320000549,
        name="ShowFPSButton", father_id=320000511,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: 0, y: 180}", size="{x: 880, y: 110}",
        bg_color="{r: 0.20, g: 0.22, b: 0.32, a: 1}",
        text_str="Compteur FPS : Masque",
        text_color="{r: 1, g: 1, b: 1, a: 1}", font_size=32
    ))

    # 5.5 TabContent_System
    yaml_blocks.append(f"""--- !u!1 &320000610
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000611}}
  m_Layer: 5
  m_Name: TabContent_System
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 0
--- !u!224 &320000611
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000610}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000621}}
  - {{fileID: 320000631}}
  - {{fileID: 320000641}}
  - {{fileID: 320000656}}
  m_Father: {{fileID: 320000201}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
""")
    # Lang FR
    yaml_blocks.append(generate_button(
        go_id=320000620, rt_id=320000621, cr_id=320000622, img_id=320000623, btn_id=320000624,
        txt_go_id=320000626, txt_rt_id=320000627, txt_cr_id=320000628, txt_comp_id=320000629,
        name="LangFRButton", father_id=320000611,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: -230, y: 380}", size="{x: 420, y: 100}",
        bg_color="{r: 0.20, g: 0.22, b: 0.32, a: 1}",
        text_str="Francais (FR)",
        text_color="{r: 1, g: 1, b: 1, a: 1}", font_size=30
    ))
    # Lang EN
    yaml_blocks.append(generate_button(
        go_id=320000630, rt_id=320000631, cr_id=320000632, img_id=320000633, btn_id=320000634,
        txt_go_id=320000636, txt_rt_id=320000637, txt_cr_id=320000638, txt_comp_id=320000639,
        name="LangENButton", father_id=320000611,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: 230, y: 380}", size="{x: 420, y: 100}",
        bg_color="{r: 0.20, g: 0.22, b: 0.32, a: 1}",
        text_str="English (EN)",
        text_color="{r: 1, g: 1, b: 1, a: 1}", font_size=30
    ))
    # Reset Score button
    yaml_blocks.append(generate_button(
        go_id=320000640, rt_id=320000641, cr_id=320000642, img_id=320000643, btn_id=320000644,
        txt_go_id=320000646, txt_rt_id=320000647, txt_cr_id=320000648, txt_comp_id=320000649,
        name="ResetScoreButton", father_id=320000611,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: 0, y: 220}", size="{x: 880, y: 110}",
        bg_color="{r: 0.45, g: 0.15, b: 0.15, a: 1}",
        text_str="Reinitialiser le Meilleur Score",
        text_color="{r: 1, g: 0.9, b: 0.9, a: 1}", font_size=30
    ))
    # Credits Text Box
    yaml_blocks.append(f"""--- !u!1 &320000655
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000656}}
  - component: {{fileID: 320000657}}
  - component: {{fileID: 320000658}}
  m_Layer: 5
  m_Name: CreditsText
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000656
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000655}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 320000611}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0.5}}
  m_AnchorMax: {{x: 0.5, y: 0.5}}
  m_AnchoredPosition: {{x: 0, y: -80}}
  m_SizeDelta: {{x: 880, y: 320}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &320000657
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000655}}
  m_CullTransparentMesh: 1
--- !u!114 &320000658
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000655}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_TEXT}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 0.75, g: 0.78, b: 0.85, a: 1}}
  m_RaycastTarget: 0
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_FontData:
    m_Font: {{fileID: 12800000, guid: {FONT_POPPINS}, type: 3}}
    m_FontSize: 26
    m_FontStyle: 0
    m_BestFit: 0
    m_MinSize: 10
    m_MaxSize: 40
    m_Alignment: 4
    m_AlignByGeometry: 0
    m_RichText: 1
    m_HorizontalOverflow: 0
    m_VerticalOverflow: 0
    m_LineSpacing: 1.2
  m_Text: "Developpeur : Camatodev\\nEditeur : Vortex Makers x K-Mer Play\\nMoteur : Unity 2022.3 LTS Vuforia AR\\nVersion : v1.2"
""")

    # 6. FooterBar (Y = -790, Size 980 x 140)
    yaml_blocks.append(f"""--- !u!1 &320000700
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000701}}
  m_Layer: 5
  m_Name: FooterBar
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000701
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000700}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 320000711}}
  m_Father: {{fileID: 320000111}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0}}
  m_AnchorMax: {{x: 0.5, y: 0}}
  m_AnchoredPosition: {{x: 0, y: 80}}
  m_SizeDelta: {{x: 980, y: 120}}
  m_Pivot: {{x: 0.5, y: 0.5}}
""")

    # Save button
    yaml_blocks.append(generate_button(
        go_id=320000710, rt_id=320000711, cr_id=320000712, img_id=320000713, btn_id=320000714,
        txt_go_id=320000716, txt_rt_id=320000717, txt_cr_id=320000718, txt_comp_id=320000719,
        name="SaveButton", father_id=320000701,
        anchor_min="{x: 0.5, y: 0.5}", anchor_max="{x: 0.5, y: 0.5}",
        pos="{x: 0, y: 0}", size="{x: 880, y: 95}",
        bg_color="{r: 0.96, g: 0.72, b: 0.18, a: 1}",
        text_str="ENREGISTRER & RETOUR",
        text_color="{r: 0.1, g: 0.1, b: 0.1, a: 1}", font_size=32,
        target_id=320000104, target_type="SettingsPanelUI, Assembly-CSharp", method_name="SaveAndClose"
    ))

    return "\n".join(yaml_blocks)

def update_main_menu():
    scene_path = 'Assets/Scenes/MainMenu_Vuforia.unity'
    with open(scene_path, 'r', encoding='utf-8') as sf:
        content = sf.read()

    # 1. Update SettingButton RectTransform (1563026194)
    old_rt_setting = re.search(r'(--- !u!224 &1563026194[\s\S]*?)(?=--- !u!|\Z)', content)
    if old_rt_setting:
        new_rt_setting = """--- !u!224 &1563026194
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1563026193}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 326345399}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
  m_AnchorMin: {x: 0.5, y: 0}
  m_AnchorMax: {x: 0.5, y: 0}
  m_AnchoredPosition: {x: -240, y: 280}
  m_SizeDelta: {x: 220, y: 220}
  m_Pivot: {x: 0.5, y: 0.5}
"""
        content = content.replace(old_rt_setting.group(1), new_rt_setting)

    # 2. Ensure SettingButton Image has PreserveAspect
    old_img_setting = re.search(r'(--- !u!114 &1563026196[\s\S]*?)(?=--- !u!|\Z)', content)
    if old_img_setting:
        block = old_img_setting.group(1)
        if 'm_PreserveAspect: 0' in block:
            block = block.replace('m_PreserveAspect: 0', 'm_PreserveAspect: 1')
            content = content.replace(old_img_setting.group(1), block)

    # 3. Add ScoreButton & FooterText & SettingsPanel to Background children if not present
    old_bg_rt = re.search(r'(--- !u!224 &326345399[\s\S]*?m_Children:[\s\S]*?)(?=  m_Father:)', content)
    if old_bg_rt:
        bg_block = old_bg_rt.group(1)
        for child_id in ['320000021', '320000031', '320000101']:
            if child_id not in bg_block:
                bg_block += f'  - {{fileID: {child_id}}}\n'
        content = content.replace(old_bg_rt.group(1), bg_block)

    # 4. Add settingsPanel to MainMenu script (114 &513843498)
    old_main_menu_script = re.search(r'(--- !u!114 &513843498[\s\S]*?)(?=--- !u!|\Z)', content)
    if old_main_menu_script:
        mm_block = old_main_menu_script.group(1)
        if 'settingsPanel:' not in mm_block:
            mm_block = mm_block.strip() + '\n  settingsPanel: {fileID: 320000104}\n'
            content = content.replace(old_main_menu_script.group(1), mm_block + '\n')

    # 5. Define ScoreButton & FooterText GameObjects
    score_and_footer_yaml = f"""--- !u!1 &320000020
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000021}}
  - component: {{fileID: 320000022}}
  - component: {{fileID: 320000023}}
  - component: {{fileID: 320000024}}
  m_Layer: 5
  m_Name: ScoreButton
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000021
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000020}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 326345399}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0}}
  m_AnchorMax: {{x: 0.5, y: 0}}
  m_AnchoredPosition: {{x: 240, y: 280}}
  m_SizeDelta: {{x: 220, y: 220}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &320000022
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000020}}
  m_CullTransparentMesh: 1
--- !u!114 &320000023
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000020}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_IMAGE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 1, g: 1, b: 1, a: 1}}
  m_RaycastTarget: 1
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_Sprite: {{fileID: 21300000, guid: {SPRITE_SCORE_BADGE}, type: 3}}
  m_Type: 0
  m_PreserveAspect: 1
  m_FillCenter: 1
  m_FillMethod: 4
  m_FillAmount: 1
  m_FillClockwise: 1
  m_FillOrigin: 0
  m_UseSpriteMesh: 0
  m_PixelsPerUnitMultiplier: 1
--- !u!114 &320000024
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000020}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_BUTTON}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Navigation:
    m_Mode: 3
    m_WrapAround: 0
    m_SelectOnUp: {{fileID: 0}}
    m_SelectOnDown: {{fileID: 0}}
    m_SelectOnLeft: {{fileID: 0}}
    m_SelectOnRight: {{fileID: 0}}
  m_Transition: 1
  m_Colors:
    m_NormalColor: {{r: 1, g: 1, b: 1, a: 1}}
    m_HighlightedColor: {{r: 0.95, g: 0.95, b: 0.95, a: 1}}
    m_PressedColor: {{r: 0.78, g: 0.78, b: 0.78, a: 1}}
    m_SelectedColor: {{r: 1, g: 1, b: 1, a: 1}}
    m_DisabledColor: {{r: 0.5, g: 0.5, b: 0.5, a: 0.5}}
    m_ColorMultiplier: 1
    m_FadeDuration: 0.1
  m_SpriteState:
    m_HighlightedSprite: {{fileID: 0}}
    m_PressedSprite: {{fileID: 0}}
    m_SelectedSprite: {{fileID: 0}}
    m_DisabledSprite: {{fileID: 0}}
  m_AnimationTriggers:
    m_NormalTrigger: Normal
    m_HighlightedTrigger: Highlighted
    m_PressedTrigger: Pressed
    m_SelectedTrigger: Selected
    m_DisabledTrigger: Disabled
  m_Interactable: 1
  m_TargetGraphic: {{fileID: 320000023}}
  m_OnClick:
    m_PersistentCalls:
      m_Calls:
      - m_Target: {{fileID: 513843498}}
        m_TargetAssemblyTypeName: MainMenu, Assembly-CSharp
        m_MethodName: Settings
        m_Mode: 1
        m_Arguments:
          m_ObjectArgument: {{fileID: 0}}
          m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine
          m_IntArgument: 0
          m_FloatArgument: 0
          m_StringArgument: 
          m_BoolArgument: 0
        m_CallState: 2
--- !u!1 &320000030
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000031}}
  - component: {{fileID: 320000032}}
  - component: {{fileID: 320000033}}
  m_Layer: 5
  m_Name: FooterText
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000031
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000030}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 326345399}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0}}
  m_AnchorMax: {{x: 0.5, y: 0}}
  m_AnchoredPosition: {{x: 0, y: 70}}
  m_SizeDelta: {{x: 1000, y: 70}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &320000032
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000030}}
  m_CullTransparentMesh: 1
--- !u!114 &320000033
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000030}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_TEXT}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 1, g: 1, b: 1, a: 1}}
  m_RaycastTarget: 0
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_FontData:
    m_Font: {{fileID: 12800000, guid: {FONT_POPPINS}, type: 3}}
    m_FontSize: 24
    m_FontStyle: 1
    m_BestFit: 0
    m_MinSize: 10
    m_MaxSize: 40
    m_Alignment: 4
    m_AlignByGeometry: 0
    m_RichText: 1
    m_HorizontalOverflow: 0
    m_VerticalOverflow: 0
    m_LineSpacing: 1
  m_Text: "<color=#F5B82E>VORTEX MAKERS x K-MER PLAY</color>   -   <color=#BBBBBB>v1.2 Vuforia AR</color>"
"""
    if '320000020' not in content:
        content += '\n' + score_and_footer_yaml

    # 6. Append SettingsPanel
    if '320000100' not in content:
        content += '\n' + generate_settings_panel_yaml(parent_rt_id=326345399)

    with open(scene_path, 'w', encoding='utf-8') as sf:
        sf.write(content)

    print("MainMenu_Vuforia.unity successfully updated!")

def update_level01():
    scene_path = 'Assets/Scenes/Level01_Vuforia.unity'
    with open(scene_path, 'r', encoding='utf-8') as sf:
        content = sf.read()

    # 1. Add ButtonSettingsPause to PauseBackground children (1802279902)
    old_pause_bg_rt = re.search(r'(--- !u!224 &1802279902[\s\S]*?m_Children:[\s\S]*?)(?=  m_Father:)', content)
    if old_pause_bg_rt:
        pause_block = old_pause_bg_rt.group(1)
        if '320000911' not in pause_block:
            pause_block += '  - {fileID: 320000911}\n'
            content = content.replace(old_pause_bg_rt.group(1), pause_block)

    # 2. Add SettingsPanel to MainCanvas children (89398172)
    old_main_canvas_rt = re.search(r'(--- !u!224 &89398172[\s\S]*?m_Children:[\s\S]*?)(?=  m_Father:)', content)
    if old_main_canvas_rt:
        canvas_block = old_main_canvas_rt.group(1)
        if '320000101' not in canvas_block:
            canvas_block += '  - {fileID: 320000101}\n'
            content = content.replace(old_main_canvas_rt.group(1), canvas_block)

    # 3. Add settingsPanel to GameManager script (114 &1112846318)
    old_gm_script = re.search(r'(--- !u!114 &1112846318[\s\S]*?)(?=--- !u!|\Z)', content)
    if old_gm_script:
        gm_block = old_gm_script.group(1)
        if 'settingsPanel:' not in gm_block:
            gm_block = gm_block.strip() + '\n  settingsPanel: {fileID: 320000104}\n'
            content = content.replace(old_gm_script.group(1), gm_block + '\n')

    # 4. Define ButtonSettingsPause
    pause_settings_btn_yaml = f"""--- !u!1 &320000910
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 320000911}}
  - component: {{fileID: 320000912}}
  - component: {{fileID: 320000913}}
  - component: {{fileID: 320000914}}
  m_Layer: 5
  m_Name: ButtonSettingsPause
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &320000911
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000910}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 1802279902}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0.5}}
  m_AnchorMax: {{x: 0.5, y: 0.5}}
  m_AnchoredPosition: {{x: 0, y: -340}}
  m_SizeDelta: {{x: 200, y: 200}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &320000912
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000910}}
  m_CullTransparentMesh: 1
--- !u!114 &320000913
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000910}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_IMAGE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 1, g: 1, b: 1, a: 1}}
  m_RaycastTarget: 1
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_Sprite: {{fileID: 21300000, guid: {SPRITE_COG}, type: 3}}
  m_Type: 0
  m_PreserveAspect: 1
  m_FillCenter: 1
  m_FillMethod: 4
  m_FillAmount: 1
  m_FillClockwise: 1
  m_FillOrigin: 0
  m_UseSpriteMesh: 0
  m_PixelsPerUnitMultiplier: 1
--- !u!114 &320000914
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 320000910}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_BUTTON}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Navigation:
    m_Mode: 3
    m_WrapAround: 0
    m_SelectOnUp: {{fileID: 0}}
    m_SelectOnDown: {{fileID: 0}}
    m_SelectOnLeft: {{fileID: 0}}
    m_SelectOnRight: {{fileID: 0}}
  m_Transition: 1
  m_Colors:
    m_NormalColor: {{r: 1, g: 1, b: 1, a: 1}}
    m_HighlightedColor: {{r: 0.95, g: 0.95, b: 0.95, a: 1}}
    m_PressedColor: {{r: 0.78, g: 0.78, b: 0.78, a: 1}}
    m_SelectedColor: {{r: 1, g: 1, b: 1, a: 1}}
    m_DisabledColor: {{r: 0.5, g: 0.5, b: 0.5, a: 0.5}}
    m_ColorMultiplier: 1
    m_FadeDuration: 0.1
  m_SpriteState:
    m_HighlightedSprite: {{fileID: 0}}
    m_PressedSprite: {{fileID: 0}}
    m_SelectedSprite: {{fileID: 0}}
    m_DisabledSprite: {{fileID: 0}}
  m_AnimationTriggers:
    m_NormalTrigger: Normal
    m_HighlightedTrigger: Highlighted
    m_PressedTrigger: Pressed
    m_SelectedTrigger: Selected
    m_DisabledTrigger: Disabled
  m_Interactable: 1
  m_TargetGraphic: {{fileID: 320000913}}
  m_OnClick:
    m_PersistentCalls:
      m_Calls:
      - m_Target: {{fileID: 1112846318}}
        m_TargetAssemblyTypeName: GameManager, Assembly-CSharp
        m_MethodName: OpenSettings
        m_Mode: 1
        m_Arguments:
          m_ObjectArgument: {{fileID: 0}}
          m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine
          m_IntArgument: 0
          m_FloatArgument: 0
          m_StringArgument: 
          m_BoolArgument: 0
        m_CallState: 2
"""
    if '320000910' not in content:
        content += '\n' + pause_settings_btn_yaml

    # 5. Append SettingsPanel to Level01_Vuforia
    if '320000100' not in content:
        content += '\n' + generate_settings_panel_yaml(parent_rt_id=89398172)

    with open(scene_path, 'w', encoding='utf-8') as sf:
        sf.write(content)

    print("Level01_Vuforia.unity successfully updated!")

if __name__ == '__main__':
    update_main_menu()
    update_level01()
    print("All scene updates completed successfully!")
