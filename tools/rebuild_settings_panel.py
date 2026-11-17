import re
import os

# Unity Built-in UGUI Script GUIDs
GUID_IMAGE = "fe87c0e1cc204ed48ad3b37840f39efc"
GUID_BUTTON = "4e29b1a8efbd4b44bb3f3716e73f07ff"
GUID_TEXT = "5f7201a12d95ffc409449d95f23cf332"
GUID_SLIDER = "67db9e8f0e2ae9c40bc1e2b64352a6b4"
GUID_OUTLINE = "cfabb0440166ab443bba8876756fdfa9"
GUID_INPUTFIELD = "d199490a83bb2b844b9695cbf13b01ef"
GUID_SETTINGS_UI = "6b3c4d5e6f7a8b9c0d1e2f3a4b5c6d7e"

# Fonts (Upright Poppins family)
FONT_BOLD = "15139204a59f5384498cc7c6c8d901b7"      # Poppins-Bold.ttf
FONT_SEMIBOLD = "a1eec4554aa10f647abb338d0a1f54e2"  # Poppins-SemiBold.ttf
FONT_REGULAR = "1614353c3d6457d4b928e28e33afc82f"   # Poppins-Regular.ttf

# Existing Sprites
SPRITE_COG = "f0f739589437a8a4ab43b4c1feed2d91"
SPRITE_RETICLE_CLASSIC = "5047fb28b20d59e41a03af6c60a9650d"
SPRITE_RETICLE_NDOP = "612ea7f093f791742b7f2243d0378fda"
SPRITE_RETICLE_PRECISION = "532b0a0fd08f4d045af5099b86a61466"

# Generated Crisp UI Sprites (in Assets/Ressources/UI/Refont UI/Generated/)
SPRITE_NDOP_RIBBON = "a1010101010101010101010101010101"
SPRITE_CARD_ROUNDED = "a2020202020202020202020202020202"
SPRITE_CARD_BORDER = "a3030303030303030303030303030303"
SPRITE_PILL_ROUNDED = "a4040404040404040404040404040404"
SPRITE_CIRCLE = "a5050505050505050505050505050505"

# Palette (RGBA floats)
C_BACKDROP = (0.03, 0.04, 0.03, 0.92)
C_MODAL_BG = (0.071, 0.094, 0.063, 0.99)      # #121810
C_MODAL_BORDER = (0.60, 0.455, 0.118, 0.85)   # #99741E
C_CARD_BG = (0.110, 0.149, 0.098, 0.98)       # #1C2619
C_CARD_BORDER = (0.282, 0.400, 0.220, 0.90)   # #486638
C_INNER_DARK = (0.078, 0.106, 0.071, 1.0)     # #141B12
C_NOTE_BG = (0.145, 0.165, 0.102, 0.95)       # #252A1A
C_GOLD = (0.961, 0.722, 0.180, 1.0)           # #F5B82E
C_GOLD_LIGHT = (1.0, 0.835, 0.420, 1.0)       # #FFD56B
C_TERRACOTTA = (0.580, 0.282, 0.153, 1.0)     # #944827
C_GREEN_OLIVE = (0.247, 0.365, 0.188, 1.0)    # #3F5D30
C_TEXT_LIGHT = (0.957, 0.929, 0.878, 1.0)     # #F4EDE0
C_TEXT_DIM = (0.722, 0.686, 0.620, 1.0)       # #B8AF9E
C_TEXT_MUTED = (0.48, 0.50, 0.45, 0.85)
C_TEXT_DARK = (0.078, 0.106, 0.071, 1.0)      # #141B12
C_DIVIDER = (0.220, 0.298, 0.184, 0.75)       # #384C2F
C_RED_BG = (0.19, 0.08, 0.07, 1.0)
C_RED_BORDER = (0.839, 0.204, 0.157, 1.0)     # #D63428
C_RED_TEXT = (0.98, 0.44, 0.38, 1.0)
C_SWATCH_GREEN = (0.345, 0.486, 0.298, 1.0)   # #587C4C
C_SWATCH_RED = (0.839, 0.204, 0.157, 1.0)     # #D63428
C_WHITE = (1.0, 1.0, 1.0, 1.0)


def fmt_color(c):
    return f"{{r: {c[0]:.4f}, g: {c[1]:.4f}, b: {c[2]:.4f}, a: {c[3]:.4f}}}"


def yaml_escape(s):
    return s.replace("\\", "\\\\").replace('"', '\\"').replace("\n", "\\n")


