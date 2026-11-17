import re
import sys
import os

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from rebuild_settings_panel import (
    UGuiBuilder,
    FONT_BOLD, FONT_SEMIBOLD, FONT_REGULAR,
    SPRITE_NDOP_RIBBON, SPRITE_CARD_ROUNDED, SPRITE_CARD_BORDER, SPRITE_PILL_ROUNDED, SPRITE_CIRCLE,
    C_BACKDROP, C_MODAL_BG, C_MODAL_BORDER, C_CARD_BG, C_CARD_BORDER, C_INNER_DARK,
    C_GOLD, C_TERRACOTTA, C_TEXT_LIGHT, C_TEXT_DIM, C_TEXT_DARK, C_DIVIDER, C_WHITE
)

SCENE_PATH = "Assets/Scenes/MainMenu_Vuforia.unity"
GUID_LEADERBOARD_UI = "b5555555555555555555555555555505"

C_SILVER = (0.831, 0.863, 0.890, 1.0)     # #D4DCE3
C_BRONZE = (0.851, 0.510, 0.271, 1.0)     # #D98245
C_RANK1_BG = (0.155, 0.145, 0.085, 1.0)
C_BANNER_BG = (0.125, 0.175, 0.110, 1.0)  # #202D1C
C_SAGE_RANK = (0.576, 0.769, 0.490, 1.0)  # #93C47D
C_ROW_BORDER = (0.282, 0.400, 0.220, 0.55)


