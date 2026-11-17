import re

SCENE_PATH = "Assets/Scenes/Level01_Vuforia.unity"

GUID_IMAGE = "fe87c0e1cc204ed48ad3b37840f39efc"
GUID_TEXT = "5f7201a12d95ffc409449d95f23cf332"
GUID_RETICLE_HUD = "b2222222222222222222222222222202"
GUID_FPS_COUNTER = "b3333333333333333333333333333303"

FONT_BOLD = "15139204a59f5384498cc7c6c8d901b7"
SPRITE_PILL_ROUNDED = "a4040404040404040404040404040404"
SPRITE_CARD_BORDER = "a3030303030303030303030303030303"
SPRITE_RETICLE_CLASSIC = "fbfbf4e278b075544975b8a7cfc45f65"
SPRITE_RETICLE_NDOP = "612ea7f093f791742b7f2243d0378fda"
SPRITE_RETICLE_PRECISION = "6c899d4b78ffb9147be6d932c03ccadc"


def strip_range(content, lo, hi):
    parts = re.split(r'(?=^--- !u!)', content, flags=re.MULTILINE)
    kept = []
    removed = 0
    for p in parts:
        m = re.match(r'^--- !u!\d+ &(\d+)', p)
        if m and lo <= int(m.group(1)) < hi:
            removed += 1
            continue
        kept.append(p)
    res = "".join(kept)
    if not res.endswith("\n"):
        res += "\n"
    return res, removed