class UGuiBuilder:
    def __init__(self, start_id=321000000):
        self.next_id = start_id
        self.blocks = []
        self.children_map = {}  # rt_id -> list of child rt_ids

    def alloc(self):
        val = self.next_id
        self.next_id += 1
        return val

    def register_child(self, parent_rt, child_rt):
        if parent_rt not in self.children_map:
            self.children_map[parent_rt] = []
        self.children_map[parent_rt].append(child_rt)

    def add_go(self, name, parent_rt, comp_ids, anchor_min, anchor_max, pos, size, pivot=(0.5, 0.5), active=1, go_id=None, rt_id=None):
        go_id = go_id if go_id is not None else self.alloc()
        rt_id = rt_id if rt_id is not None else self.alloc()
        all_comps = [rt_id] + comp_ids
        comp_lines = "\n".join([f"  - component: {{fileID: {cid}}}" for cid in all_comps])
        self.register_child(parent_rt, rt_id)
        if rt_id not in self.children_map:
            self.children_map[rt_id] = []

        self.blocks.append(("go", f"""--- !u!1 &{go_id}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
{comp_lines}
  m_Layer: 5
  m_Name: {name}
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: {active}"""))

        self.blocks.append(("rt", (rt_id, go_id, parent_rt, anchor_min, anchor_max, pos, size, pivot)))
        return go_id, rt_id

    def add_cr(self, go_id, cr_id=None):
        cr_id = cr_id if cr_id is not None else self.alloc()
        self.blocks.append(("raw", f"""--- !u!222 &{cr_id}
CanvasRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_CullTransparentMesh: 1"""))
        return cr_id

    def add_image(self, go_id, color, sprite_guid=None, img_type=0, preserve_aspect=0, raycast=1, img_id=None):
        img_id = img_id if img_id is not None else self.alloc()
        sprite_ref = f"{{fileID: 21300000, guid: {sprite_guid}, type: 3}}" if sprite_guid else "{fileID: 0}"
        self.blocks.append(("raw", f"""--- !u!114 &{img_id}
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
  m_Color: {fmt_color(color)}
  m_RaycastTarget: {raycast}
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_Sprite: {sprite_ref}
  m_Type: {img_type}
  m_PreserveAspect: {preserve_aspect}
  m_FillCenter: 1
  m_FillMethod: 4
  m_FillAmount: 1
  m_FillClockwise: 1
  m_FillOrigin: 0
  m_UseSpriteMesh: 0
  m_PixelsPerUnitMultiplier: 1"""))
        return img_id

    def add_text(self, go_id, text_str, font_size, color, font_guid=FONT_REGULAR, alignment=4, line_spacing=1.0, raycast=0, rich_text=1, overflow_v=1, txt_id=None):
        # alignment: 3=MiddleLeft, 4=MiddleCenter, 5=MiddleRight, 0=UpperLeft
        txt_id = txt_id if txt_id is not None else self.alloc()
        self.blocks.append(("raw", f"""--- !u!114 &{txt_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_TEXT}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Material: {{fileID: 0}}
  m_Color: {fmt_color(color)}
  m_RaycastTarget: {raycast}
  m_RaycastPadding: {{x: 0, y: 0, z: 0, w: 0}}
  m_Maskable: 1
  m_OnCullStateChanged:
    m_PersistentCalls:
      m_Calls: []
  m_FontData:
    m_Font: {{fileID: 12800000, guid: {font_guid}, type: 3}}
    m_FontSize: {font_size}
    m_FontStyle: 0
    m_BestFit: 0
    m_MinSize: 10
    m_MaxSize: 80
    m_Alignment: {alignment}
    m_AlignByGeometry: 0
    m_RichText: {rich_text}
    m_HorizontalOverflow: 0
    m_VerticalOverflow: {overflow_v}
    m_LineSpacing: {line_spacing}
  m_Text: "{yaml_escape(text_str)}" """.rstrip()))
        return txt_id

    def add_button(self, go_id, target_graphic_id, interactable=1, btn_id=None):
        btn_id = btn_id if btn_id is not None else self.alloc()
        self.blocks.append(("raw", f"""--- !u!114 &{btn_id}
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
    m_HighlightedColor: {{r: 0.96, g: 0.96, b: 0.96, a: 1}}
    m_PressedColor: {{r: 0.82, g: 0.82, b: 0.82, a: 1}}
    m_SelectedColor: {{r: 1, g: 1, b: 1, a: 1}}
    m_DisabledColor: {{r: 0.78, g: 0.78, b: 0.78, a: 0.5}}
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
  m_Interactable: {interactable}
  m_TargetGraphic: {{fileID: {target_graphic_id}}}
  m_OnClick:
    m_PersistentCalls:
      m_Calls: []"""))
        return btn_id

    def add_outline(self, go_id, color, dist=(3.0, -3.0), enabled=1, out_id=None):
        out_id = out_id if out_id is not None else self.alloc()
        self.blocks.append(("raw", f"""--- !u!114 &{out_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: {enabled}
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_OUTLINE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_EffectColor: {fmt_color(color)}
  m_EffectDistance: {{x: {dist[0]}, y: {dist[1]}}}
  m_UseGraphicAlpha: 1"""))
        return out_id

    def add_slider_comp(self, go_id, handle_img_id, fill_rt_id, handle_rt_id, min_v, max_v, val, sld_id=None):
        sld_id = sld_id if sld_id is not None else self.alloc()
        self.blocks.append(("raw", f"""--- !u!114 &{sld_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_SLIDER}, type: 3}}
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
    m_HighlightedColor: {{r: 1, g: 0.95, b: 0.85, a: 1}}
    m_PressedColor: {{r: 0.85, g: 0.75, b: 0.55, a: 1}}
    m_SelectedColor: {{r: 1, g: 1, b: 1, a: 1}}
    m_DisabledColor: {{r: 0.7, g: 0.7, b: 0.7, a: 0.5}}
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
  m_TargetGraphic: {{fileID: {handle_img_id}}}
  m_FillRect: {{fileID: {fill_rt_id}}}
  m_HandleRect: {{fileID: {handle_rt_id}}}
  m_Direction: 0
  m_MinValue: {min_v}
  m_MaxValue: {max_v}
  m_WholeNumbers: 0
  m_Value: {val}
  m_OnValueChanged:
    m_PersistentCalls:
      m_Calls: []"""))
        return sld_id

    def add_inputfield_comp(self, go_id, target_img_id, text_comp_id, placeholder_comp_id, initial_text="Chasseur K-Mer", inp_id=None):
        inp_id = inp_id if inp_id is not None else self.alloc()
        self.blocks.append(("raw", f"""--- !u!114 &{inp_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go_id}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_INPUTFIELD}, type: 3}}
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
    m_HighlightedColor: {{r: 0.96, g: 0.96, b: 0.96, a: 1}}
    m_PressedColor: {{r: 0.85, g: 0.85, b: 0.85, a: 1}}
    m_SelectedColor: {{r: 1, g: 1, b: 1, a: 1}}
    m_DisabledColor: {{r: 0.78, g: 0.78, b: 0.78, a: 0.5}}
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
  m_TargetGraphic: {{fileID: {target_img_id}}}
  m_TextComponent: {{fileID: {text_comp_id}}}
  m_Placeholder: {{fileID: {placeholder_comp_id}}}
  m_ContentType: 0
  m_InputType: 0
  m_AsteriskChar: 42
  m_KeyboardType: 0
  m_LineType: 0
  m_HideMobileInput: 0
  m_CharacterValidation: 0
  m_CharacterLimit: 20
  m_OnSubmit:
    m_PersistentCalls:
      m_Calls: []
  m_OnDidEndEdit:
    m_PersistentCalls:
      m_Calls: []
  m_OnValueChanged:
    m_PersistentCalls:
      m_Calls: []
  m_CaretColor: {{r: 0.961, g: 0.722, b: 0.180, a: 1}}
  m_CustomCaretColor: 1
  m_SelectionColor: {{r: 0.961, g: 0.722, b: 0.180, a: 0.35}}
  m_Text: "{yaml_escape(initial_text)}"
  m_CaretBlinkRate: 0.85
  m_CaretWidth: 2
  m_ReadOnly: 0
  m_ShouldActivateOnSelect: 1"""))
        return inp_id

    # ---------------- HIGH-LEVEL UI WIDGET HELPERS ----------------

    def make_image_node(self, name, parent_rt, anchor_min, anchor_max, pos, size, color, sprite_guid=None, img_type=0, preserve_aspect=0, raycast=0, pivot=(0.5, 0.5)):
        go_id = self.alloc()
        rt_id = self.alloc()
        cr_id = self.alloc()
        img_id = self.alloc()
        self.add_go(name, parent_rt, [cr_id, img_id], anchor_min, anchor_max, pos, size, pivot=pivot, go_id=go_id, rt_id=rt_id)
        self.add_cr(go_id, cr_id)
        self.add_image(go_id, color, sprite_guid=sprite_guid, img_type=img_type, preserve_aspect=preserve_aspect, raycast=raycast, img_id=img_id)
        return go_id, rt_id, img_id

    def make_text_node(self, name, parent_rt, anchor_min, anchor_max, pos, size, text_str, font_size, color, font_guid=FONT_REGULAR, alignment=4, line_spacing=1.0, pivot=(0.5, 0.5), rich_text=1):
        go_id = self.alloc()
        rt_id = self.alloc()
        cr_id = self.alloc()
        txt_id = self.alloc()
        self.add_go(name, parent_rt, [cr_id, txt_id], anchor_min, anchor_max, pos, size, pivot=pivot, go_id=go_id, rt_id=rt_id)
        self.add_cr(go_id, cr_id)
        self.add_text(go_id, text_str, font_size, color, font_guid=font_guid, alignment=alignment, line_spacing=line_spacing, rich_text=rich_text, txt_id=txt_id)
        return go_id, rt_id, txt_id

    def make_mod_card(self, name, parent_rt, y_top, height, tag_text, title_text=None, width=940):
        # Top-anchored card inside parent_rt
        go_id, card_rt, _ = self.make_image_node(
            name, parent_rt, (0.5, 1), (0.5, 1), (0, y_top), (width, height),
            C_CARD_BG, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=0, pivot=(0.5, 1)
        )
        # Crisp rounded border overlay
        self.make_image_node(
            f"{name}_Border", card_rt, (0, 0), (1, 1), (0, 0), (0, 0),
            C_CARD_BORDER, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
        )
        # Top-right terracotta badge tag
        if tag_text:
            tag_w = max(180, len(tag_text) * 14 + 44)
            _, tag_rt, _ = self.make_image_node(
                f"{name}_TagBg", card_rt, (1, 1), (1, 1), (-28, 0), (tag_w, 36),
                C_TERRACOTTA, sprite_guid=SPRITE_PILL_ROUNDED, img_type=1, raycast=0, pivot=(1, 0.5)
            )
            self.make_text_node(
                f"{name}_TagText", tag_rt, (0, 0), (1, 1), (0, 0), (0, 0),
                tag_text, 19, C_WHITE, font_guid=FONT_BOLD, alignment=4
            )
        # Card Title in Gold
        if title_text:
            self.make_text_node(
                f"{name}_Title", card_rt, (0, 1), (1, 1), (0, -52), (-80, 48),
                title_text, 33, C_GOLD, font_guid=FONT_BOLD, alignment=3, pivot=(0.5, 0.5)
            )
        return card_rt

    def make_slider_row(self, name, parent_rt, y_pos, label_text, initial_val_text, min_v, max_v, val, width=860):
        # Row container anchored to top of card
        row_go, row_rt = self.add_go(name, parent_rt, [], (0.5, 1), (0.5, 1), (0, y_pos), (width, 108), pivot=(0.5, 1))

        # Left label
        self.make_text_node(
            f"{name}_Label", row_rt, (0, 1), (0.75, 1), (0, -24), (0, 44),
            label_text, 27, C_TEXT_LIGHT, font_guid=FONT_SEMIBOLD, alignment=3, pivot=(0.5, 0.5)
        )
        # Right value text
        _, _, val_txt_id = self.make_text_node(
            f"{name}_Value", row_rt, (0.70, 1), (1, 1), (0, -24), (0, 44),
            initial_val_text, 27, C_GOLD, font_guid=FONT_BOLD, alignment=5, pivot=(0.5, 0.5)
        )

        # Actual Unity Slider GameObject
        sld_go = self.alloc()
        sld_rt = self.alloc()
        sld_comp = self.alloc()
        self.add_go(f"{name}_Slider", row_rt, [sld_comp], (0.5, 1), (0.5, 1), (0, -76), (width, 40), pivot=(0.5, 0.5), go_id=sld_go, rt_id=sld_rt)

        # Slider Background track
        self.make_image_node(
            "Background", sld_rt, (0, 0.5), (1, 0.5), (0, 0), (0, 18),
            C_INNER_DARK, sprite_guid=SPRITE_PILL_ROUNDED, img_type=1, raycast=1
        )

        # Fill Area
        _, fill_area_rt = self.add_go("Fill Area", sld_rt, [], (0, 0.5), (1, 0.5), (-6, 0), (-12, 18))
        # Fill Image (Gold)
        _, fill_rt, _ = self.make_image_node(
            "Fill", fill_area_rt, (0, 0), (0.8, 1), (0, 0), (12, 0),
            C_GOLD, sprite_guid=SPRITE_PILL_ROUNDED, img_type=1, raycast=0
        )

        # Handle Slide Area
        _, handle_area_rt = self.add_go("Handle Slide Area", sld_rt, [], (0, 0), (1, 1), (0, 0), (-36, 0))
        # Handle Circle Knob
        _, handle_rt, handle_img_id = self.make_image_node(
            "Handle", handle_area_rt, (0.8, 0.5), (0.8, 0.5), (0, 0), (38, 38),
            C_GOLD_LIGHT, sprite_guid=SPRITE_CIRCLE, img_type=0, raycast=1
        )

        self.add_slider_comp(sld_go, handle_img_id, fill_rt, handle_rt, min_v, max_v, val, sld_id=sld_comp)
        return sld_comp, val_txt_id

    def make_toggle_row(self, name, parent_rt, y_pos, title_text, subtitle_text, is_on=True, interactable=True, width=860):
        row_go, row_rt = self.add_go(name, parent_rt, [], (0.5, 1), (0.5, 1), (0, y_pos), (width, 100), pivot=(0.5, 1))

        t_col = C_TEXT_LIGHT if interactable else C_TEXT_MUTED
        s_col = C_TEXT_DIM if interactable else (0.42, 0.45, 0.40, 0.75)

        self.make_text_node(
            f"{name}_Title", row_rt, (0, 1), (0.82, 1), (0, -28), (0, 42),
            title_text, 27, t_col, font_guid=FONT_BOLD, alignment=3
        )
        self.make_text_node(
            f"{name}_Subtitle", row_rt, (0, 1), (0.82, 1), (0, -68), (0, 36),
            subtitle_text, 22, s_col, font_guid=FONT_REGULAR, alignment=3
        )

        # Toggle Switch Button (104 x 52 pill)
        btn_go = self.alloc()
        btn_rt = self.alloc()
        btn_cr = self.alloc()
        btn_img = self.alloc()
        btn_comp = self.alloc()
        self.add_go(f"{name}_SwitchBtn", row_rt, [btn_cr, btn_img, btn_comp], (1, 0.5), (1, 0.5), (-54, 0), (104, 52), go_id=btn_go, rt_id=btn_rt)
        self.add_cr(btn_go, btn_cr)
        bg_col = C_GREEN_OLIVE if is_on else C_INNER_DARK
        self.add_image(btn_go, bg_col, sprite_guid=SPRITE_PILL_ROUNDED, img_type=1, raycast=1, img_id=btn_img)
        self.add_button(btn_go, btn_img, interactable=(1 if interactable else 0), btn_id=btn_comp)

        # Border on switch capsule
        self.make_image_node(
            f"{name}_SwitchBorder", btn_rt, (0, 0), (1, 1), (0, 0), (0, 0),
            (0.961, 0.722, 0.180, 0.7) if is_on else (0.28, 0.40, 0.22, 0.6),
            sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
        )

        # Knob Circle (40x40)
        knob_x = 24 if is_on else -24
        knob_col = C_WHITE if is_on else (0.55, 0.48, 0.35, 1.0)
        _, knob_rt, _ = self.make_image_node(
            f"{name}_Knob", btn_rt, (0.5, 0.5), (0.5, 0.5), (knob_x, 0), (40, 40),
            knob_col, sprite_guid=SPRITE_CIRCLE, img_type=0, raycast=0
        )
        return btn_comp, btn_img, knob_rt

    def make_pill_button(self, name, parent_rt, anchor_min, anchor_max, pos, size, bg_color, text_str, text_color, font_size, font_guid=FONT_BOLD, sprite_guid=SPRITE_PILL_ROUNDED, img_type=1, border_color=None, pivot=(0.5, 0.5)):
        go_id = self.alloc()
        rt_id = self.alloc()
        cr_id = self.alloc()
        img_id = self.alloc()
        btn_id = self.alloc()
        self.add_go(name, parent_rt, [cr_id, img_id, btn_id], anchor_min, anchor_max, pos, size, pivot=pivot, go_id=go_id, rt_id=rt_id)
        self.add_cr(go_id, cr_id)
        self.add_image(go_id, bg_color, sprite_guid=sprite_guid, img_type=img_type, raycast=1, img_id=img_id)
        self.add_button(go_id, img_id, interactable=1, btn_id=btn_id)
        if border_color is not None:
            self.make_image_node(
                f"{name}_Border", rt_id, (0, 0), (1, 1), (0, 0), (0, 0),
                border_color, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
            )
        _, _, txt_id = self.make_text_node(
            f"{name}_Text", rt_id, (0, 0), (1, 1), (0, 0), (-16, 0),
            text_str, font_size, text_color, font_guid=font_guid, alignment=4
        )
        return go_id, rt_id, btn_id, txt_id

    def make_segmented_pair(self, name, parent_rt, y_pos, left_label, right_label, left_active=True, width=860, height=82):
        # Container track
        _, cont_rt, _ = self.make_image_node(
            f"{name}_Track", parent_rt, (0.5, 1), (0.5, 1), (0, y_pos), (width, height),
            C_INNER_DARK, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=0, pivot=(0.5, 1)
        )
        self.make_image_node(
            f"{name}_TrackBorder", cont_rt, (0, 0), (1, 1), (0, 0), (0, 0),
            C_CARD_BORDER, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
        )
        btn_w = (width - 20) // 2
        btn_h = height - 14
        offset = btn_w // 2 + 3

        _, _, left_btn, _ = self.make_pill_button(
            f"{name}_LeftBtn", cont_rt, (0.5, 0.5), (0.5, 0.5), (-offset, 0), (btn_w, btn_h),
            C_GOLD if left_active else C_INNER_DARK,
            left_label,
            C_TEXT_DARK if left_active else C_TEXT_LIGHT,
            25, font_guid=FONT_BOLD, sprite_guid=SPRITE_CARD_ROUNDED
        )
        _, _, right_btn, _ = self.make_pill_button(
            f"{name}_RightBtn", cont_rt, (0.5, 0.5), (0.5, 0.5), (offset, 0), (btn_w, btn_h),
            C_GOLD if not left_active else C_INNER_DARK,
            right_label,
            C_TEXT_DARK if not left_active else C_TEXT_LIGHT,
            25, font_guid=FONT_BOLD, sprite_guid=SPRITE_CARD_ROUNDED
        )
        return left_btn, right_btn

    def make_note_box(self, name, parent_rt, y_top, height, text_str, width=940):
        _, box_rt, _ = self.make_image_node(
            name, parent_rt, (0.5, 1), (0.5, 1), (0, y_top), (width, height),
            C_NOTE_BG, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=0, pivot=(0.5, 1)
        )
        # Golden left accent bar
        self.make_image_node(
            f"{name}_Bar", box_rt, (0, 0), (0, 1), (6, 0), (8, -24),
            C_GOLD, sprite_guid=None, img_type=0, raycast=0, pivot=(0, 0.5)
        )
        self.make_text_node(
            f"{name}_Text", box_rt, (0, 0), (1, 1), (14, 0), (-56, -24),
            text_str, 23, (0.83, 0.80, 0.72, 1.0), font_guid=FONT_REGULAR, alignment=3, line_spacing=1.22
        )

    def render_yaml(self):
        out = []
        for kind, payload in self.blocks:
            if kind in ("go", "raw"):
                out.append(payload)
            elif kind == "rt":
                rt_id, go_id, parent_rt, amin, amax, pos, size, pivot = payload
                children = self.children_map.get(rt_id, [])
                if children:
                    ch_lines = "  m_Children:\n" + "\n".join([f"  - {{fileID: {c}}}" for c in children])
                else:
                    ch_lines = "  m_Children: []"
                out.append(f"""--- !u!224 &{rt_id}
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
{ch_lines}
  m_Father: {{fileID: {parent_rt}}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: {amin[0]}, y: {amin[1]}}}
  m_AnchorMax: {{x: {amax[0]}, y: {amax[1]}}}
  m_AnchoredPosition: {{x: {pos[0]}, y: {pos[1]}}}
  m_SizeDelta: {{x: {size[0]}, y: {size[1]}}}
  m_Pivot: {{x: {pivot[0]}, y: {pivot[1]}}}""")
        return "\n".join(out) + "\n"