def build_leaderboard_panel_yaml(parent_rt_id):
    b = UGuiBuilder(start_id=323000100)

    ROOT_GO = 323000000
    ROOT_RT = 323000001
    ROOT_CR = 323000002
    ROOT_IMG = 323000003
    ROOT_SCRIPT = 323000004

    b.add_go("LeaderboardPanel", parent_rt_id, [ROOT_CR, ROOT_IMG, ROOT_SCRIPT], (0, 0), (1, 1), (0, 0), (0, 0), active=0, go_id=ROOT_GO, rt_id=ROOT_RT)
    b.add_cr(ROOT_GO, ROOT_CR)
    b.add_image(ROOT_GO, C_BACKDROP, sprite_guid=None, raycast=1, img_id=ROOT_IMG)

    script_placeholder_idx = len(b.blocks)
    b.blocks.append(("raw", "PLACEHOLDER_LEADERBOARD_UI"))

    # ---------------- MODAL WINDOW (1020 x 1780) ----------------
    _, modal_rt, _ = b.make_image_node(
        "ModalWindow", ROOT_RT, (0.5, 0.5), (0.5, 0.5), (0, 0), (1020, 1780),
        C_MODAL_BG, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=1
    )
    b.make_image_node(
        "ModalBorder", modal_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        C_MODAL_BORDER, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
    )

    # Top & Bottom Ndop Ribbons
    b.make_image_node(
        "TopNdopRibbon", modal_rt, (0.5, 1), (0.5, 1), (0, -18), (1020, 36),
        C_WHITE, sprite_guid=SPRITE_NDOP_RIBBON, img_type=0, raycast=0
    )
    b.make_image_node(
        "BottomNdopRibbon", modal_rt, (0.5, 0), (0.5, 0), (0, 18), (1020, 36),
        C_WHITE, sprite_guid=SPRITE_NDOP_RIBBON, img_type=0, raycast=0
    )

    # ---------------- HEADER ----------------
    _, badge_rt, _ = b.make_image_node(
        "HeaderRankBadge", modal_rt, (0.5, 1), (0.5, 1), (-436, -96), (74, 74),
        C_GOLD, sprite_guid=SPRITE_CIRCLE, img_type=0, raycast=0
    )
    b.make_text_node(
        "HeaderRankBadgeText", badge_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        "#1", 32, C_TEXT_DARK, font_guid=FONT_BOLD, alignment=4
    )
    _, _, header_title_txt = b.make_text_node(
        "HeaderTitle", modal_rt, (0.5, 1), (0.5, 1), (-16, -96), (720, 68),
        "CLASSEMENT CHASSEURS", 40, C_GOLD, font_guid=FONT_BOLD, alignment=3
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

    # ---------------- SEGMENTED TOP 5 / TOP 10 ----------------
    top5_btn, top10_btn = b.make_segmented_pair(
        "Seg_TopCount", modal_rt, -176,
        "TOP 5", "TOP 10",
        left_active=True, width=940, height=82
    )

    # ---------------- CARD 1 : PODIUM ÉLITE (TOP 3) ----------------
    podium_card_go, podium_card_rt, _ = b.make_image_node(
        "Card_Podium", modal_rt, (0.5, 1), (0.5, 1), (0, -286), (940, 314),
        C_CARD_BG, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=0, pivot=(0.5, 1)
    )
    b.make_image_node(
        "Card_Podium_Border", podium_card_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        C_CARD_BORDER, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
    )
    _, ptag_rt, _ = b.make_image_node(
        "Card_Podium_TagBg", podium_card_rt, (1, 1), (1, 1), (-28, 0), (220, 36),
        C_TERRACOTTA, sprite_guid=SPRITE_PILL_ROUNDED, img_type=1, raycast=0, pivot=(1, 0.5)
    )
    _, _, podium_tag_txt = b.make_text_node(
        "Card_Podium_TagText", ptag_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        "PODIUM ÉLITE", 19, C_WHITE, font_guid=FONT_BOLD, alignment=4
    )

    # 3 Podium Slots: index 0 = Rank 1 (center tall), index 1 = Rank 2 (left), index 2 = Rank 3 (right)
    podium_specs = [
        ("Podium_Rank1", "1", 0, 304, 262, C_GOLD, C_RANK1_BG, C_GOLD, 66, 30, 26, 30),
        ("Podium_Rank2", "2", -300, 274, 236, C_SILVER, C_INNER_DARK, C_ROW_BORDER, 54, 26, 24, 26),
        ("Podium_Rank3", "3", 300, 274, 224, C_BRONZE, C_INNER_DARK, C_ROW_BORDER, 54, 26, 24, 26),
    ]
    podium_refs = []
    for pname, medal_num, px, pw, ph, medal_col, bg_col, brd_col, m_sz, m_fs, ps_fs, sc_fs in podium_specs:
        s_go, s_rt, s_bg = b.make_image_node(
            pname, podium_card_rt, (0.5, 0), (0.5, 0), (px, 22), (pw, ph),
            bg_col, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=0, pivot=(0.5, 0)
        )
        _, _, s_brd = b.make_image_node(
            f"{pname}_Border", s_rt, (0, 0), (1, 1), (0, 0), (0, 0),
            brd_col, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
        )
        _, m_rt, _ = b.make_image_node(
            f"{pname}_Medal", s_rt, (0.5, 1), (0.5, 1), (0, -48), (m_sz, m_sz),
            medal_col, sprite_guid=SPRITE_CIRCLE, img_type=0, raycast=0
        )
        b.make_text_node(
            f"{pname}_MedalText", m_rt, (0, 0), (1, 1), (0, 0), (0, 0),
            medal_num, m_fs, C_TEXT_DARK, font_guid=FONT_BOLD, alignment=4
        )
        _, _, s_pseudo = b.make_text_node(
            f"{pname}_Pseudo", s_rt, (0.5, 1), (0.5, 1), (0, -110), (pw - 24, 38),
            "Chasseur K-Mer", ps_fs, C_TEXT_LIGHT, font_guid=FONT_BOLD, alignment=4
        )
        _, _, s_score = b.make_text_node(
            f"{pname}_Score", s_rt, (0.5, 1), (0.5, 1), (0, -156), (pw - 20, 42),
            "12 400 PTS", sc_fs, C_GOLD, font_guid=FONT_BOLD, alignment=4
        )
        _, _, s_date = b.make_text_node(
            f"{pname}_Date", s_rt, (0.5, 0), (0.5, 0), (0, 24), (pw - 24, 30),
            "17/11", 20, C_TEXT_DIM, font_guid=FONT_REGULAR, alignment=4
        )
        podium_refs.append((s_go, s_bg, s_brd, s_pseudo, s_score, s_date))

    # ---------------- CARD 2 : CLASSEMENT LOCAL (#4..#10 + EMPTY STATE) ----------------
    _, list_card_rt, _ = b.make_image_node(
        "Card_RankingList", modal_rt, (0.5, 1), (0.5, 1), (0, -626), (940, 760),
        C_CARD_BG, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=0, pivot=(0.5, 1)
    )
    b.make_image_node(
        "Card_RankingList_Border", list_card_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        C_CARD_BORDER, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
    )
    _, ltag_rt, _ = b.make_image_node(
        "Card_RankingList_TagBg", list_card_rt, (1, 1), (1, 1), (-28, 0), (310, 36),
        C_TERRACOTTA, sprite_guid=SPRITE_PILL_ROUNDED, img_type=1, raycast=0, pivot=(1, 0.5)
    )
    _, _, list_tag_txt = b.make_text_node(
        "Card_RankingList_TagText", ltag_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        "CLASSEMENT LOCAL (TOP 5)", 19, C_WHITE, font_guid=FONT_BOLD, alignment=4
    )

    # Table Header Row
    lh_go, lh_rt = b.add_go("ListHeader", list_card_rt, [], (0.5, 1), (0.5, 1), (0, -44), (890, 44), pivot=(0.5, 0.5))
    b.make_text_node("Col_Rank", lh_rt, (0.5, 0.5), (0.5, 0.5), (-404, 0), (70, 36), "#", 21, C_TEXT_DIM, font_guid=FONT_BOLD, alignment=3)
    _, _, col_hunter_txt = b.make_text_node("Col_Hunter", lh_rt, (0.5, 0.5), (0.5, 0.5), (-148, 0), (400, 36), "CHASSEUR", 21, C_TEXT_DIM, font_guid=FONT_BOLD, alignment=3)
    _, _, col_score_txt = b.make_text_node("Col_Score", lh_rt, (0.5, 0.5), (0.5, 0.5), (206, 0), (220, 36), "SCORE", 21, C_TEXT_DIM, font_guid=FONT_BOLD, alignment=5)
    _, _, col_date_txt = b.make_text_node("Col_Date", lh_rt, (0.5, 0.5), (0.5, 0.5), (382, 0), (110, 36), "DATE", 21, C_TEXT_DIM, font_guid=FONT_BOLD, alignment=5)
    b.make_image_node("ListHeaderDivider", lh_rt, (0.5, 0), (0.5, 0), (0, -2), (890, 2), C_DIVIDER, sprite_guid=None, raycast=0)

    # 7 Score Rows (#4 to #10)
    row_refs = []
    for idx in range(7):
        rank_num = idx + 4
        ry = -78 - (idx * 94)
        r_go, r_rt, r_bg = b.make_image_node(
            f"Row_Rank{rank_num}", list_card_rt, (0.5, 1), (0.5, 1), (0, ry), (890, 84),
            C_INNER_DARK, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=0, pivot=(0.5, 1)
        )
        _, _, r_brd = b.make_image_node(
            f"Row_Rank{rank_num}_Border", r_rt, (0, 0), (1, 1), (0, 0), (0, 0),
            C_ROW_BORDER, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
        )
        _, _, r_rank = b.make_text_node(
            f"Row_Rank{rank_num}_Rank", r_rt, (0.5, 0.5), (0.5, 0.5), (-396, 0), (76, 44),
            f"#{rank_num}", 25, C_TEXT_DIM, font_guid=FONT_BOLD, alignment=3
        )
        _, _, r_pseudo = b.make_text_node(
            f"Row_Rank{rank_num}_Pseudo", r_rt, (0.5, 0.5), (0.5, 0.5), (-166, 0), (350, 44),
            "Chasseur K-Mer", 26, C_TEXT_LIGHT, font_guid=FONT_BOLD, alignment=3
        )
        # Mini "TOI" / "YOU" Gold Pill Badge
        me_go, me_rt, _ = b.make_image_node(
            f"Row_Rank{rank_num}_MeBadge", r_rt, (0.5, 0.5), (0.5, 0.5), (56, 0), (78, 32),
            C_GOLD, sprite_guid=SPRITE_PILL_ROUNDED, img_type=1, raycast=0
        )
        _, _, me_txt = b.make_text_node(
            f"Row_Rank{rank_num}_MeBadgeText", me_rt, (0, 0), (1, 1), (0, 0), (0, 0),
            "TOI", 18, C_TEXT_DARK, font_guid=FONT_BOLD, alignment=4
        )
        _, _, r_score = b.make_text_node(
            f"Row_Rank{rank_num}_Score", r_rt, (0.5, 0.5), (0.5, 0.5), (210, 0), (220, 44),
            "7 450 PTS", 26, C_GOLD, font_guid=FONT_BOLD, alignment=5
        )
        _, _, r_date = b.make_text_node(
            f"Row_Rank{rank_num}_Date", r_rt, (0.5, 0.5), (0.5, 0.5), (382, 0), (104, 44),
            "17/11", 22, C_TEXT_DIM, font_guid=FONT_REGULAR, alignment=5
        )
        row_refs.append((r_go, r_bg, r_brd, r_rank, r_pseudo, me_go, me_txt, r_score, r_date))

    # Hint when only 1-3 scores exist (all on podium, rows #4..#10 empty)
    hint_go, hint_rt = b.add_go("MoreGamesHint", list_card_rt, [], (0.5, 0.5), (0.5, 0.5), (0, -20), (800, 120), active=0)
    _, _, hint_txt = b.make_text_node(
        "MoreGamesHintText", hint_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        "Joue d'autres parties pour remplir le classement Top 5 !",
        26, C_TEXT_DIM, font_guid=FONT_REGULAR, alignment=4, line_spacing=1.3
    )

    # Empty State (0 score recorded / after Reset)
    emp_go, emp_rt = b.add_go("EmptyState", list_card_rt, [], (0.5, 0.5), (0.5, 0.5), (0, 10), (820, 380), active=0)
    _, e_icon_rt, _ = b.make_image_node(
        "EmptyIconBg", emp_rt, (0.5, 1), (0.5, 1), (0, -80), (124, 124),
        (0.16, 0.15, 0.08, 1.0), sprite_guid=SPRITE_CIRCLE, img_type=0, raycast=0
    )
    b.make_text_node(
        "EmptyIconZero", e_icon_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        "0", 54, C_GOLD, font_guid=FONT_BOLD, alignment=4
    )
    _, _, emp_title_txt = b.make_text_node(
        "EmptyTitle", emp_rt, (0.5, 1), (0.5, 1), (0, -190), (780, 56),
        "Aucun score enregistré", 34, C_GOLD, font_guid=FONT_BOLD, alignment=4
    )
    _, _, emp_desc_txt = b.make_text_node(
        "EmptyDesc", emp_rt, (0.5, 1), (0.5, 1), (0, -276), (760, 120),
        "Lance une partie pour entrer dans le classement sous le pseudo « Chasseur K-Mer » !",
        26, C_TEXT_DIM, font_guid=FONT_REGULAR, alignment=4, line_spacing=1.3
    )

    # ---------------- ACTIVE PLAYER SUMMARY BANNER ----------------
    _, ban_rt, _ = b.make_image_node(
        "ActivePlayerBanner", modal_rt, (0.5, 1), (0.5, 1), (0, -1410), (940, 140),
        C_BANNER_BG, sprite_guid=SPRITE_CARD_ROUNDED, img_type=1, raycast=0, pivot=(0.5, 1)
    )
    b.make_image_node(
        "ActivePlayerBanner_Border", ban_rt, (0, 0), (1, 1), (0, 0), (0, 0),
        C_MODAL_BORDER, sprite_guid=SPRITE_CARD_BORDER, img_type=1, raycast=0
    )
    _, _, active_prof_lbl = b.make_text_node(
        "ActiveProfileLabel", ban_rt, (0, 1), (0.58, 1), (36, -42), (0, 36),
        "PROFIL ACTIF (PARAMÈTRES)", 21, C_TEXT_DIM, font_guid=FONT_BOLD, alignment=3
    )
    _, _, active_pseudo_txt = b.make_text_node(
        "ActivePseudoValue", ban_rt, (0, 1), (0.58, 1), (36, -92), (0, 48),
        "Chasseur K-Mer", 32, C_TEXT_LIGHT, font_guid=FONT_BOLD, alignment=3
    )
    _, _, personal_best_txt = b.make_text_node(
        "PersonalBestValue", ban_rt, (0.55, 1), (1, 1), (-36, -46), (0, 50),
        "0 PTS", 36, C_GOLD, font_guid=FONT_BOLD, alignment=5
    )
    _, _, personal_rank_txt = b.make_text_node(
        "PersonalRankValue", ban_rt, (0.55, 1), (1, 1), (-36, -98), (0, 38),
        "Non classé", 23, C_SAGE_RANK, font_guid=FONT_BOLD, alignment=5
    )

    # ---------------- FOOTER CTA ----------------
    b.make_image_node(
        "FooterDivider", modal_rt, (0.5, 0), (0.5, 0), (0, 184), (950, 2),
        C_DIVIDER, sprite_guid=None, raycast=0
    )
    _, _, back_footer_btn, back_footer_txt = b.make_pill_button(
        "BackToMenuButton", modal_rt, (0.5, 0), (0.5, 0), (0, 108), (920, 96),
        C_GOLD, "RETOUR AU MENU", C_TEXT_DARK, 34, font_guid=FONT_BOLD,
        sprite_guid=SPRITE_PILL_ROUNDED, img_type=1
    )

    # ---------------- SERIALIZE LeaderboardPanelUI MONOBEHAVIOUR ----------------
    podium_yaml = "\n".join([
        f"""  - root: {{fileID: {p[0]}}}
    bgImage: {{fileID: {p[1]}}}
    borderImage: {{fileID: {p[2]}}}
    pseudoText: {{fileID: {p[3]}}}
    scoreText: {{fileID: {p[4]}}}
    dateText: {{fileID: {p[5]}}}"""
        for p in podium_refs
    ])

    rows_yaml = "\n".join([
        f"""  - root: {{fileID: {r[0]}}}
    bgImage: {{fileID: {r[1]}}}
    borderImage: {{fileID: {r[2]}}}
    rankText: {{fileID: {r[3]}}}
    pseudoText: {{fileID: {r[4]}}}
    meBadge: {{fileID: {r[5]}}}
    meBadgeText: {{fileID: {r[6]}}}
    scoreText: {{fileID: {r[7]}}}
    dateText: {{fileID: {r[8]}}}"""
        for r in row_refs
    ])

    leaderboard_ui_yaml = f"""--- !u!114 &{ROOT_SCRIPT}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {ROOT_GO}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_LEADERBOARD_UI}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  panelRoot: {{fileID: {ROOT_GO}}}
  closeButton: {{fileID: {close_btn_id}}}
  backFooterButton: {{fileID: {back_footer_btn}}}
  backFooterButtonText: {{fileID: {back_footer_txt}}}
  headerTitleText: {{fileID: {header_title_txt}}}
  top5Button: {{fileID: {top5_btn}}}
  top10Button: {{fileID: {top10_btn}}}
  podiumCardRoot: {{fileID: {podium_card_go}}}
  podiumTagText: {{fileID: {podium_tag_txt}}}
  podiumSlots:
{podium_yaml}
  listTagText: {{fileID: {list_tag_txt}}}
  listHeaderRoot: {{fileID: {lh_go}}}
  colHunterText: {{fileID: {col_hunter_txt}}}
  colScoreText: {{fileID: {col_score_txt}}}
  colDateText: {{fileID: {col_date_txt}}}
  rankRows:
{rows_yaml}
  moreGamesHintRoot: {{fileID: {hint_go}}}
  moreGamesHintText: {{fileID: {hint_txt}}}
  emptyStateRoot: {{fileID: {emp_go}}}
  emptyTitleText: {{fileID: {emp_title_txt}}}
  emptyDescText: {{fileID: {emp_desc_txt}}}
  activeProfileLabelText: {{fileID: {active_prof_lbl}}}
  activePseudoText: {{fileID: {active_pseudo_txt}}}
  personalBestText: {{fileID: {personal_best_txt}}}
  personalRankText: {{fileID: {personal_rank_txt}}}"""

    b.blocks[script_placeholder_idx] = ("raw", leaderboard_ui_yaml)
    return b.render_yaml()


def strip_leaderboard(content):
    parts = re.split(r'(?=^--- !u!)', content, flags=re.MULTILINE)
    kept = []
    removed = 0
    for p in parts:
        m = re.match(r'^--- !u!\d+ &(\d+)', p)
        if m and 323000000 <= int(m.group(1)) < 324000000:
            removed += 1
            continue
        kept.append(p)
    res = "".join(kept)
    if not res.endswith("\n"):
        res += "\n"
    return res, removed


def inject_into_main_menu():
    with open(SCENE_PATH, "r", encoding="utf-8") as f:
        content = f.read()

    content, removed = strip_leaderboard(content)

    # 1. Add LeaderboardPanel RT (323000001) to Background (&326345399) m_Children
    bg_m = re.search(r'(--- !u!224 &326345399[\s\S]*?m_Children:[\s\S]*?)(?=  m_Father:)', content)
    if bg_m:
        bg_block = bg_m.group(1)
        if "323000001" not in bg_block:
            bg_block += "  - {fileID: 323000001}\n"
            content = content.replace(bg_m.group(1), bg_block, 1)

    # 2. Wire leaderboardPanel: {fileID: 323000004} on MainMenu script (114 &513843498)
    mm_m = re.search(r'(--- !u!114 &513843498[\s\S]*?)(?=^--- !u!|\Z)', content, flags=re.MULTILINE)
    if mm_m:
        mm_block = mm_m.group(1)
        if "leaderboardPanel:" not in mm_block:
            mm_block = mm_block.rstrip() + "\n  leaderboardPanel: {fileID: 323000004}\n"
            content = content.replace(mm_m.group(1), mm_block, 1)

    # 3. Re-wire ScoreButton (114 &320000024) OnClick from Settings -> OpenLeaderboard
    sb_m = re.search(r'(--- !u!114 &320000024[\s\S]*?)(?=^--- !u!|\Z)', content, flags=re.MULTILINE)
    if sb_m:
        sb_block = sb_m.group(1)
        sb_block = sb_block.replace("m_MethodName: Settings", "m_MethodName: OpenLeaderboard")
        content = content.replace(sb_m.group(1), sb_block, 1)

    # 4. Append LeaderboardPanel YAML
    lb_yaml = build_leaderboard_panel_yaml(parent_rt_id=326345399)
    final_content = content + lb_yaml

    # Verify zero duplicates and zero dangling refs
    all_ids = re.findall(r'^--- !u!\d+ &(\d+)', final_content, flags=re.MULTILINE)
    if len(all_ids) != len(set(all_ids)):
        dups = [x for x in set(all_ids) if all_ids.count(x) > 1]
        raise RuntimeError(f"Duplicate fileIDs in {SCENE_PATH}: {dups}")

    defined = set(all_ids)
    lb_refs = re.findall(r'\{fileID: (\d+)\}', lb_yaml)
    missing = [r for r in set(lb_refs) if r != '0' and r not in defined]
    if missing:
        raise RuntimeError(f"Missing refs in LeaderboardPanel: {missing}")

    with open(SCENE_PATH, "w", encoding="utf-8", newline="\n") as f:
        f.write(final_content)

    print(f"[OK] {SCENE_PATH}: stripped {removed} old leaderboard blocks, injected complete LeaderboardPanel (total YAML objects: {len(all_ids)}, 0 duplicates, 0 missing refs).")


if __name__ == "__main__":
    inject_into_main_menu()