def run():
    with open(SCENE_PATH, "r", encoding="utf-8") as f:
        content = f.read()

    # Clean any previous run in [322000000, 323000000)
    content, removed = strip_range(content, 322000000, 323000000)

    # 1. Wire flyingMosquitoAudioSource on PlayerActions (114 &968145033)
    pa_m = re.search(r'(--- !u!114 &968145033[\s\S]*?)(?=^--- !u!|\Z)', content, flags=re.MULTILINE)
    if pa_m:
        pa_block = pa_m.group(1)
        pa_block = pa_block.replace("flyingMosquitoAudioSource: {fileID: 0}", "flyingMosquitoAudioSource: {fileID: 968145041}")
        content = content.replace(pa_m.group(1), pa_block, 1)

    # 2. Update Focus Image (114 &1860464565): PreserveAspect=1, RaycastTarget=0
    focus_img_m = re.search(r'(--- !u!114 &1860464565[\s\S]*?)(?=^--- !u!|\Z)', content, flags=re.MULTILINE)
    if focus_img_m:
        fi_block = focus_img_m.group(1)
        fi_block = fi_block.replace("m_PreserveAspect: 0", "m_PreserveAspect: 1")
        fi_block = fi_block.replace("m_RaycastTarget: 1", "m_RaycastTarget: 0")
        content = content.replace(focus_img_m.group(1), fi_block, 1)

    # 3. Attach ReticleHUDController (fileID: 322000100) to Focus GameObject (1 &1860464563)
    focus_go_m = re.search(r'(--- !u!1 &1860464563[\s\S]*?m_Component:[\s\S]*?)(?=  m_Layer:)', content)
    if focus_go_m:
        fg_block = focus_go_m.group(1)
        if "322000100" not in fg_block:
            fg_block += "  - component: {fileID: 322000100}\n"
            content = content.replace(focus_go_m.group(1), fg_block, 1)

    # 4. Insert FPSCounterBadge (RT: 322000201) into MainCanvas (&89398172) BEFORE PauseBackground / SettingsPanel
    mc_rt_m = re.search(r'(--- !u!224 &89398172[\s\S]*?m_Children:[\s\S]*?)(?=  m_Father:)', content)
    if mc_rt_m:
        mc_block = mc_rt_m.group(1)
        if "322000201" not in mc_block:
            if "  - {fileID: 1802279902}\n" in mc_block:
                mc_block = mc_block.replace("  - {fileID: 1802279902}\n", "  - {fileID: 322000201}\n  - {fileID: 1802279902}\n", 1)
            else:
                mc_block += "  - {fileID: 322000201}\n"
            content = content.replace(mc_rt_m.group(1), mc_block, 1)

    # 5. Append ReticleHUDController + FPSCounterBadge YAML blocks
    new_blocks = f"""--- !u!114 &322000100
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 1860464563}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_RETICLE_HUD}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  targetImage: {{fileID: 1860464565}}
  fallbackReticleSprites:
  - {{fileID: 21300000, guid: {SPRITE_RETICLE_CLASSIC}, type: 3}}
  - {{fileID: 21300000, guid: {SPRITE_RETICLE_NDOP}, type: 3}}
  - {{fileID: 21300000, guid: {SPRITE_RETICLE_PRECISION}, type: 3}}
--- !u!1 &322000200
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 322000201}}
  - component: {{fileID: 322000202}}
  m_Layer: 5
  m_Name: FPSCounterBadge
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &322000201
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000200}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 322000204}}
  m_Father: {{fileID: 89398172}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 1, y: 1}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: -168, y: -42}}
  m_SizeDelta: {{x: 192, y: 48}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!114 &322000202
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000200}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_FPS_COUNTER}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  visualContainer: {{fileID: 322000203}}
  fpsText: {{fileID: 322000213}}
  updateInterval: 0.25
--- !u!1 &322000203
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 322000204}}
  - component: {{fileID: 322000205}}
  - component: {{fileID: 322000206}}
  m_Layer: 5
  m_Name: Visual
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 0
--- !u!224 &322000204
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000203}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 322000208}}
  - {{fileID: 322000212}}
  m_Father: {{fileID: 322000201}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &322000205
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000203}}
  m_CullTransparentMesh: 1
--- !u!114 &322000206
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000203}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_IMAGE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 0.0784, g: 0.1059, b: 0.0706, a: 0.88}}
  m_RaycastTarget: 0
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_Sprite: {{fileID: 21300000, guid: {SPRITE_PILL_ROUNDED}, type: 3}}
  m_Type: 1
  m_PreserveAspect: 0
  m_FillCenter: 1
  m_FillMethod: 4
  m_FillAmount: 1
  m_FillClockwise: 1
  m_FillOrigin: 0
  m_UseSpriteMesh: 0
  m_PixelsPerUnitMultiplier: 1
--- !u!1 &322000207
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 322000208}}
  - component: {{fileID: 322000209}}
  - component: {{fileID: 322000210}}
  m_Layer: 5
  m_Name: Border
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &322000208
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000207}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 322000204}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: 0, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!222 &322000209
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000207}}
  m_CullTransparentMesh: 1
--- !u!114 &322000210
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000207}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_IMAGE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 0.9608, g: 0.7216, b: 0.1804, a: 0.75}}
  m_RaycastTarget: 0
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_Sprite: {{fileID: 21300000, guid: {SPRITE_CARD_BORDER}, type: 3}}
  m_Type: 1
  m_PreserveAspect: 0
  m_FillCenter: 1
  m_FillMethod: 4
  m_FillAmount: 1
  m_FillClockwise: 1
  m_FillOrigin: 0
  m_UseSpriteMesh: 0
  m_PixelsPerUnitMultiplier: 1
--- !u!1 &322000211
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 322000212}}
  - component: {{fileID: 322000214}}
  - component: {{fileID: 322000213}}
  m_Layer: 5
  m_Name: FPSText
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!224 &322000212
RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000211}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 322000204}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0, y: 0}}
  m_AnchorMax: {{x: 1, y: 1}}
  m_AnchoredPosition: {{x: 0, y: 0}}
  m_SizeDelta: {{x: -16, y: 0}}
  m_Pivot: {{x: 0.5, y: 0.5}}
--- !u!114 &322000213
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000211}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_TEXT}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {{r: 0.9608, g: 0.7216, b: 0.1804, a: 1}}
  m_RaycastTarget: 0
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_FontData:
    m_Font: {{fileID: 12800000, guid: {FONT_BOLD}, type: 3}}
    m_FontSize: 24
    m_FontStyle: 0
    m_BestFit: 0
    m_MinSize: 12
    m_MaxSize: 40
    m_Alignment: 4
    m_AlignByGeometry: 0
    m_RichText: 0
    m_HorizontalOverflow: 0
    m_VerticalOverflow: 1
    m_LineSpacing: 1
  m_Text: "60 FPS"
--- !u!222 &322000214
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 322000211}}
  m_CullTransparentMesh: 1
"""

    content += new_blocks

    # Verify zero duplicate fileIDs and zero missing references for new blocks
    all_ids = re.findall(r'^--- !u!\d+ &(\d+)', content, flags=re.MULTILINE)
    if len(all_ids) != len(set(all_ids)):
        dups = [x for x in set(all_ids) if all_ids.count(x) > 1]
        raise RuntimeError(f"Duplicate fileIDs in {SCENE_PATH}: {dups}")

    defined = set(all_ids)
    new_refs = re.findall(r'\{fileID: (\d+)\}', new_blocks)
    missing = [r for r in set(new_refs) if r != '0' and r not in defined]
    if missing:
        raise RuntimeError(f"Missing fileID refs in {SCENE_PATH}: {missing}")

    with open(SCENE_PATH, "w", encoding="utf-8", newline="\n") as f:
        f.write(content)

    print(f"[OK] {SCENE_PATH} updated! Removed {removed} old blocks, added ReticleHUDController + FPSCounterBadge, wired flyingMosquitoAudioSource. Total YAML objects: {len(all_ids)}, 0 duplicates, 0 missing refs.")


if __name__ == "__main__":
    run()