def build_settings_panel_yaml(parent_rt_id):
    b = UGuiBuilder(start_id=321000000)

    # Fixed root IDs so MainMenu & Level01 references stay 100% intact!
    ROOT_GO = 320000100
    ROOT_RT = 320000101
    ROOT_CR = 320000102
    ROOT_IMG = 320000103
    ROOT_SCRIPT = 320000104

    b.add_go("SettingsPanel", parent_rt_id, [ROOT_CR, ROOT_IMG, ROOT_SCRIPT], (0, 0), (1, 1), (0, 0), (0, 0), active=0, go_id=ROOT_GO, rt_id=ROOT_RT)
    b.add_cr(ROOT_GO, ROOT_CR)
    b.add_image(ROOT_GO, C_BACKDROP, sprite_guid=None, raycast=1, img_id=ROOT_IMG)

    # Reserve placeholder index for SettingsPanelUI MonoBehaviour (we fill all IDs after building children)
    script_placeholder_idx = len(b.blocks)
    b.blocks.append(("raw", "PLACEHOLDER_SETTINGS_UI"))

    # ---------------- MODAL WINDOW (1020 x 1780) ----------------
    _, modal_rt, _ = b.make_image_node(
        "ModalWindow", ROOT_RT, (0.5, 0.5), (0.5, 0.5), (0, 0), (1020, 1780),
        C_MODAL_BG, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=1
    )
    b.make_image_node(
        "ModalBorder", modal_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        C_MODAL_BORDER, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
    )

    # Top & Bottom Ndop Braided Ribbons
    b.make_image_node(
        "TopNdopRibbon", modal_rt, (0.5, 1), (0.5, 1), (0, -18), (1020, 36),
        C_WHITE, sprite_guid=SPRITE_NDOP_RIBBON, img_type=0, raycast=0
    )
    b.make_image_node(
        "BottomNdopRibbon", modal_rt, (0.5, 0), (0.5, 0), (0, 18), (1020, 36),
        C_WHITE, sprite_guid=SPRITE_NDOP_RIBBON, img_type=0, raycast=0
    )

    # ---------------- HEADER ----------------
    b.make_image_node(
        "HeaderCogIcon", modal_rt, (0.5, 1), (0.5, 1), (-436, -96), (62, 62),
        C_WHITE, sprite_guid=SPRITE_COG, img_type=0, preserve_aspect=1, raycast=0
    )
    _, _, header_title_txt = b.make_text_node(
        "HeaderTitle", modal_rt, (0.5, 1), (0.5, 1), (-20, -96), (720, 68),
        "PARAMÈTRES AUDIO", 40, C_GOLD, font_guid=FONT_BOLD, alignment=3
    )
    _, _, close_btn_id, _ = b.make_pill_button(
        "CloseButton", modal_rt, (0.5, 1), (0.5, 1), (434, -96), (76, 76),
        (0.14, 0.19, 0.13, 1.0), "X", C_TEXT_DIM, 34, font_guid=FONT_BOLD,
        sprite_guid=SPRITE_CIRCLE, img_type=0
    )
    b.make_image_node(
        "HeaderDivider", modal_rt, (0.5, 1), (0.5, 1), (0, -152), (950, 2),
        C_DIVIDER, sprite_guid=None, raycast=0
    )

    # ---------------- TAB NAVIGATION BAR (5 Onglets) ----------------
    _, tab_bar_rt, _ = b.make_image_node(
        "TabBarContainer", modal_rt, (0.5, 1), (0.5, 1), (0, -214), (950, 84),
        C_INNER_DARK, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=0
    )
    b.make_image_node(
        "TabBarBorder", tab_bar_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        C_CARD_BORDER, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
    )

    tab_specs = [
        ("TabBtn_Audio", "Audio", -372, True),
        ("TabBtn_Controls", "Contrôles", -186, False),
        ("TabBtn_Reticle", "Viseur", 0, False),
        ("TabBtn_Graphics", "Graphismes", 186, False),
        ("TabBtn_System", "Système", 372, False),
    ]
    tab_btn_ids = []
    tab_txt_ids = []
    for t_name, t_label, tx, is_active in tab_specs:
        _, _, tb_id, tt_id = b.make_pill_button(
            t_name, tab_bar_rt, (0.5, 0.5), (0.5, 0.5), (tx, 0), (178, 66),
            C_GOLD if is_active else C_CARD_BG,
            t_label,
            C_TEXT_DARK if is_active else C_TEXT_LIGHT,
            23, font_guid=FONT_BOLD, sprite_guid=SPRITE_CARD_ROUNDED
        )
        tab_btn_ids.append(tb_id)
        tab_txt_ids.append(tt_id)

    # ---------------- CONTENT CONTAINERS (5 Tabs) ----------------
    tab_content_gos = []
    tab_content_rts = []
    for idx, cname in enumerate(["TabContent_Audio", "TabContent_Controls", "TabContent_Reticle", "TabContent_Graphics", "TabContent_System"]):
        c_go, c_rt = b.add_go(
            cname, modal_rt, [], (0.5, 1), (0.5, 1), (0, -274), (960, 1290),
            pivot=(0.5, 1), active=(1 if idx == 0 else 0)
        )
        tab_content_gos.append(c_go)
        tab_content_rts.append(c_rt)

    # ==========================================================
    # TAB 1 : AUDIO (Capture 105817)
    # ==========================================================
    t1 = tab_content_rts[0]
    card_vol = b.make_mod_card("Card_AudioVolumes", t1, -22, 650, "COMPOSANT MODULAIRE", "Volumes Indépendants")
    master_sld, master_val_txt = b.make_slider_row("Row_Master", card_vol, -112, "Volume Général (Master)", "80%", 0.0, 1.0, 0.80)
    sfx_sld, sfx_val_txt = b.make_slider_row("Row_SFX", card_vol, -242, "Effets Sonores (SFX - Spray & Tirs)", "90%", 0.0, 1.0, 0.90)
    bgm_sld, bgm_val_txt = b.make_slider_row("Row_BGM", card_vol, -372, "Musique d'Ambiance BGM", "65%", 0.0, 1.0, 0.65)
    buzz_sld, buzz_val_txt = b.make_slider_row("Row_Buzz", card_vol, -502, "Bourdonnement Moustiques 3D", "75%", 0.0, 1.0, 0.75)

    card_mute = b.make_mod_card("Card_MuteGlobal", t1, -708, 195, "RACCOURCI", None)
    mute_btn, mute_bg, mute_knob = b.make_toggle_row(
        "Row_MuteGlobal", card_mute, -46,
        "Couper Tout le Son (Mute Global)",
        "Désactive instantanément tous les flux audio",
        is_on=False, interactable=True
    )

    b.make_note_box(
        "Note_Audio", t1, -940, 165,
        "<color=#F5B82E>Validation :</color> Chaque curseur pilote directement son propre groupe audio avec sauvegarde instantanée dans PlayerPrefs."
    )

    # ==========================================================
    # TAB 2 : CONTRÔLES & HAPTIQUE (Capture 105823)
    # ==========================================================
    t2 = tab_content_rts[1]
    card_hap = b.make_mod_card("Card_Haptics", t2, -22, 345, "HAPTIQUE MOBILE", "Retour Tactile / Vibrations")
    hap_bite_btn, hap_bite_bg, hap_bite_knob = b.make_toggle_row(
        "Row_HapticBite", card_hap, -104,
        "Vibration lors d'une Piqûre subie",
        "Secousse haptique quand un moustique pique le joueur",
        is_on=True, interactable=True
    )
    b.make_toggle_row(
        "Row_HapticShootLocked", card_hap, -222,
        "Vibration lors du Tir de Spray",
        "Désactivé selon vos consignes express",
        is_on=False, interactable=False
    )

    card_ergo = b.make_mod_card("Card_Handedness", t2, -402, 230, "ERGONOMIE AR", "Disposition des Commandes")
    hand_right_btn, hand_left_btn = b.make_segmented_pair(
        "Seg_Handedness", card_ergo, -112,
        "Droitier (Tir à droite)", "Gaucher (Tir à gauche)",
        left_active=True
    )

    card_gyro = b.make_mod_card("Card_Gyro", t2, -668, 240, "SUIVI GYROSCOPIQUE", "Sensibilité & Lissage AR")
    gyro_sld, gyro_val_txt = b.make_slider_row("Row_Gyro", card_gyro, -108, "Sensibilité Caméra", "1.0x (Défaut)", 0.2, 2.0, 1.0)

    b.make_note_box(
        "Note_Controls", t2, -944, 165,
        "<color=#F5B82E>Validation :</color> La vibration sur tir est bien retirée. Seule la piqûre déclenche l'impulsion physique pour marquer le choc."
    )

    # ==========================================================
    # TAB 3 : VISEUR & RADAR 360° (Capture 105830)
    # ==========================================================
    t3 = tab_content_rts[2]
    card_ret = b.make_mod_card("Card_Reticle", t3, -22, 600, "PERSONNALISATION", "Choix du Réticule de Visée")

    reticle_specs = [
        ("ReticleStyle_Classic", "Classique", SPRITE_RETICLE_CLASSIC, -292, False),
        ("ReticleStyle_Ndop", "Ndop Guerrier", SPRITE_RETICLE_NDOP, 0, True),
        ("ReticleStyle_Precision", "Précision", SPRITE_RETICLE_PRECISION, 292, False),
    ]
    reticle_style_btns = []
    reticle_style_outs = []
    for rname, rlabel, rsprite, rx, ractive in reticle_specs:
        r_go = b.alloc()
        r_rt = b.alloc()
        r_cr = b.alloc()
        r_img = b.alloc()
        r_btn = b.alloc()
        r_out = b.alloc()
        b.add_go(rname, card_ret, [r_cr, r_img, r_btn, r_out], (0.5, 1), (0.5, 1), (rx, -212), (266, 210), go_id=r_go, rt_id=r_rt)
        b.add_cr(r_go, r_cr)
        b.add_image(r_go, (0.14, 0.18, 0.12, 1.0) if ractive else C_INNER_DARK, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=1, img_id=r_img)
        b.add_button(r_go, r_img, interactable=1, btn_id=r_btn)
        b.add_outline(r_go, C_GOLD, dist=(4.0, -4.0), enabled=(1 if ractive else 0), out_id=r_out)
        # Reticle Icon inside card
        b.make_image_node(
            f"{rname}_Icon", r_rt, (0.5, 0.5), (0.5, 0.5), (0, 26), (104, 104),
            C_GOLD, sprite_guid=rsprite, img_type=0, preserve_aspect=1, raycast=0
        )
        # Reticle Label
        b.make_text_node(
            f"{rname}_Label", r_rt, (0.5, 0), (0.5, 0), (0, 34), (240, 40),
            rlabel, 24, C_TEXT_LIGHT, font_guid=FONT_SEMIBOLD, alignment=4
        )
        reticle_style_btns.append(r_btn)
        reticle_style_outs.append(r_out)

    # Color swatches label
    b.make_text_node(
        "ReticleColor_Label", card_ret, (0, 1), (0.6, 1), (44, -372), (0, 42),
        "Couleur du Réticule :", 25, C_TEXT_DIM, font_guid=FONT_SEMIBOLD, alignment=3
    )

    color_specs = [
        ("Swatch_Gold", C_GOLD, -360, True),
        ("Swatch_Green", C_SWATCH_GREEN, -248, False),
        ("Swatch_Red", C_SWATCH_RED, -136, False),
        ("Swatch_White", C_WHITE, -24, False),
    ]
    reticle_color_btns = []
    reticle_color_outs = []
    for cname, cval, cx, cactive in color_specs:
        c_go = b.alloc()
        c_rt = b.alloc()
        c_cr = b.alloc()
        c_img = b.alloc()
        c_btn = b.alloc()
        c_out = b.alloc()
        b.add_go(cname, card_ret, [c_cr, c_img, c_btn, c_out], (0.5, 1), (0.5, 1), (cx, -466), (82, 82), go_id=c_go, rt_id=c_rt)
        b.add_cr(c_go, c_cr)
        b.add_image(c_go, cval, sprite_guid=SPRITE_CIRCLE, img_type=0, raycast=1, img_id=c_img)
        b.add_button(c_go, c_img, interactable=1, btn_id=c_btn)
        b.add_outline(c_go, C_WHITE, dist=(5.0, -5.0), enabled=(1 if cactive else 0), out_id=c_out)
        reticle_color_btns.append(c_btn)
        reticle_color_outs.append(c_out)

    # Live Reticle Preview badge on bottom-right of Card 1
    _, prev_bg_rt, _ = b.make_image_node(
        "ReticlePreview_Bg", card_ret, (0.5, 1), (0.5, 1), (310, -456), (176, 176),
        C_INNER_DARK, sprite_guid=SPRITE_CIRCLE, img_type=0, raycast=0
    )
    _, _, reticle_preview_img = b.make_image_node(
        "ReticlePreview_Img", prev_bg_rt, (0.5, 0.5), (0.5, 0.5), (0, 0), (124, 124),
        C_GOLD, sprite_guid=SPRITE_RETICLE_NDOP, img_type=0, preserve_aspect=1, raycast=0
    )

    # Card 2 : Radar Moustiques
    card_radar = b.make_mod_card("Card_Radar", t3, -658, 360, "ASSISTANCE 360°", "Flèche Radar Moustiques")
    radar_btn, radar_bg, radar_knob = b.make_toggle_row(
        "Row_RadarToggle", card_radar, -104,
        "Indicateur Radar à l'Écran",
        "Pointe vers le moustique le plus proche",
        is_on=True, interactable=True
    )
    radar_sld, radar_val_txt = b.make_slider_row("Row_RadarRadius", card_radar, -226, "Rayon de la Flèche", "220 px", 120.0, 350.0, 220.0)

    # ==========================================================
    # TAB 4 : GRAPHISMES & BATTERIE AR (Capture 105837)
    # ==========================================================
    t4 = tab_content_rts[3]
    card_fps = b.make_mod_card("Card_FPS", t4, -22, 435, "BATTERIE SMARTPHONE", "Profil de Fréquence (FPS)")
    fps30_btn, fps60_btn = b.make_segmented_pair(
        "Seg_FPS", card_fps, -110,
        "30 FPS (Éco Batterie)", "60 FPS (Qualité Fluide)",
        left_active=False
    )
    b.make_note_box(
        "Note_FPS", card_fps, -226, 170,
        "Le mode 30 FPS préserve l'autonomie et réduit l'échauffement thermique lors des longues sessions AR à la caméra.",
        width=860
    )

    card_perf = b.make_mod_card("Card_PerfCounter", t4, -496, 235, "OUTIL TECHNIQUE", "Compteur de Performances")
    showfps_btn, showfps_bg, showfps_knob = b.make_toggle_row(
        "Row_ShowFPS", card_perf, -106,
        "Afficher les FPS en jeu",
        "Indicateur discret dans l'angle supérieur",
        is_on=False, interactable=True
    )

    # ==========================================================
    # TAB 5 : SYSTÈME & LANGUES + PSEUDO (Capture 105844)
    # ==========================================================
    t5 = tab_content_rts[4]
    card_lang = b.make_mod_card("Card_Language", t5, -16, 315, "RÉGIONALISATION", "Choix de la Langue")
    lang_fr_btn, lang_en_btn = b.make_segmented_pair(
        "Seg_Language", card_lang, -104,
        "FR  Français", "GB  English",
        left_active=True, height=76
    )
    b.make_note_box(
        "Note_Language", card_lang, -204, 86,
        "Sélection exclusive Français / Anglais conforme à votre décision.",
        width=860
    )

    # Card 2 : Profil & Pseudo du Chasseur (for Leaderboard!)
    card_pseudo = b.make_mod_card("Card_PlayerPseudo", t5, -360, 215, "PROFIL JOUEUR", "Pseudo du Chasseur")
    # InputField container
    inp_go = b.alloc()
    inp_rt = b.alloc()
    inp_cr = b.alloc()
    inp_img = b.alloc()
    inp_comp = b.alloc()
    b.add_go("PseudoInputField", card_pseudo, [inp_cr, inp_img, inp_comp], (0.5, 1), (0.5, 1), (0, -142), (860, 76), go_id=inp_go, rt_id=inp_rt)
    b.add_cr(inp_go, inp_cr)
    b.add_image(inp_go, C_INNER_DARK, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=1, img_id=inp_img)
    b.make_image_node(
        "PseudoInputBorder", inp_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        C_CARD_BORDER, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
    )
    _, _, inp_placeholder_id = b.make_text_node(
        "Placeholder", inp_rt, (0, 0), (1, 1), (0, 0), (-48, -12),
        "Entrez votre pseudo...", 26, (0.55, 0.55, 0.50, 0.7), font_guid=FONT_REGULAR, alignment=3
    )
    _, _, inp_text_id = b.make_text_node(
        "Text", inp_rt, (0, 0), (1, 1), (0, 0), (-48, -12),
        "Chasseur K-Mer", 28, C_GOLD, font_guid=FONT_BOLD, alignment=3, rich_text=0
    )
    b.add_inputfield_comp(inp_go, inp_img, inp_text_id, inp_placeholder_id, initial_text="Chasseur K-Mer", inp_id=inp_comp)

    # Card 3 : Données de Jeu (Meilleur Score + Reset)
    card_data = b.make_mod_card("Card_GameData", t5, -604, 240, "SAUVEGARDE LOCALE", "Données de Jeu")
    b.make_text_node(
        "BestScore_Label", card_data, (0, 1), (0.6, 1), (42, -118), (0, 38),
        "Meilleur Score Actuel", 25, C_TEXT_LIGHT, font_guid=FONT_SEMIBOLD, alignment=3
    )
    _, _, best_score_val_txt = b.make_text_node(
        "BestScore_Value", card_data, (0, 1), (0.6, 1), (42, -166), (0, 54),
        "12 450 PTS", 40, C_GOLD, font_guid=FONT_BOLD, alignment=3
    )
    _, _, reset_feedback_txt = b.make_text_node(
        "ResetFeedback_Text", card_data, (0, 1), (0.6, 1), (42, -210), (0, 30),
        "", 20, C_TEXT_DIM, font_guid=FONT_REGULAR, alignment=3
    )
    _, _, reset_score_btn, _ = b.make_pill_button(
        "ResetScoreButton", card_data, (1, 1), (1, 1), (-180, -152), (280, 80),
        C_RED_BG, "Réinitialiser", C_RED_TEXT, 26, font_guid=FONT_BOLD,
        sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, border_color=C_RED_BORDER
    )

    # Card 4 : À Propos du Jeu (Crédits)
    card_credits = b.make_mod_card("Card_Credits", t5, -874, 285, "CRÉDITS", "À Propos du Jeu")
    _, _, credits_txt = b.make_text_node(
        "CreditsText", card_credits, (0, 1), (1, 1), (0, -182), (-84, 180),
        "Développeur : Camatodev\nÉditeur : Vortex Makers x K-Mer Play\nMoteur : Unity 2022.3 LTS - Vuforia Engine AR\nVersion : v1.2 (Festival Pousse-Pions Build)",
        24, C_TEXT_DIM, font_guid=FONT_REGULAR, alignment=0, line_spacing=1.35
    )

    # ---------------- FOOTER (ENREGISTRER & RETOUR) ----------------
    b.make_image_node(
        "FooterDivider", modal_rt, (0.5, 0), (0.5, 0), (0, 184), (950, 2),
        C_DIVIDER, sprite_guid=None, raycast=0
    )
    _, _, save_btn_id, _ = b.make_pill_button(
        "SaveButton", modal_rt, (0.5, 0), (0.5, 0), (0, 108), (920, 96),
        C_GOLD, "ENREGISTRER & RETOUR", C_TEXT_DARK, 34, font_guid=FONT_BOLD,
        sprite_guid=SPRITE_PILL_ROUNDED, img_type=1
    )

    # ---------------- FILL SettingsPanelUI MONOBEHAVIOUR ----------------
    settings_ui_yaml = f"""--- !u!114 &{ROOT_SCRIPT}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {ROOT_GO}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_SETTINGS_UI}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  panelRoot: {{fileID: {ROOT_GO}}}
  closeButton: {{fileID: {close_btn_id}}}
  saveButton: {{fileID: {save_btn_id}}}
  headerTitleText: {{fileID: {header_title_txt}}}
  tabButtons:
  - {{fileID: {tab_btn_ids[0]}}}
  - {{fileID: {tab_btn_ids[1]}}}
  - {{fileID: {tab_btn_ids[2]}}}
  - {{fileID: {tab_btn_ids[3]}}}
  - {{fileID: {tab_btn_ids[4]}}}
  tabContents:
  - {{fileID: {tab_content_gos[0]}}}
  - {{fileID: {tab_content_gos[1]}}}
  - {{fileID: {tab_content_gos[2]}}}
  - {{fileID: {tab_content_gos[3]}}}
  - {{fileID: {tab_content_gos[4]}}}
  tabButtonTexts:
  - {{fileID: {tab_txt_ids[0]}}}
  - {{fileID: {tab_txt_ids[1]}}}
  - {{fileID: {tab_txt_ids[2]}}}
  - {{fileID: {tab_txt_ids[3]}}}
  - {{fileID: {tab_txt_ids[4]}}}
  masterSlider: {{fileID: {master_sld}}}
  masterValueText: {{fileID: {master_val_txt}}}
  sfxSlider: {{fileID: {sfx_sld}}}
  sfxValueText: {{fileID: {sfx_val_txt}}}
  bgmSlider: {{fileID: {bgm_sld}}}
  bgmValueText: {{fileID: {bgm_val_txt}}}
  buzzSlider: {{fileID: {buzz_sld}}}
  buzzValueText: {{fileID: {buzz_val_txt}}}
  muteGlobalButton: {{fileID: {mute_btn}}}
  muteGlobalToggleBg: {{fileID: {mute_bg}}}
  muteGlobalToggleKnob: {{fileID: {mute_knob}}}
  hapticBiteButton: {{fileID: {hap_bite_btn}}}
  hapticBiteToggleBg: {{fileID: {hap_bite_bg}}}
  hapticBiteToggleKnob: {{fileID: {hap_bite_knob}}}
  handednessRightButton: {{fileID: {hand_right_btn}}}
  handednessLeftButton: {{fileID: {hand_left_btn}}}
  gyroSlider: {{fileID: {gyro_sld}}}
  gyroValueText: {{fileID: {gyro_val_txt}}}
  reticleStyleButtons:
  - {{fileID: {reticle_style_btns[0]}}}
  - {{fileID: {reticle_style_btns[1]}}}
  - {{fileID: {reticle_style_btns[2]}}}
  reticleStyleOutlines:
  - {{fileID: {reticle_style_outs[0]}}}
  - {{fileID: {reticle_style_outs[1]}}}
  - {{fileID: {reticle_style_outs[2]}}}
  reticleColorButtons:
  - {{fileID: {reticle_color_btns[0]}}}
  - {{fileID: {reticle_color_btns[1]}}}
  - {{fileID: {reticle_color_btns[2]}}}
  - {{fileID: {reticle_color_btns[3]}}}
  reticleColorOutlines:
  - {{fileID: {reticle_color_outs[0]}}}
  - {{fileID: {reticle_color_outs[1]}}}
  - {{fileID: {reticle_color_outs[2]}}}
  - {{fileID: {reticle_color_outs[3]}}}
  reticlePreviewImage: {{fileID: {reticle_preview_img}}}
  reticleSprites:
  - {{fileID: 21300000, guid: {SPRITE_RETICLE_CLASSIC}, type: 3}}
  - {{fileID: 21300000, guid: {SPRITE_RETICLE_NDOP}, type: 3}}
  - {{fileID: 21300000, guid: {SPRITE_RETICLE_PRECISION}, type: 3}}
  radarToggleButton: {{fileID: {radar_btn}}}
  radarToggleBg: {{fileID: {radar_bg}}}
  radarToggleKnob: {{fileID: {radar_knob}}}
  radarRadiusSlider: {{fileID: {radar_sld}}}
  radarRadiusValueText: {{fileID: {radar_val_txt}}}
  fps30Button: {{fileID: {fps30_btn}}}
  fps60Button: {{fileID: {fps60_btn}}}
  showFPSButton: {{fileID: {showfps_btn}}}
  showFPSToggleBg: {{fileID: {showfps_bg}}}
  showFPSToggleKnob: {{fileID: {showfps_knob}}}
  langFRButton: {{fileID: {lang_fr_btn}}}
  langENButton: {{fileID: {lang_en_btn}}}
  pseudoInput: {{fileID: {inp_comp}}}
  bestScoreValueText: {{fileID: {best_score_val_txt}}}
  resetScoreButton: {{fileID: {reset_score_btn}}}
  resetScoreFeedbackText: {{fileID: {reset_feedback_txt}}}
  creditsText: {{fileID: {credits_txt}}}"""

    b.blocks[script_placeholder_idx] = ("raw", settings_ui_yaml)
    return b.render_yaml()


def strip_old_settings_panel(scene_content):
    """Removes every YAML document block whose fileID is in [320000100, 320000900) or [321000000, 322000000)."""
    parts = re.split(r'(?=^--- !u!)', scene_content, flags=re.MULTILINE)
    kept = []
    removed_count = 0
    for part in parts:
        m = re.match(r'^--- !u!\d+ &(\d+)', part)
        if m:
            fid = int(m.group(1))
            if (320000100 <= fid < 320000900) or (321000000 <= fid < 322000000):
                removed_count += 1
                continue
        kept.append(part)
    result = "".join(kept)
    if not result.endswith("\n"):
        result += "\n"
    return result, removed_count


def inject_into_scene(scene_path, parent_rt_id):
    with open(scene_path, "r", encoding="utf-8") as f:
        content = f.read()

    cleaned, removed = strip_old_settings_panel(content)
    new_panel_yaml = build_settings_panel_yaml(parent_rt_id)
    final_content = cleaned + new_panel_yaml

    # Verify zero duplicate fileIDs across entire scene!
    all_ids = re.findall(r'^--- !u!\d+ &(\d+)', final_content, flags=re.MULTILINE)
    unique_ids = set(all_ids)
    if len(all_ids) != len(unique_ids):
        dups = [x for x in unique_ids if all_ids.count(x) > 1]
        raise RuntimeError(f"Duplicate fileIDs detected in {scene_path}: {dups}")

    with open(scene_path, "w", encoding="utf-8", newline="\n") as f:
        f.write(final_content)

    print(f"[OK] {scene_path}: stripped {removed} old blocks, injected new rich SettingsPanel (total YAML objects: {len(all_ids)}, 0 duplicates).")


if __name__ == "__main__":
    inject_into_scene("Assets/Scenes/MainMenu_Vuforia.unity", parent_rt_id=326345399)
    inject_into_scene("Assets/Scenes/Level01_Vuforia.unity", parent_rt_id=89398172)
    print("Done! Both scenes updated cleanly.")
